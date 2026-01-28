import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SubSink } from 'subsink';
import { ProductService } from '../products/services/product.service';
import { GlobalService, FixedService } from '@core/services';
import { Product } from '@core/models';
import { ProductCardComponent } from '@shared/components/product-card/product-card.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, ProductCardComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.less']
})
export class DashboardComponent implements OnInit, OnDestroy {
  private subs = new SubSink();

  trendingProducts: Product[] = [];
  bestDeals: Product[] = [];
  isLoadingTrending: boolean = true;
  isLoadingDeals: boolean = true;

  // Stats
  stats = {
    totalProducts: 0,
    totalRetailers: 0,
    priceDrops: 0,
    activeAlerts: 0
  };

  constructor(
    private productService: ProductService,
    public globalService: GlobalService,
    public fixedService: FixedService
  ) {}

  ngOnInit(): void {
    this.loadTrendingProducts();
    this.loadBestDeals();
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  loadTrendingProducts(): void {
    this.isLoadingTrending = true;
    this.subs.sink = this.productService.getTrending(8).subscribe({
      next: (products) => {
        this.trendingProducts = products;
        this.isLoadingTrending = false;
      },
      error: () => {
        this.isLoadingTrending = false;
      }
    });
  }

  loadBestDeals(): void {
    this.isLoadingDeals = true;
    this.subs.sink = this.productService.getDeals(8).subscribe({
      next: (products) => {
        this.bestDeals = products;
        this.isLoadingDeals = false;
      },
      error: () => {
        this.isLoadingDeals = false;
      }
    });
  }
}
