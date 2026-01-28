import { Injectable, EventEmitter } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import Swal from 'sweetalert2';
import numeral from 'numeral';
import moment from 'moment';

export interface Language {
  code: string;
  name: string;
  icon: string;
  dir: 'ltr' | 'rtl';
}

@Injectable({
  providedIn: 'root'
})
export class GlobalService {
  // Observable state
  appLangChanged$ = new BehaviorSubject<Language | null>(null);
  loading$ = new BehaviorSubject<boolean>(false);
  cartUpdated$ = new EventEmitter<void>();
  wishlistUpdated$ = new EventEmitter<void>();

  // Current language
  private currentLanguage: Language = {
    code: 'en',
    name: 'English',
    icon: 'flag-us',
    dir: 'ltr'
  };

  // Available languages
  readonly languages: Language[] = [
    { code: 'en', name: 'English', icon: 'flag-us', dir: 'ltr' },
    { code: 'ar', name: 'العربية', icon: 'flag-sa', dir: 'rtl' }
  ];

  constructor(private toastr: ToastrService) {
    this.initializeLanguage();
  }

  private initializeLanguage(): void {
    const savedLang = localStorage.getItem('app_lang');
    if (savedLang) {
      const lang = this.languages.find(l => l.code === savedLang);
      if (lang) {
        this.currentLanguage = lang;
      }
    }
    this.applyLanguageSettings();
  }

  private applyLanguageSettings(): void {
    document.documentElement.lang = this.currentLanguage.code;
    document.documentElement.dir = this.currentLanguage.dir;
    document.body.classList.remove('english', 'arabic');
    document.body.classList.add(this.currentLanguage.code === 'ar' ? 'arabic' : 'english');
  }

  // Language methods
  get activeLang(): Language {
    return this.currentLanguage;
  }

  setLanguage(langCode: string): void {
    const lang = this.languages.find(l => l.code === langCode);
    if (lang) {
      this.currentLanguage = lang;
      localStorage.setItem('app_lang', langCode);
      this.applyLanguageSettings();
      this.appLangChanged$.next(lang);
    }
  }

  // Loading state
  showLoading(): void {
    this.loading$.next(true);
  }

  hideLoading(): void {
    this.loading$.next(false);
  }

  // Notifications
  notificationMessage(
    type: 'success' | 'error' | 'warning' | 'info',
    message: string,
    title?: string
  ): void {
    const options = {
      timeOut: 3000,
      positionClass: 'toast-top-right',
      closeButton: true,
      progressBar: true
    };

    switch (type) {
      case 'success':
        this.toastr.success(message, title || 'Success', options);
        break;
      case 'error':
        this.toastr.error(message, title || 'Error', options);
        break;
      case 'warning':
        this.toastr.warning(message, title || 'Warning', options);
        break;
      case 'info':
        this.toastr.info(message, title || 'Info', options);
        break;
    }
  }

  // Sweet Alert dialogs
  async confirmDialog(
    title: string,
    text: string,
    confirmText: string = 'Yes',
    cancelText: string = 'Cancel'
  ): Promise<boolean> {
    const result = await Swal.fire({
      title,
      text,
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#1176bd',
      cancelButtonColor: '#dd0a35',
      confirmButtonText: confirmText,
      cancelButtonText: cancelText
    });
    return result.isConfirmed;
  }

  async successDialog(title: string, text: string): Promise<void> {
    await Swal.fire({
      title,
      text,
      icon: 'success',
      confirmButtonColor: '#1176bd'
    });
  }

  async errorDialog(title: string, text: string): Promise<void> {
    await Swal.fire({
      title,
      text,
      icon: 'error',
      confirmButtonColor: '#1176bd'
    });
  }

  // Formatting utilities
  formatMoney(value: number, currency: string = '$'): string {
    return `${currency}${numeral(value).format('0,0.00')}`;
  }

  formatNumber(value: number): string {
    return numeral(value).format('0,0');
  }

  formatPercent(value: number): string {
    return `${numeral(value).format('0.0')}%`;
  }

  formatDate(date: Date | string, format: string = 'MMM DD, YYYY'): string {
    return moment(date).format(format);
  }

  formatDateTime(date: Date | string): string {
    return moment(date).format('MMM DD, YYYY HH:mm');
  }

  formatRelativeTime(date: Date | string): string {
    return moment(date).fromNow();
  }

  // Utility methods
  generateUUID(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
      const r = Math.random() * 16 | 0;
      const v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }

  copyToClipboard(text: string): void {
    navigator.clipboard.writeText(text).then(() => {
      this.notificationMessage('success', 'Copied to clipboard');
    });
  }

  // Storage helpers
  setLocalStorage(key: string, value: any): void {
    localStorage.setItem(key, JSON.stringify(value));
  }

  getLocalStorage<T>(key: string): T | null {
    const item = localStorage.getItem(key);
    return item ? JSON.parse(item) : null;
  }

  removeLocalStorage(key: string): void {
    localStorage.removeItem(key);
  }
}
