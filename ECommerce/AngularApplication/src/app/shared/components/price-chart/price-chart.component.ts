import { Component, Input, OnChanges, SimpleChanges, ViewChild, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chart, ChartConfiguration, ChartType, registerables } from 'chart.js';
import { PriceHistory } from '@core/models';
import { GlobalService } from '@core/services';

Chart.register(...registerables);

@Component({
  selector: 'app-price-chart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './price-chart.component.html',
  styleUrls: ['./price-chart.component.less']
})
export class PriceChartComponent implements AfterViewInit, OnChanges, OnDestroy {
  @ViewChild('chartCanvas') chartCanvas!: ElementRef<HTMLCanvasElement>;

  @Input() priceHistory: PriceHistory[] = [];
  @Input() height: number = 300;
  @Input() showLegend: boolean = true;

  private chart: Chart | null = null;

  constructor(private globalService: GlobalService) {}

  ngAfterViewInit(): void {
    this.createChart();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['priceHistory'] && !changes['priceHistory'].firstChange) {
      this.updateChart();
    }
  }

  ngOnDestroy(): void {
    if (this.chart) {
      this.chart.destroy();
    }
  }

  private createChart(): void {
    if (!this.chartCanvas || this.priceHistory.length === 0) return;

    const ctx = this.chartCanvas.nativeElement.getContext('2d');
    if (!ctx) return;

    const { labels, datasets } = this.prepareChartData();

    const config: ChartConfiguration = {
      type: 'line' as ChartType,
      data: {
        labels,
        datasets
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        interaction: {
          mode: 'index',
          intersect: false
        },
        plugins: {
          legend: {
            display: this.showLegend,
            position: 'top',
            labels: {
              usePointStyle: true,
              padding: 20
            }
          },
          tooltip: {
            backgroundColor: 'rgba(0, 0, 0, 0.8)',
            padding: 12,
            titleFont: {
              size: 14,
              weight: 'bold'
            },
            bodyFont: {
              size: 13
            },
            callbacks: {
              label: (context) => {
                const value = context.parsed.y;
                return `${context.dataset.label}: ${this.globalService.formatMoney(value ?? 0)}`;
              }
            }
          }
        },
        scales: {
          x: {
            grid: {
              display: false
            },
            ticks: {
              maxRotation: 45,
              minRotation: 45
            }
          },
          y: {
            beginAtZero: false,
            grid: {
              color: 'rgba(0, 0, 0, 0.05)'
            },
            ticks: {
              callback: (value) => this.globalService.formatMoney(value as number)
            }
          }
        }
      }
    };

    this.chart = new Chart(ctx, config);
  }

  private updateChart(): void {
    if (this.chart) {
      const { labels, datasets } = this.prepareChartData();
      this.chart.data.labels = labels;
      this.chart.data.datasets = datasets;
      this.chart.update();
    } else {
      this.createChart();
    }
  }

  private prepareChartData(): { labels: string[]; datasets: any[] } {
    // Group by retailer
    const retailerData = new Map<string, { name: string; prices: { date: string; price: number }[] }>();

    this.priceHistory.forEach((item) => {
      const retailerId = item.retailerId;
      if (!retailerData.has(retailerId)) {
        retailerData.set(retailerId, {
          name: item.retailerName,
          prices: []
        });
      }
      retailerData.get(retailerId)!.prices.push({
        date: this.globalService.formatDate(item.date, 'MMM DD'),
        price: item.price
      });
    });

    // Get unique dates
    const allDates = [...new Set(this.priceHistory.map(p => this.globalService.formatDate(p.date, 'MMM DD')))];
    allDates.sort((a, b) => new Date(a).getTime() - new Date(b).getTime());

    // Color palette
    const colors = [
      { border: '#1176bd', bg: 'rgba(17, 118, 189, 0.1)' },
      { border: '#11b7bd', bg: 'rgba(17, 183, 189, 0.1)' },
      { border: '#17dd8a', bg: 'rgba(23, 221, 138, 0.1)' },
      { border: '#ffa500', bg: 'rgba(255, 165, 0, 0.1)' },
      { border: '#dd0a35', bg: 'rgba(221, 10, 53, 0.1)' },
      { border: '#6858ff', bg: 'rgba(104, 88, 255, 0.1)' }
    ];

    // Create datasets
    const datasets: any[] = [];
    let colorIndex = 0;

    retailerData.forEach((data, retailerId) => {
      const color = colors[colorIndex % colors.length];

      // Map prices to dates
      const priceMap = new Map(data.prices.map(p => [p.date, p.price]));
      const dataPoints = allDates.map(date => priceMap.get(date) ?? null);

      datasets.push({
        label: data.name,
        data: dataPoints,
        borderColor: color.border,
        backgroundColor: color.bg,
        borderWidth: 2,
        fill: true,
        tension: 0.4,
        pointRadius: 4,
        pointHoverRadius: 6,
        pointBackgroundColor: color.border,
        spanGaps: true
      });

      colorIndex++;
    });

    return { labels: allDates, datasets };
  }
}
