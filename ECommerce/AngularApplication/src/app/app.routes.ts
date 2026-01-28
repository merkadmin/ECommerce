import { Routes } from '@angular/router';
import { AuthGuard, GuestGuard } from '@core/guards/auth.guard';

export const routes: Routes = [
  // Full Layout Routes (with navbar & sidebar)
  {
    path: '',
    loadComponent: () => import('@core/theme/full-layout/full-layout.component')
      .then(m => m.FullLayoutComponent),
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component')
          .then(m => m.DashboardComponent),
        title: 'Dashboard - PriceCompare'
      },
      {
        path: 'products',
        loadChildren: () => import('./features/products/products.routes')
          .then(m => m.PRODUCTS_ROUTES)
      },
      {
        path: 'categories',
        loadComponent: () => import('./features/categories/categories.component')
          .then(m => m.CategoriesComponent),
        title: 'Categories - PriceCompare'
      },
      {
        path: 'retailers',
        loadComponent: () => import('./features/retailers/retailers.component')
          .then(m => m.RetailersComponent),
        title: 'Retailers - PriceCompare'
      },
      {
        path: 'alerts',
        loadComponent: () => import('./features/price-alerts/price-alerts.component')
          .then(m => m.PriceAlertsComponent),
        canActivate: [AuthGuard],
        title: 'Price Alerts - PriceCompare'
      },
      {
        path: 'wishlist',
        loadComponent: () => import('./features/wishlist/wishlist.component')
          .then(m => m.WishlistComponent),
        title: 'Wishlist - PriceCompare'
      },
      {
        path: 'cart',
        loadComponent: () => import('./features/cart/cart.component')
          .then(m => m.CartComponent),
        title: 'Cart - PriceCompare'
      }
    ]
  },
  // Simple Layout Routes (auth pages)
  {
    path: '',
    loadComponent: () => import('@core/theme/simple-layout/simple-layout.component')
      .then(m => m.SimpleLayoutComponent),
    children: [
      {
        path: 'login',
        loadComponent: () => import('./features/account/login/login.component')
          .then(m => m.LoginComponent),
        canActivate: [GuestGuard],
        title: 'Login - PriceCompare'
      },
      {
        path: 'register',
        loadComponent: () => import('./features/account/register/register.component')
          .then(m => m.RegisterComponent),
        canActivate: [GuestGuard],
        title: 'Register - PriceCompare'
      },
      {
        path: 'forgot-password',
        loadComponent: () => import('./features/account/forgot-password/forgot-password.component')
          .then(m => m.ForgotPasswordComponent),
        title: 'Forgot Password - PriceCompare'
      }
    ]
  },
  // 404 Page
  {
    path: '**',
    loadComponent: () => import('./features/not-found/not-found.component')
      .then(m => m.NotFoundComponent),
    title: 'Page Not Found - PriceCompare'
  }
];
