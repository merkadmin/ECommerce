import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { map, tap, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ConfigService } from './config.service';
import { GlobalService } from './global.service';
import {
  User,
  UserProfile,
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  ApiResponse
} from '../models';
import { CookieKey } from '../enums';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<UserProfile | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(
    private http: HttpClient,
    private configService: ConfigService,
    private globalService: GlobalService,
    private router: Router
  ) {
    this.checkStoredAuth();
  }

  private get baseUrl(): string {
    return `${this.configService.apiBaseUrl}/Auth`;
  }

  private checkStoredAuth(): void {
    const token = this.getAccessToken();
    const userJson = localStorage.getItem(CookieKey.UserProfile);

    if (token && userJson) {
      try {
        const user = JSON.parse(userJson) as UserProfile;
        this.currentUserSubject.next(user);
        this.isAuthenticatedSubject.next(true);
      } catch {
        this.clearAuth();
      }
    }
  }

  get currentUser(): UserProfile | null {
    return this.currentUserSubject.value;
  }

  get isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  getAccessToken(): string | null {
    return localStorage.getItem(CookieKey.AccessToken);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(CookieKey.RefreshToken);
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.baseUrl}/login`, request).pipe(
      map(response => response.data),
      tap(response => {
        this.storeAuth(response);
        this.currentUserSubject.next(response.user);
        this.isAuthenticatedSubject.next(true);
      }),
      catchError(error => {
        this.globalService.notificationMessage('error', 'Login failed. Please check your credentials.');
        return throwError(() => error);
      })
    );
  }

  register(request: RegisterRequest): Observable<LoginResponse> {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.baseUrl}/register`, request).pipe(
      map(response => response.data),
      tap(response => {
        this.storeAuth(response);
        this.currentUserSubject.next(response.user);
        this.isAuthenticatedSubject.next(true);
        this.globalService.notificationMessage('success', 'Registration successful!');
      }),
      catchError(error => {
        this.globalService.notificationMessage('error', 'Registration failed. Please try again.');
        return throwError(() => error);
      })
    );
  }

  refreshToken(): Observable<LoginResponse> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      return throwError(() => new Error('No refresh token'));
    }

    return this.http.post<ApiResponse<LoginResponse>>(`${this.baseUrl}/refresh`, { refreshToken }).pipe(
      map(response => response.data),
      tap(response => {
        this.storeAuth(response);
        this.currentUserSubject.next(response.user);
      }),
      catchError(error => {
        this.logout();
        return throwError(() => error);
      })
    );
  }

  logout(): void {
    this.clearAuth();
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/login']);
  }

  private storeAuth(response: LoginResponse): void {
    localStorage.setItem(CookieKey.AccessToken, response.accessToken);
    localStorage.setItem(CookieKey.RefreshToken, response.refreshToken);
    localStorage.setItem(CookieKey.UserProfile, JSON.stringify(response.user));
  }

  private clearAuth(): void {
    localStorage.removeItem(CookieKey.AccessToken);
    localStorage.removeItem(CookieKey.RefreshToken);
    localStorage.removeItem(CookieKey.UserProfile);
  }

  updateProfile(profile: Partial<UserProfile>): Observable<UserProfile> {
    return this.http.put<ApiResponse<UserProfile>>(`${this.configService.apiBaseUrl}/Users/profile`, profile).pipe(
      map(response => response.data),
      tap(user => {
        const currentUser = this.currentUserSubject.value;
        if (currentUser) {
          const updatedUser = { ...currentUser, ...user };
          this.currentUserSubject.next(updatedUser);
          localStorage.setItem(CookieKey.UserProfile, JSON.stringify(updatedUser));
        }
      })
    );
  }

  changePassword(currentPassword: string, newPassword: string): Observable<boolean> {
    return this.http.post<ApiResponse<boolean>>(`${this.baseUrl}/change-password`, {
      currentPassword,
      newPassword
    }).pipe(
      map(response => response.data),
      tap(() => {
        this.globalService.notificationMessage('success', 'Password changed successfully');
      })
    );
  }

  forgotPassword(email: string): Observable<boolean> {
    return this.http.post<ApiResponse<boolean>>(`${this.baseUrl}/forgot-password`, { email }).pipe(
      map(response => response.data),
      tap(() => {
        this.globalService.notificationMessage('info', 'Password reset email sent');
      })
    );
  }

  resetPassword(token: string, newPassword: string): Observable<boolean> {
    return this.http.post<ApiResponse<boolean>>(`${this.baseUrl}/reset-password`, {
      token,
      newPassword
    }).pipe(
      map(response => response.data)
    );
  }
}
