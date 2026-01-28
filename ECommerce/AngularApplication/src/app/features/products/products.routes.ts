import { Routes } from '@angular/router';

export const PRODUCTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./components/product-list/product-list.component')
      .then(m => m.ProductListComponent),
    title: 'Products - PriceCompare'
  },
  {
    path: 'trending',
    loadComponent: () => import('./components/trending-products/trending-products.component')
      .then(m => m.TrendingProductsComponent),
    title: 'Trending Products - PriceCompare'
  },
  {
    path: 'deals',
    loadComponent: () => import('./components/best-deals/best-deals.component')
      .then(m => m.BestDealsComponent),
    title: 'Best Deals - PriceCompare'
  },
  {
    path: 'compare',
    loadComponent: () => import('./components/product-compare/product-compare.component')
      .then(m => m.ProductCompareComponent),
    title: 'Compare Products - PriceCompare'
  },
  {
    path: ':id',
    loadComponent: () => import('./components/product-detail/product-detail.component')
      .then(m => m.ProductDetailComponent),
    title: 'Product Details - PriceCompare'
  }
];
