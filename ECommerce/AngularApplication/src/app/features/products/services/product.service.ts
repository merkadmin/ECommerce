import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '@core/services';
import { ApiController } from '@core/enums';
import {
  Product,
  ProductDetail,
  ProductFilter,
  PagedResult,
  PriceComparison,
  PriceHistory
} from '@core/models';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  constructor(private apiService: ApiService) {}

  // Get products with filters and pagination
  getProducts(filter: ProductFilter): Observable<PagedResult<Product>> {
    return this.apiService.search<Product>(ApiController.Products, filter);
  }

  // Get single product details
  getProduct(id: string): Observable<ProductDetail> {
    return this.apiService.getById<ProductDetail>(ApiController.Products, id);
  }

  // Get price comparison for a product
  getPriceComparison(productId: string): Observable<PriceComparison> {
    return this.apiService.getByParams<PriceComparison>(
      ApiController.Products,
      `${productId}/compare`,
      {}
    );
  }

  // Get price history for a product
  getPriceHistory(productId: string, days: number = 30): Observable<PriceHistory[]> {
    return this.apiService.getByParams<PriceHistory[]>(
      ApiController.Products,
      `${productId}/history`,
      { days: days.toString() }
    );
  }

  // Get trending products
  getTrending(limit: number = 10): Observable<Product[]> {
    return this.apiService.getByParams<Product[]>(
      ApiController.Products,
      'trending',
      { limit: limit.toString() }
    );
  }

  // Get best deals
  getDeals(limit: number = 10): Observable<Product[]> {
    return this.apiService.getByParams<Product[]>(
      ApiController.Products,
      'deals',
      { limit: limit.toString() }
    );
  }

  // Search products
  searchProducts(query: string): Observable<Product[]> {
    return this.apiService.getByParams<Product[]>(
      ApiController.Products,
      'search',
      { q: query }
    );
  }

  // Get products by category
  getProductsByCategory(categoryId: string, page: number = 1, pageSize: number = 20): Observable<PagedResult<Product>> {
    const filter: ProductFilter = {
      categoryId,
      page,
      pageSize
    };
    return this.getProducts(filter);
  }

  // Get products by retailer
  getProductsByRetailer(retailerId: string, page: number = 1, pageSize: number = 20): Observable<PagedResult<Product>> {
    const filter: ProductFilter = {
      retailerIds: [retailerId],
      page,
      pageSize
    };
    return this.getProducts(filter);
  }
}
