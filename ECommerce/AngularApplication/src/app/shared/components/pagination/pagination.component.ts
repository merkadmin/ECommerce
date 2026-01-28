import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaginationModel } from '@core/models';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.less'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PaginationComponent implements OnChanges {
  @Input() total: number = 0;
  @Input() recordPerPage: number = 20;
  @Input() page: number = 1;
  @Input() pageSizeOptions: number[] = [10, 20, 50, 100];
  @Input() showPageSizeSelector: boolean = true;
  @Input() showPageInfo: boolean = true;

  @Output() changePagination = new EventEmitter<PaginationModel>();

  pagination: PaginationModel = {
    currentPage: 1,
    lastPage: 1,
    recordPerPage: 20,
    total: 0,
    list: [],
    from: 0,
    to: 0
  };

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['total'] || changes['recordPerPage'] || changes['page']) {
      this.calculatePagination();
    }
  }

  private calculatePagination(): void {
    this.pagination.total = this.total;
    this.pagination.recordPerPage = this.recordPerPage;
    this.pagination.currentPage = this.page;
    this.pagination.lastPage = Math.ceil(this.total / this.recordPerPage) || 1;

    // Calculate from/to
    this.pagination.from = this.total > 0 ? (this.page - 1) * this.recordPerPage + 1 : 0;
    this.pagination.to = Math.min(this.page * this.recordPerPage, this.total);

    // Generate page list (window of 5 pages around current)
    this.generatePageList();
  }

  private generatePageList(): void {
    const list: number[] = [];
    const current = this.pagination.currentPage;
    const last = this.pagination.lastPage;

    let start = Math.max(1, current - 2);
    let end = Math.min(last, current + 2);

    // Adjust window if at edges
    if (current <= 2) {
      end = Math.min(5, last);
    }
    if (current >= last - 1) {
      start = Math.max(1, last - 4);
    }

    for (let i = start; i <= end; i++) {
      list.push(i);
    }

    this.pagination.list = list;
  }

  change(type: 'first' | 'previous' | 'next' | 'last', page?: number): void {
    let newPage = this.pagination.currentPage;

    switch (type) {
      case 'first':
        newPage = 1;
        break;
      case 'previous':
        newPage = Math.max(1, this.pagination.currentPage - 1);
        break;
      case 'next':
        newPage = Math.min(this.pagination.lastPage, this.pagination.currentPage + 1);
        break;
      case 'last':
        newPage = this.pagination.lastPage;
        break;
    }

    if (page !== undefined) {
      newPage = page;
    }

    if (newPage !== this.pagination.currentPage) {
      this.pagination.currentPage = newPage;
      this.calculatePagination();
      this.changePagination.emit({ ...this.pagination });
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.pagination.lastPage && page !== this.pagination.currentPage) {
      this.change('first', page);
    }
  }

  onPageSizeChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const newSize = parseInt(select.value, 10);

    this.pagination.recordPerPage = newSize;
    this.pagination.currentPage = 1; // Reset to first page
    this.calculatePagination();
    this.changePagination.emit({ ...this.pagination });
  }
}
