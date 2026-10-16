import { booleanAttribute, Component, forwardRef, input, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

function nullableNumber(value: number | string | null): number | null {
  return value === null || value === '' ? null : Number(value);
}

@Component({
  selector: 'app-number-field',
  standalone: true,
  templateUrl: './number-field.component.html',
  styles: [':host { display: block; min-width: 0; }'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => NumberFieldComponent),
      multi: true,
    },
  ],
})
export class NumberFieldComponent implements ControlValueAccessor {
  readonly label = input.required<string>();
  readonly unit = input('');
  readonly required = input(false, { transform: booleanAttribute });
  readonly min = input<number | null, number | string | null>(null, {
    alias: 'min',
    transform: nullableNumber,
  });
  readonly max = input<number | null, number | string | null>(null, {
    alias: 'max',
    transform: nullableNumber,
  });
  readonly step = input('any');
  readonly value = signal<number | null>(null);
  readonly disabled = signal(false);

  private onChange: (value: number | null) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: number | null): void {
    this.value.set(value);
  }

  registerOnChange(fn: (value: number | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  handleInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const value =
      input.value === '' || Number.isNaN(input.valueAsNumber) ? null : input.valueAsNumber;
    this.value.set(value);
    this.onChange(value);
  }

  handleBlur(): void {
    this.onTouched();
  }
}
