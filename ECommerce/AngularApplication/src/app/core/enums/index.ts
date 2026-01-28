// API Action Names
export enum ApiAction {
  GetAll = 'GetAll',
  GetByID = 'GetByID',
  GetByParams = 'GetByParams',
  Insert = 'Insert',
  Update = 'Update',
  Delete = 'Delete',
  Search = 'Search'
}

// API Controller Names
export enum ApiController {
  Products = 'Products',
  Categories = 'Categories',
  Retailers = 'Retailers',
  Prices = 'Prices',
  PriceAlerts = 'PriceAlerts',
  Wishlist = 'Wishlist',
  Cart = 'Cart',
  Auth = 'Auth',
  Users = 'Users'
}

// Cookie Keys
export enum CookieKey {
  AccessToken = 'access_token',
  RefreshToken = 'refresh_token',
  UserProfile = 'user_profile',
  Language = 'app_lang'
}

// Response Status
export enum ResponseStatus {
  Success = 'Success',
  Failed = 'Failed',
  Unauthorized = 'Unauthorized',
  NotFound = 'NotFound',
  ValidationError = 'ValidationError'
}

// Sort Options
export enum SortOption {
  PriceAsc = 'price_asc',
  PriceDesc = 'price_desc',
  NameAsc = 'name_asc',
  NameDesc = 'name_desc',
  Discount = 'discount',
  Newest = 'newest',
  Rating = 'rating'
}

// Product Status
export enum ProductStatus {
  BestDeal = 'Best Deal',
  PriceDrop = 'Price Drop',
  New = 'New',
  Normal = 'Normal'
}

// Alert Status
export enum AlertStatus {
  Active = 'Active',
  Triggered = 'Triggered',
  Expired = 'Expired',
  Disabled = 'Disabled'
}

// Error Handle Location
export enum ErrorHandleLocation {
  Interceptor = 'interceptor',
  Caller = 'caller'
}
