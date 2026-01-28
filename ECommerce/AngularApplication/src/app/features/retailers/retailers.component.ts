import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-retailers',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="retailers-page">
      <h1>Retailers</h1>
      <p>View all partnered retailers</p>
      <!-- TODO: Implement retailers grid -->
    </div>
  `,
  styles: [`
    .retailers-page {
      h1 { font-size: 1.75rem; font-weight: 700; margin: 0 0 0.5rem; }
      p { color: #6c757d; }
    }
  `]
})
export class RetailersComponent {}
