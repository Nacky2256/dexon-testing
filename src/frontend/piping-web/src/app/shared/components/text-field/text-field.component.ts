import { booleanAttribute, Component, forwardRef, input, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-text-field',
  standalone: true,
  templateUrl: './text-field.component.html',
  styles: [':host { display: block; min-width: 0; }'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TextFieldComponent),
      multi: true,
    },
  ],
})
export class TextFieldComponent implements ControlValueAccessor {
  readonly label = input.required<string>();
  readonly placeholder = input('');
  readonly required = input(false, { transform: booleanAttribute });
  readonly maxLength = input<number | null, number | string | null>(null, {
    alias: 'maxlength',
    transform: (value) => (value === null || value === '' ? null : Number(value)),
  });

  readonly value = signal('');
  readonly disabled = signal(false);

  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  writeValue(value: string | null): void {
    this.value.set(value ?? '');
  }

  registerOnChange(fn: (value: string) => void): void {
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
    this.onChange(value);
  }

  handleBlur(): void {
    this.onTouched();
  }
}
