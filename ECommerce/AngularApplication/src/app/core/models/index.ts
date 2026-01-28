// Product Models
export interface Product {
  id: string;
  name: string;
  description: string;
  brand: string;
  sku: string;
  imageUrl: string;
  categoryId: string;
  categoryName: string;
  lowestPrice: number;
  highestPrice: number;
  averagePrice: number;
  retailerCount: number;
  status: 'Best Deal' | 'Price Drop' | 'New' | 'Normal';
  createdAt: Date;
  updatedAt: Date;
}

export interface ProductDetail extends Product {
  prices: RetailerPrice[];
  priceHistory: PriceHistory[];
  specifications: ProductSpecification[];
}

export interface ProductSpecification {
  name: string;
  value: string;
}

// Price Models
export interface RetailerPrice {
  id: string;
  retailerId: string;
  retailerName: string;
  retailerLogo: string;
  currentPrice: number;
  originalPrice: number | null;
  discountPercent: number | null;
  productUrl: string;
  isAvailable: boolean;
  shippingCost: number | null;
  rating: number | null;
  reviewCount: number | null;
  lastUpdated: Date;
}

export interface PriceHistory {
  date: Date;
  price: number;
  retailerId: string;
  retailerName: string;
}

export interface PriceComparison {
  productId: string;
  productName: string;
  lowestPrice: number;
  highestPrice: number;
  averagePrice: number;
  pricesByRetailer: RetailerPrice[];
}

// Category Models
export interface Category {
  id: string;
  name: string;
  icon: string;
  parentCategoryId: string | null;
  productCount: number;
  subCategories: Category[];
}

// Retailer Models
export interface Retailer {
  id: string;
  name: string;
  logoUrl: string;
  baseUrl: string;
  averageRating: number;
  productCount: number;
  isActive: boolean;
}

// Price Alert Models
export interface PriceAlert {
  id: string;
  productId: string;
  productName: string;
  productImage: string;
  targetPrice: number;
  currentLowestPrice: number;
  isActive: boolean;
  isTriggered: boolean;
  createdAt: Date;
  triggeredAt: Date | null;
}

export interface CreatePriceAlertDto {
  productId: string;
  targetPrice: number;
}

// Wishlist & Cart Models
export interface WishlistItem {
  id: string;
  productId: string;
  product: Product;
  addedAt: Date;
}

export interface CartItem {
  id: string;
  productId: string;
  product: Product;
  selectedRetailerId: string;
  selectedRetailer: RetailerPrice;
  quantity: number;
}

// User Models
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  avatarUrl: string | null;
}

export interface UserProfile extends User {
  alertCount: number;
  wishlistCount: number;
}

// Auth Models
export interface LoginRequest {
  email: string;
  password: string;
  rememberMe: boolean;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  user: UserProfile;
  expiresIn: number;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

// API Response Models
export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string | null;
  errors: string[] | null;
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// Filter & Pagination Models
export interface ProductFilter {
  search?: string;
  categoryId?: string;
  retailerIds?: string[];
  minPrice?: number;
  maxPrice?: number;
  sortBy?: 'price_asc' | 'price_desc' | 'name_asc' | 'name_desc' | 'discount' | 'newest';
  page: number;
  pageSize: number;
}

export interface PaginationModel {
  currentPage: number;
  lastPage: number;
  recordPerPage: number;
  total: number;
  list: number[];
  from: number;
  to: number;
}

// Config Model
export interface AppConfig {
  apiBaseUrl: string;
  isProduction: boolean;
  enableAnalytics: boolean;
  defaultLanguage: string;
  supportedLanguages: string[];
}
