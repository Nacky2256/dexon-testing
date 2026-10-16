import { booleanAttribute, Component, forwardRef, input, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

export interface SelectOption {
  label: string;
  value: number | string;
}

@Component({
  selector: 'app-select-field',
  standalone: true,
  templateUrl: './select-field.component.html',
  styles: [':host { display: block; min-width: 0; }'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SelectFieldComponent),
      multi: true,
    },
  ],
})
export class SelectFieldComponent implements ControlValueAccessor {
  readonly label = input.required<string>();
  readonly options = input.required<readonly SelectOption[]>();
  readonly required = input(false, { transform: booleanAttribute });
  readonly value = signal<number | string | null>(null);
  readonly disabled = signal(false);

  private onChange: (value: number | string | null) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: number | string | null): void {
    this.value.set(value);
  }

  registerOnChange(fn: (value: number | string | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  serializedValue(): string {
    const value = this.value();
    return value === null ? '' : String(value);
  }

  handleChange(event: Event): void {
    const selectedValue = (event.target as HTMLSelectElement).value;
    const value =
      this.options().find((option) => String(option.value) === selectedValue)?.value ?? null;
    this.value.set(value);
    this.onChange(value);
  }

  handleBlur(): void {
    this.onTouched();
  }
}
