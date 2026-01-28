import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { SubSink } from 'subsink';
import { filter } from 'rxjs/operators';
import { FixedService, SidebarItem } from '@core/services';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.less']
})
export class SidebarComponent implements OnInit, OnDestroy {
  private subs = new SubSink();

  sidebarItems: SidebarItem[] = [];
  activeUrl: string = '';
  expandedItems: Set<string> = new Set();

  constructor(
    public fixedService: FixedService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Subscribe to sidebar items
    this.subs.sink = this.fixedService.sidebarItems$.subscribe(items => {
      this.sidebarItems = items;
    });

    // Track current URL
    this.activeUrl = this.router.url.split('?')[0];

    this.subs.sink = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.activeUrl = event.urlAfterRedirects.split('?')[0];
        this.expandParentOfActiveItem();
      });

    // Expand parent of initial active item
    this.expandParentOfActiveItem();
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  private expandParentOfActiveItem(): void {
    this.sidebarItems.forEach(item => {
      if (item.children?.some(child => this.isActive(child))) {
        this.expandedItems.add(item.id);
      }
    });
  }

  isActive(item: SidebarItem): boolean {
    if (item.relativeURL === this.activeUrl) {
      return true;
    }
    if (item.activeUrl && this.activeUrl.startsWith(item.activeUrl)) {
      return true;
    }
    return false;
  }

  isExpanded(item: SidebarItem): boolean {
    return this.expandedItems.has(item.id);
  }

  toggleItem(item: SidebarItem, event: Event): void {
    event.preventDefault();
    event.stopPropagation();

    if (this.expandedItems.has(item.id)) {
      this.expandedItems.delete(item.id);
    } else {
      this.expandedItems.add(item.id);
    }
  }

  navigateTo(url: string | undefined): void {
    if (url) {
      this.router.navigate([url]);
    }
  }
}
