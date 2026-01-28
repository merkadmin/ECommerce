import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { SubSink } from 'subsink';
import { ProductService } from '../../services/product.service';
import { GlobalService, FixedService } from '@core/services';
import { ProductDetail, RetailerPrice, PriceHistory } from '@core/models';
import { PriceChartComponent } from '@shared/components/price-chart/price-chart.component';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, PriceChartComponent],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.less']
})
export class ProductDetailComponent implements OnInit, OnDestroy {
  private subs = new SubSink();

  product: ProductDetail | null = null;
  priceHistory: PriceHistory[] = [];
  isLoading: boolean = true;
  activeTab: 'prices' | 'history' | 'specs' = 'prices';
  historyDays: number = 30;

  constructor(
    private productService: ProductService,
    public globalService: GlobalService,
    public fixedService: FixedService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.subs.sink = this.route.params.subscribe(params => {
      if (params['id']) {
        this.loadProduct(params['id']);
      }
    });
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  loadProduct(id: string): void {
    this.isLoading = true;

    this.subs.sink = this.productService.getProduct(id).subscribe({
      next: (product) => {
        this.product = product;
        this.isLoading = false;
        this.loadPriceHistory(id);
      },
      error: (error) => {
        this.globalService.notificationMessage('error', 'Failed to load product');
        this.isLoading = false;
        this.router.navigate(['/products']);
      }
    });
  }

  loadPriceHistory(productId: string): void {
    this.subs.sink = this.productService.getPriceHistory(productId, this.historyDays).subscribe({
      next: (history) => {
        this.priceHistory = history;
      },
      error: () => {
        // Silently fail for history
      }
    });
  }

  onHistoryDaysChange(days: number): void {
    this.historyDays = days;
    if (this.product) {
      this.loadPriceHistory(this.product.id);
    }
  }

  get lowestPriceRetailer(): RetailerPrice | null {
    if (!this.product?.prices?.length) return null;
    return this.product.prices.reduce((lowest, current) =>
      current.currentPrice < lowest.currentPrice ? current : lowest
    );
  }

  get savings(): number {
    if (!this.product) return 0;
    return this.product.highestPrice - this.product.lowestPrice;
  }

  get savingsPercent(): number {
    if (!this.product || this.product.highestPrice === 0) return 0;
    return Math.round((this.savings / this.product.highestPrice) * 100);
  }

  openRetailerLink(price: RetailerPrice): void {
    if (price.productUrl) {
      window.open(price.productUrl, '_blank');
    }
  }

  addToWishlist(): void {
    this.globalService.notificationMessage('success', 'Added to wishlist');
    // TODO: Call WishlistService
  }

  setAlert(): void {
    this.globalService.notificationMessage('info', 'Price alert feature coming soon');
    // TODO: Open price alert modal
  }

  shareProduct(): void {
    if (navigator.share) {
      navigator.share({
        title: this.product?.name,
        text: `Check out this product: ${this.product?.name}`,
        url: window.location.href
      });
    } else {
      this.globalService.copyToClipboard(window.location.href);
    }
  }
}
