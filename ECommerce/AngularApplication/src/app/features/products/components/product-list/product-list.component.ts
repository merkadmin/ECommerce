import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { SubSink } from 'subsink';
import { ProductService } from '../../services/product.service';
import { GlobalService, FixedService } from '@core/services';
import { Product, ProductFilter, PagedResult, Category, Retailer, PaginationModel } from '@core/models';
import { ProductCardComponent } from '@shared/components/product-card/product-card.component';
import { PaginationComponent } from '@shared/components/pagination/pagination.component';
import { CrInputComponent } from '@shared/components/cr-input/cr-input.component';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    ProductCardComponent,
    PaginationComponent,
    CrInputComponent
  ],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.less']
})
export class ProductListComponent implements OnInit, OnDestroy {
  private subs = new SubSink();

  products: Product[] = [];
  totalProducts: number = 0;
  isLoading: boolean = false;
  viewMode: 'grid' | 'list' = 'grid';

  // Filters
  filter: ProductFilter = {
    search: '',
    categoryId: undefined,
    retailerIds: [],
    minPrice: undefined,
    maxPrice: undefined,
    sortBy: 'newest',
    page: 1,
    pageSize: 20
  };

  // Filter options
  categories: Category[] = [];
  retailers: Retailer[] = [];
  sortOptions: any[] = [];
  priceRanges: any[] = [];

  // UI State
  showFilters: boolean = true;
  wishlistIds: Set<string> = new Set();
  compareIds: Set<string> = new Set();

  constructor(
    private productService: ProductService,
    public globalService: GlobalService,
    public fixedService: FixedService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.sortOptions = this.fixedService.productSortOptions;
    this.priceRanges = this.fixedService.priceRanges;

    // Get query params
    this.subs.sink = this.route.queryParams.subscribe(params => {
      if (params['search']) {
        this.filter.search = params['search'];
      }
      if (params['category']) {
        this.filter.categoryId = params['category'];
      }
      this.loadProducts();
    });

    // Load filter options
    this.loadCategories();
    this.loadRetailers();
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  loadProducts(): void {
    this.isLoading = true;
    this.subs.sink = this.productService.getProducts(this.filter).subscribe({
      next: (result: PagedResult<Product>) => {
        this.products = result.items;
        this.totalProducts = result.total;
        this.isLoading = false;
      },
      error: (error) => {
        this.globalService.notificationMessage('error', 'Failed to load products');
        this.isLoading = false;
      }
    });
  }

  loadCategories(): void {
    // TODO: Load from CategoryService
  }

  loadRetailers(): void {
    // TODO: Load from RetailerService
  }

  onSearchSetup(searchObservable: Observable<string>): void {
    this.subs.sink = searchObservable.subscribe(query => {
      this.filter.search = query;
      this.filter.page = 1;
      this.loadProducts();
      this.updateUrl();
    });
  }

  onSearchClear(): void {
    this.filter.search = '';
    this.filter.page = 1;
    this.loadProducts();
    this.updateUrl();
  }

  onFilterChange(): void {
    this.filter.page = 1;
    this.loadProducts();
    this.updateUrl();
  }

  onSortChange(sortBy: string): void {
    this.filter.sortBy = sortBy as ProductFilter['sortBy'];
    this.loadProducts();
  }

  onPaginationChange(pagination: PaginationModel): void {
    this.filter.page = pagination.currentPage;
    this.filter.pageSize = pagination.recordPerPage;
    this.loadProducts();
  }

  onPriceRangeSelect(range: { min: number; max: number | null }): void {
    this.filter.minPrice = range.min;
    this.filter.maxPrice = range.max ?? undefined;
    this.onFilterChange();
  }

  clearFilters(): void {
    this.filter = {
      search: '',
      categoryId: undefined,
      retailerIds: [],
      minPrice: undefined,
      maxPrice: undefined,
      sortBy: 'newest',
      page: 1,
      pageSize: 20
    };
    this.loadProducts();
    this.updateUrl();
  }

  toggleViewMode(): void {
    this.viewMode = this.viewMode === 'grid' ? 'list' : 'grid';
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  // Wishlist actions
  onAddToWishlist(product: Product): void {
    this.wishlistIds.add(product.id);
    this.globalService.notificationMessage('success', 'Added to wishlist');
    // TODO: Call WishlistService
  }

  onRemoveFromWishlist(product: Product): void {
    this.wishlistIds.delete(product.id);
    this.globalService.notificationMessage('info', 'Removed from wishlist');
    // TODO: Call WishlistService
  }

  // Compare actions
  onAddToCompare(product: Product): void {
    if (this.compareIds.size >= 4) {
      this.globalService.notificationMessage('warning', 'You can compare up to 4 products');
      return;
    }
    this.compareIds.add(product.id);
    this.globalService.notificationMessage('success', 'Added to compare');
  }

  onRemoveFromCompare(product: Product): void {
    this.compareIds.delete(product.id);
    this.globalService.notificationMessage('info', 'Removed from compare');
  }

  // Price alert
  onSetAlert(product: Product): void {
    // TODO: Open price alert modal
    this.globalService.notificationMessage('info', 'Price alert feature coming soon');
  }

  // View details
  onViewDetails(product: Product): void {
    this.router.navigate(['/products', product.id]);
  }

  isInWishlist(productId: string): boolean {
    return this.wishlistIds.has(productId);
  }

  isInCompare(productId: string): boolean {
    return this.compareIds.has(productId);
  }

  getCompareIds(): string {
    return Array.from(this.compareIds).join(',');
  }

  private updateUrl(): void {
    const queryParams: any = {};
    if (this.filter.search) {
      queryParams.search = this.filter.search;
    }
    if (this.filter.categoryId) {
      queryParams.category = this.filter.categoryId;
    }
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams,
      queryParamsHandling: 'merge'
    });
  }
}
