import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { GlobalService } from '@core/services';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  template: `<router-outlet></router-outlet>`,
  styles: []
})
export class AppComponent implements OnInit {
  constructor(
    private translate: TranslateService,
    private globalService: GlobalService
  ) {}

  ngOnInit(): void {
    // Initialize translation
    this.translate.setDefaultLang('en');
    this.translate.use(this.globalService.activeLang.code);

    // Subscribe to language changes
    this.globalService.appLangChanged$.subscribe(lang => {
      if (lang) {
        this.translate.use(lang.code);
      }
    });
  }
}
