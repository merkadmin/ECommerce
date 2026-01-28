import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-best-deals',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="deals-page">
      <h1>Best Deals</h1>
      <p>Products with the biggest discounts</p>
      <!-- TODO: Implement best deals list -->
    </div>
  `,
  styles: [`
    .deals-page {
      h1 { font-size: 1.75rem; font-weight: 700; margin: 0 0 0.5rem; }
      p { color: #6c757d; }
    }
  `]
})
export class BestDealsComponent {}
