import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-trending-products',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="trending-page">
      <h1>Trending Products</h1>
      <p>Most popular products right now</p>
      <!-- TODO: Implement trending products list -->
    </div>
  `,
  styles: [`
    .trending-page {
      h1 { font-size: 1.75rem; font-weight: 700; margin: 0 0 0.5rem; }
      p { color: #6c757d; }
    }
  `]
})
export class TrendingProductsComponent {}
