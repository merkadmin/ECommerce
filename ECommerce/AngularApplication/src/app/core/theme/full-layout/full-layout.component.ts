import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SubSink } from 'subsink';
import { NavbarComponent } from '../navbar/navbar.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { FixedService, GlobalService } from '@core/services';

@Component({
  selector: 'app-full-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, NavbarComponent, SidebarComponent],
  templateUrl: './full-layout.component.html',
  styleUrls: ['./full-layout.component.less']
})
export class FullLayoutComponent implements OnInit, OnDestroy {
  private subs = new SubSink();

  showNavBar: boolean = true;
  showSideBar: boolean = true;
  isLoading: boolean = false;

  constructor(
    public fixedService: FixedService,
    private globalService: GlobalService
  ) {}

  ngOnInit(): void {
    this.showNavBar = this.fixedService.showNavBar;
    this.showSideBar = this.fixedService.showSideBar;

    // Subscribe to loading state
    this.subs.sink = this.globalService.loading$.subscribe(loading => {
      this.isLoading = loading;
    });
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }
}
