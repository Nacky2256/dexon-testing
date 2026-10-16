import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PipingLineWrite } from '../../../../../models';
import { DateFieldComponent } from '../../../../../shared/components/date-field/date-field.component';
import { NumberFieldComponent } from '../../../../../shared/components/number-field/number-field.component';
import { TextFieldComponent } from '../../../../../shared/components/text-field/text-field.component';

@Component({
  selector: 'app-line-form-dialog',
  standalone: true,
  imports: [FormsModule, DateFieldComponent, NumberFieldComponent, TextFieldComponent],
  templateUrl: './line-form-dialog.component.html',
  styleUrl: './line-form-dialog.component.css',
})
export class LineFormDialogComponent {
  @Input() draft!: PipingLineWrite;
  @Input() editing = false;
  @Input() saving = false;
  @Input() errorMessage = '';

  @Output() draftChange = new EventEmitter<PipingLineWrite>();
  @Output() saveRequested = new EventEmitter<void>();
  @Output() closed = new EventEmitter<void>();

  @ViewChild('dialog') private dialogRef?: ElementRef<HTMLDialogElement>;

  open(): void {
    this.dialogRef?.nativeElement.showModal();
  }

  close(): void {
    this.dialogRef?.nativeElement.close();
  }

  updateDraft<K extends keyof PipingLineWrite>(key: K, value: PipingLineWrite[K]): void {
    this.draftChange.emit({ ...this.draft, [key]: value });
  }

  onSave(): void {
    this.saveRequested.emit();
  }
}
