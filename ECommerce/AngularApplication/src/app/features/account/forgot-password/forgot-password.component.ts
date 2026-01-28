import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService, GlobalService } from '@core/services';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="forgot-password-form">
      <h2>Reset Password</h2>
      <p class="subtitle">Enter your email to receive reset instructions</p>

      <form (ngSubmit)="onSubmit()" *ngIf="!emailSent">
        <div class="form-group">
          <label>Email</label>
          <input type="email" class="form-control" [(ngModel)]="email" name="email" placeholder="Enter your email" required />
        </div>

        <button type="submit" class="btn btn-primary" [disabled]="isLoading">
          {{ isLoading ? 'Sending...' : 'Send Reset Link' }}
        </button>
      </form>

      <div class="success-message" *ngIf="emailSent">
        <i class="icon-check-circle"></i>
        <h3>Check your email</h3>
        <p>We've sent password reset instructions to {{ email }}</p>
      </div>

      <p class="back-link">
        <a routerLink="/login">← Back to login</a>
      </p>
    </div>
  `,
  styles: [`
    .forgot-password-form {
      text-align: center;
      h2 { font-size: 1.5rem; font-weight: 700; margin: 0 0 0.5rem; }
      .subtitle { color: #6c757d; margin: 0 0 2rem; }
      form { text-align: left; }
      .form-group { margin-bottom: 1.5rem; }
      .form-group label { display: block; font-size: 0.875rem; font-weight: 500; margin-bottom: 0.5rem; }
      .form-control { width: 100%; height: 44px; padding: 0 1rem; border: 2px solid #ddd; border-radius: 8px; }
      .form-control:focus { border-color: #1176bd; outline: none; }
      .btn { width: 100%; height: 48px; background: linear-gradient(135deg, #1176bd, #11b7bd); color: #fff; border: none; border-radius: 8px; font-weight: 500; cursor: pointer; }
      .success-message { padding: 2rem 0; }
      .success-message i { font-size: 3rem; color: #17dd8a; margin-bottom: 1rem; }
      .success-message h3 { margin: 0 0 0.5rem; }
      .success-message p { color: #6c757d; }
      .back-link { margin-top: 1.5rem; }
      .back-link a { color: #1176bd; text-decoration: none; }
    }
  `]
})
export class ForgotPasswordComponent {
  email = '';
  isLoading = false;
  emailSent = false;

  constructor(
    private authService: AuthService,
    private globalService: GlobalService
  ) {}

  onSubmit(): void {
    this.isLoading = true;
    this.authService.forgotPassword(this.email).subscribe({
      next: () => {
        this.emailSent = true;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }
}
