import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-price-alerts',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="price-alerts-page">
      <h1>Price Alerts</h1>
      <p>Manage your price alerts and get notified when prices drop</p>
      <!-- TODO: Implement price alerts list -->
    </div>
  `,
  styles: [`
    .price-alerts-page {
      h1 { font-size: 1.75rem; font-weight: 700; margin: 0 0 0.5rem; }
      p { color: #6c757d; }
    }
  `]
})
export class PriceAlertsComponent {}
