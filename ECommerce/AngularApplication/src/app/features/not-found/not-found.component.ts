import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="not-found-page">
      <div class="content">
        <h1>404</h1>
        <h2>Page Not Found</h2>
        <p>The page you're looking for doesn't exist or has been moved.</p>
        <a routerLink="/dashboard" class="btn btn-primary">
          <i class="icon-home"></i>
          Go to Dashboard
        </a>
      </div>
    </div>
  `,
  styles: [`
    .not-found-page {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, #1176bd, #11b7bd);
      padding: 2rem;
    }
    .content {
      text-align: center;
      color: #fff;
    }
    h1 {
      font-size: 8rem;
      font-weight: 700;
      margin: 0;
      line-height: 1;
      opacity: 0.9;
    }
    h2 {
      font-size: 2rem;
      font-weight: 600;
      margin: 0 0 1rem;
    }
    p {
      opacity: 0.9;
      margin: 0 0 2rem;
    }
    .btn {
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      padding: 1rem 2rem;
      background: #fff;
      color: #1176bd;
      text-decoration: none;
      border-radius: 8px;
      font-weight: 500;
      transition: transform 0.2s ease;
    }
    .btn:hover {
      transform: translateY(-2px);
    }
  `]
})
export class NotFoundComponent {}
