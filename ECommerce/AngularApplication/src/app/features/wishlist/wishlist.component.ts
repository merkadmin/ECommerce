import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-wishlist',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="wishlist-page">
      <h1>My Wishlist</h1>
      <p>Products you've saved for later</p>
      <!-- TODO: Implement wishlist grid -->
    </div>
  `,
  styles: [`
    .wishlist-page {
      h1 { font-size: 1.75rem; font-weight: 700; margin: 0 0 0.5rem; }
      p { color: #6c757d; }
    }
  `]
})
export class WishlistComponent {}
