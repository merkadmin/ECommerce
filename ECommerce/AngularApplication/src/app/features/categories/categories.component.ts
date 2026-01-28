import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="categories-page">
      <h1>Categories</h1>
      <p>Browse products by category</p>
      <!-- TODO: Implement categories grid -->
    </div>
  `,
  styles: [`
    .categories-page {
      h1 { font-size: 1.75rem; font-weight: 700; margin: 0 0 0.5rem; }
      p { color: #6c757d; }
    }
  `]
})
export class CategoriesComponent {}
