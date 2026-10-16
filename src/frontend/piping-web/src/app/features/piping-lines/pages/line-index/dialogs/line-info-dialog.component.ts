import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { PipingLineDetail } from '../../../../../models';

@Component({
  selector: 'app-line-info-dialog',
  standalone: true,
  templateUrl: './line-info-dialog.component.html',
  styleUrl: './line-info-dialog.component.css',
})
export class LineInfoDialogComponent {
  @Input() line: PipingLineDetail | null = null;
  @Input() loading = false;
  @Input() errorMessage = '';
  @Input() deleting = false;

  @Output() closed = new EventEmitter<void>();
  @Output() editLine = new EventEmitter<PipingLineDetail>();
  @Output() deleteLine = new EventEmitter<void>();

  @ViewChild('dialog') private dialogRef?: ElementRef<HTMLDialogElement>;

  open(): void {
    this.dialogRef?.nativeElement.showModal();
  }

  close(): void {
    this.dialogRef?.nativeElement.close();
  }

  onClose(): void {
    this.closed.emit();
  }

  requestEdit(): void {
    if (this.line) this.editLine.emit(this.line);
  }

  requestDelete(): void {
    this.deleteLine.emit();
  }

  testPointCount(line: PipingLineDetail): number {
    return line.cmls.reduce((count, cml) => count + cml.testPoints.length, 0);
  }
}
