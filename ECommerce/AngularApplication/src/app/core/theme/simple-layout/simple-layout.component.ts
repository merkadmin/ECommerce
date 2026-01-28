import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-simple-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './simple-layout.component.html',
  styleUrls: ['./simple-layout.component.less']
})
export class SimpleLayoutComponent {}
