import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Product } from '@core/models';
import { GlobalService } from '@core/services';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.less'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductCardComponent {
  @Input() product!: Product;
  @Input() showCompareButton: boolean = true;
  @Input() showWishlistButton: boolean = true;
  @Input() isInWishlist: boolean = false;
  @Input() isInCompare: boolean = false;

  @Output() addToWishlist = new EventEmitter<Product>();
  @Output() removeFromWishlist = new EventEmitter<Product>();
  @Output() addToCompare = new EventEmitter<Product>();
  @Output() removeFromCompare = new EventEmitter<Product>();
  @Output() setAlert = new EventEmitter<Product>();
  @Output() viewDetails = new EventEmitter<Product>();

  constructor(public globalService: GlobalService) {}

  get discountPercent(): number | null {
    if (this.product.highestPrice && this.product.lowestPrice < this.product.highestPrice) {
      return Math.round(((this.product.highestPrice - this.product.lowestPrice) / this.product.highestPrice) * 100);
    }
    return null;
  }

  get statusBadgeClass(): string {
    switch (this.product.status) {
      case 'Best Deal':
        return 'badge-success';
      case 'Price Drop':
        return 'badge-primary';
      case 'New':
        return 'badge-info';
      default:
        return 'badge-secondary';
    }
  }

  onWishlistClick(event: Event): void {
    event.stopPropagation();
    if (this.isInWishlist) {
      this.removeFromWishlist.emit(this.product);
    } else {
      this.addToWishlist.emit(this.product);
    }
  }

  onCompareClick(event: Event): void {
    event.stopPropagation();
    if (this.isInCompare) {
      this.removeFromCompare.emit(this.product);
    } else {
      this.addToCompare.emit(this.product);
    }
  }

  onAlertClick(event: Event): void {
    event.stopPropagation();
    this.setAlert.emit(this.product);
  }

  onCardClick(): void {
    this.viewDetails.emit(this.product);
  }
}
