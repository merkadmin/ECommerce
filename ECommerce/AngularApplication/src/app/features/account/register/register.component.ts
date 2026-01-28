import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService, GlobalService } from '@core/services';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="register-form">
      <h2>Create Account</h2>
      <p class="subtitle">Join us to start saving money</p>

      <form (ngSubmit)="onSubmit()">
        <div class="form-row">
          <div class="form-group">
            <label>First Name</label>
            <input type="text" class="form-control" [(ngModel)]="firstName" name="firstName" required />
          </div>
          <div class="form-group">
            <label>Last Name</label>
            <input type="text" class="form-control" [(ngModel)]="lastName" name="lastName" required />
          </div>
        </div>

        <div class="form-group">
          <label>Email</label>
          <input type="email" class="form-control" [(ngModel)]="email" name="email" required />
        </div>

        <div class="form-group">
          <label>Password</label>
          <input type="password" class="form-control" [(ngModel)]="password" name="password" required />
        </div>

        <div class="form-group">
          <label>Confirm Password</label>
          <input type="password" class="form-control" [(ngModel)]="confirmPassword" name="confirmPassword" required />
        </div>

        <button type="submit" class="btn btn-primary" [disabled]="isLoading">
          {{ isLoading ? 'Creating account...' : 'Create Account' }}
        </button>
      </form>

      <p class="login-link">
        Already have an account? <a routerLink="/login">Sign in</a>
      </p>
    </div>
  `,
  styles: [`
    .register-form {
      text-align: center;
      h2 { font-size: 1.5rem; font-weight: 700; margin: 0 0 0.5rem; }
      .subtitle { color: #6c757d; margin: 0 0 2rem; }
      form { text-align: left; }
      .form-row { display: flex; gap: 1rem; }
      .form-group { margin-bottom: 1rem; flex: 1; }
      .form-group label { display: block; font-size: 0.875rem; font-weight: 500; margin-bottom: 0.5rem; }
      .form-control { width: 100%; height: 44px; padding: 0 1rem; border: 2px solid #ddd; border-radius: 8px; }
      .form-control:focus { border-color: #1176bd; outline: none; }
      .btn { width: 100%; height: 48px; background: linear-gradient(135deg, #1176bd, #11b7bd); color: #fff; border: none; border-radius: 8px; font-weight: 500; cursor: pointer; }
      .btn:disabled { opacity: 0.7; }
      .login-link { margin-top: 1.5rem; font-size: 0.875rem; color: #6c757d; }
      .login-link a { color: #1176bd; font-weight: 500; text-decoration: none; }
    }
  `]
})
export class RegisterComponent {
  firstName = '';
  lastName = '';
  email = '';
  password = '';
  confirmPassword = '';
  isLoading = false;

  constructor(
    private authService: AuthService,
    private globalService: GlobalService,
    private router: Router
  ) {}

  onSubmit(): void {
    if (this.password !== this.confirmPassword) {
      this.globalService.notificationMessage('error', 'Passwords do not match');
      return;
    }

    this.isLoading = true;
    this.authService.register({
      email: this.email,
      password: this.password,
      firstName: this.firstName,
      lastName: this.lastName
    }).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: () => this.isLoading = false
    });
  }
}
