import {
  Component,
  Input,
  Output,
  EventEmitter,
  ViewChild,
  ElementRef,
  AfterViewInit,
  forwardRef,
  OnDestroy
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { Observable, fromEvent, Subject } from 'rxjs';
import { map, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { FixedService } from '@core/services';

@Component({
  selector: 'app-cr-input',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cr-input.component.html',
  styleUrls: ['./cr-input.component.less'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CrInputComponent),
      multi: true
    }
  ]
})
export class CrInputComponent implements ControlValueAccessor, AfterViewInit, OnDestroy {
  @ViewChild('crInput') crInput!: ElementRef<HTMLInputElement>;

  @Input() placeholder: string = '';
  @Input() inputClass: string = '';
  @Input() hasClearBtn: boolean = true;
  @Input() hasInputIcon: boolean = false;
  @Input() isInvalid: boolean = false;
  @Input() loading: boolean = false;
  @Input() disabled: boolean = false;
  @Input() type: string = 'text';
  @Input() maxLength: number | null = null;
  @Input() debounceMs: number | null = null;

  @Output() keypress = new EventEmitter<KeyboardEvent>();
  @Output() onClear = new EventEmitter<void>();
  @Output() inputObserverWithTimeDelay = new EventEmitter<Observable<string>>();
  @Output() valueChange = new EventEmitter<string>();
  @Output() onEnter = new EventEmitter<string>();

  value: string = '';
  private destroy$ = new Subject<void>();
  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  constructor(private fixedService: FixedService) {}

  ngAfterViewInit(): void {
    const debounce = this.debounceMs ?? this.fixedService.waitingTimeForInstanceSearch;

    // Emit debounced input observable
    const inputObservable = fromEvent<InputEvent>(this.crInput.nativeElement, 'input').pipe(
      map((event) => (event.target as HTMLInputElement).value),
      debounceTime(debounce),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    );

    this.inputObserverWithTimeDelay.emit(inputObservable);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // ControlValueAccessor implementation
  writeValue(value: string): void {
    this.value = value || '';
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  // Event handlers
  onInputChange(): void {
    this.onChange(this.value);
    this.valueChange.emit(this.value);
  }

  onKeypress(event: KeyboardEvent): void {
    this.keypress.emit(event);

    if (event.key === 'Enter') {
      this.onEnter.emit(this.value);
    }
  }

  onBlur(): void {
    this.onTouched();
  }

  clear(): void {
    this.value = '';
    this.onChange(this.value);
    this.valueChange.emit(this.value);
    this.onClear.emit();
    this.crInput?.nativeElement?.focus();
  }

  focus(): void {
    this.crInput?.nativeElement?.focus();
  }
}
