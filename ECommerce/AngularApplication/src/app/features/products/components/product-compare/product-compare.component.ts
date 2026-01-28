import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-compare',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="compare-page">
      <h1>Compare Products</h1>
      <p>Compare specifications and prices side by side</p>
      <!-- TODO: Implement product comparison table -->
    </div>
  `,
  styles: [`
    .compare-page {
      h1 { font-size: 1.75rem; font-weight: 700; margin: 0 0 0.5rem; }
      p { color: #6c757d; }
    }
  `]
})
export class ProductCompareComponent {}
