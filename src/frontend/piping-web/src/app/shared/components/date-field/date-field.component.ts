import { booleanAttribute, Component, forwardRef, input, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-date-field',
  standalone: true,
  templateUrl: './date-field.component.html',
  styles: [':host { display: block; min-width: 0; }'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DateFieldComponent),
      multi: true,
    },
  ],
})
export class DateFieldComponent implements ControlValueAccessor {
  readonly label = input.required<string>();
  readonly required = input(false, { transform: booleanAttribute });
  readonly value = signal('');
  readonly disabled = signal(false);

  private onChange: (value: string | null) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: string | null): void {
    this.value.set(value ?? '');
  }

  registerOnChange(fn: (value: string | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  handleInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.value.set(value);
    this.onChange(value || null);
  }

  handleBlur(): void {
    this.onTouched();
  }
}
