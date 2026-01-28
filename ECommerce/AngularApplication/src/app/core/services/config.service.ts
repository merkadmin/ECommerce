import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { AppConfig } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private config: AppConfig | null = null;

  constructor(private http: HttpClient) {}

  async loadConfig(): Promise<void> {
    try {
      this.config = await firstValueFrom(
        this.http.get<AppConfig>('/config/config.json')
      );
    } catch (error) {
      console.error('Failed to load configuration', error);
      // Use default config
      this.config = {
        apiBaseUrl: 'http://localhost:5000/api',
        isProduction: false,
        enableAnalytics: false,
        defaultLanguage: 'en',
        supportedLanguages: ['en', 'ar']
      };
    }
  }

  get apiBaseUrl(): string {
    return this.config?.apiBaseUrl || 'http://localhost:5000/api';
  }

  get isProduction(): boolean {
    return this.config?.isProduction || false;
  }

  get enableAnalytics(): boolean {
    return this.config?.enableAnalytics || false;
  }

  get defaultLanguage(): string {
    return this.config?.defaultLanguage || 'en';
  }

  get supportedLanguages(): string[] {
    return this.config?.supportedLanguages || ['en', 'ar'];
  }

  getConfig(): AppConfig | null {
    return this.config;
  }
}
