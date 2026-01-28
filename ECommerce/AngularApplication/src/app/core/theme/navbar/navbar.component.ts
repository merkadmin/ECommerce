import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SubSink } from 'subsink';
import { GlobalService, FixedService, AuthService, Language } from '@core/services';
import { UserProfile } from '@core/models';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.less']
})
export class NavbarComponent implements OnInit, OnDestroy {
  private subs = new SubSink();

  currentUser: UserProfile | null = null;
  isAuthenticated: boolean = false;
  activeLang: Language;
  languages: Language[];

  showUserMenu: boolean = false;
  showLangMenu: boolean = false;
  showNotifications: boolean = false;
  searchQuery: string = '';

  cartCount: number = 0;
  wishlistCount: number = 0;
  alertCount: number = 0;

  constructor(
    public globalService: GlobalService,
    public fixedService: FixedService,
    private authService: AuthService,
    private router: Router
  ) {
    this.activeLang = this.globalService.activeLang;
    this.languages = this.globalService.languages;
  }

  ngOnInit(): void {
    this.subs.sink = this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });

    this.subs.sink = this.authService.isAuthenticated$.subscribe(isAuth => {
      this.isAuthenticated = isAuth;
    });

    this.subs.sink = this.globalService.appLangChanged$.subscribe(lang => {
      if (lang) {
        this.activeLang = lang;
      }
    });
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  onSearch(): void {
    if (this.searchQuery.trim()) {
      this.router.navigate(['/products'], {
        queryParams: { search: this.searchQuery }
      });
    }
  }

  changeLanguage(langCode: string): void {
    this.globalService.setLanguage(langCode);
    this.showLangMenu = false;
    // Optionally reload page for full RTL/LTR switch
    window.location.reload();
  }

  logout(): void {
    this.authService.logout();
    this.showUserMenu = false;
  }

  toggleUserMenu(): void {
    this.showUserMenu = !this.showUserMenu;
    this.showLangMenu = false;
    this.showNotifications = false;
  }

  toggleLangMenu(): void {
    this.showLangMenu = !this.showLangMenu;
    this.showUserMenu = false;
    this.showNotifications = false;
  }

  toggleNotifications(): void {
    this.showNotifications = !this.showNotifications;
    this.showUserMenu = false;
    this.showLangMenu = false;
  }

  closeAllMenus(): void {
    this.showUserMenu = false;
    this.showLangMenu = false;
    this.showNotifications = false;
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
    this.closeAllMenus();
  }
}
