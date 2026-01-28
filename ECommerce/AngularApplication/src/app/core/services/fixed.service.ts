import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface NavItem {
  id: string;
  name: string;
  icon: string;
  relativeURL?: string;
  externalLink?: string;
  openInNewTab?: boolean;
  children?: NavItem[];
  isBeta?: boolean;
  isHidden?: boolean;
  badge?: number;
}

export interface SidebarItem {
  id: string;
  name: string;
  icon: string;
  relativeURL?: string;
  children?: SidebarItem[];
  isBeta?: boolean;
  isHidden?: boolean;
  notificationCount?: number;
  activeUrl?: string;
}

@Injectable({
  providedIn: 'root'
})
export class FixedService {
  // UI State
  showNavBar: boolean = true;
  showSideBar: boolean = true;
  showFooter: boolean = true;

  // Observable sidebar items for dynamic updates
  sidebarItems$ = new BehaviorSubject<SidebarItem[]>([]);

  // Search debounce time
  readonly waitingTimeForInstanceSearch = 300;

  // Pagination defaults
  readonly defaultPageSize = 20;
  readonly pageSizeOptions = [10, 20, 50, 100];

  // Currency
  readonly defaultCurrency = '$';

  // Date formats
  readonly dateFormat = 'MMM DD, YYYY';
  readonly dateTimeFormat = 'MMM DD, YYYY HH:mm';

  // Static Sidebar Items
  readonly SidebarItems: SidebarItem[] = [
    {
      id: 'dashboard',
      name: 'Dashboard',
      icon: 'icon-dashboard',
      relativeURL: '/dashboard'
    },
    {
      id: 'products',
      name: 'Products',
      icon: 'icon-box',
      relativeURL: '/products',
      children: [
        { id: 'product-list', name: 'All Products', icon: 'icon-list', relativeURL: '/products' },
        { id: 'compare', name: 'Compare', icon: 'icon-compare', relativeURL: '/products/compare' },
        { id: 'trending', name: 'Trending', icon: 'icon-trending', relativeURL: '/products/trending' },
        { id: 'deals', name: 'Best Deals', icon: 'icon-percent', relativeURL: '/products/deals' }
      ]
    },
    {
      id: 'categories',
      name: 'Categories',
      icon: 'icon-category',
      relativeURL: '/categories'
    },
    {
      id: 'retailers',
      name: 'Retailers',
      icon: 'icon-store',
      relativeURL: '/retailers'
    },
    {
      id: 'alerts',
      name: 'Price Alerts',
      icon: 'icon-bell',
      relativeURL: '/alerts',
      notificationCount: 0
    },
    {
      id: 'wishlist',
      name: 'Wishlist',
      icon: 'icon-heart',
      relativeURL: '/wishlist'
    },
    {
      id: 'cart',
      name: 'Cart',
      icon: 'icon-cart',
      relativeURL: '/cart',
      notificationCount: 0
    }
  ];

  // Nav Items (top navigation)
  readonly NavItems: NavItem[] = [
    {
      id: 'browse',
      name: 'Browse',
      icon: 'icon-browse',
      children: [
        { id: 'all-products', name: 'All Products', icon: 'icon-list', relativeURL: '/products' },
        { id: 'categories', name: 'Categories', icon: 'icon-category', relativeURL: '/categories' },
        { id: 'retailers', name: 'Retailers', icon: 'icon-store', relativeURL: '/retailers' }
      ]
    },
    {
      id: 'deals',
      name: 'Deals',
      icon: 'icon-percent',
      relativeURL: '/products/deals'
    }
  ];

  // Sort options for products
  readonly productSortOptions = [
    { value: 'price_asc', label: 'Price: Low to High' },
    { value: 'price_desc', label: 'Price: High to Low' },
    { value: 'name_asc', label: 'Name: A to Z' },
    { value: 'name_desc', label: 'Name: Z to A' },
    { value: 'discount', label: 'Biggest Discount' },
    { value: 'newest', label: 'Newest First' },
    { value: 'rating', label: 'Highest Rated' }
  ];

  // Price range presets
  readonly priceRanges = [
    { label: 'Under $25', min: 0, max: 25 },
    { label: '$25 - $50', min: 25, max: 50 },
    { label: '$50 - $100', min: 50, max: 100 },
    { label: '$100 - $200', min: 100, max: 200 },
    { label: '$200 - $500', min: 200, max: 500 },
    { label: 'Over $500', min: 500, max: null }
  ];

  // Chart colors
  readonly chartColors = {
    primary: '#1176bd',
    secondary: '#11b7bd',
    success: '#17dd8a',
    danger: '#dd0a35',
    warning: '#ffa500',
    info: '#5fa8ff'
  };

  // Badge configurations for product status
  readonly productStatusBadges = [
    { value: 'Best Deal', colorClass: 'badge-outline-success' },
    { value: 'Price Drop', colorClass: 'badge-outline-primary' },
    { value: 'New', colorClass: 'badge-outline-info' },
    { value: 'Normal', colorClass: 'badge-outline-secondary' }
  ];

  // Alert status badges
  readonly alertStatusBadges = [
    { value: 'Active', colorClass: 'badge-outline-primary' },
    { value: 'Triggered', colorClass: 'badge-outline-success' },
    { value: 'Expired', colorClass: 'badge-outline-warning' },
    { value: 'Disabled', colorClass: 'badge-outline-secondary' }
  ];

  constructor() {
    this.sidebarItems$.next(this.SidebarItems);
  }

  // Update sidebar notification counts
  updateSidebarNotification(itemId: string, count: number): void {
    const items = [...this.sidebarItems$.value];
    const item = items.find(i => i.id === itemId);
    if (item) {
      item.notificationCount = count;
      this.sidebarItems$.next(items);
    }
  }
}
