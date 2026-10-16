import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal, ViewChild } from '@angular/core';
import { Observable } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PipingApiService } from '../../../../piping-api.service';
import { PipingLineDetail, PipingLineSummary, PipingLineWrite } from '../../../../models';
import { toUserMessage } from '../../../../http-error';
import { LineFormDialogComponent } from './dialogs/line-form-dialog.component';
import { LineInfoDialogComponent } from './dialogs/line-info-dialog.component';

const emptyDraft = (): PipingLineWrite => ({
  lineNumber: '',
  location: null,
  fromDescription: null,
  toDescription: null,
  drawingNumber: null,
  service: null,
  material: null,
  inserviceDate: null,
  pipeSizeInch: null,
  originalThicknessMm: null,
  stress: null,
  jointEfficiency: null,
  corrosionAllowanceMm: null,
  designLife: null,
  designPressure: null,
  operatingPressure: null,
  designTemperature: null,
  operatingTemperature: null,
});

@Component({
  selector: 'app-line-index',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    LineFormDialogComponent,
    LineInfoDialogComponent,
  ],
  templateUrl: './line-index.component.html',
  styleUrl: './line-index.component.css',
})
export class LineIndexComponent implements OnInit {
  private readonly api = inject(PipingApiService);
  @ViewChild(LineInfoDialogComponent) private infoDialog?: LineInfoDialogComponent;
  @ViewChild(LineFormDialogComponent) private formDialog?: LineFormDialogComponent;
  readonly lines = signal<PipingLineSummary[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly editingId = signal<number | null>(null);
  readonly draft = signal(emptyDraft());
  readonly errorMessage = signal('');
  readonly search = signal('');
  readonly selectedLine = signal<PipingLineDetail | null>(null);
  readonly infoLoading = signal(false);
  readonly infoError = signal('');
  readonly deleting = signal(false);
  readonly filteredLines = computed(() => {
    const q = this.search().trim().toLowerCase();
    const lines = this.lines();
    return q
      ? lines.filter((x) =>
          [x.lineNumber, x.location, x.service, x.fromDescription, x.toDescription].some((value) =>
            value?.toLowerCase().includes(q),
          ),
        )
      : lines;
  });

  ngOnInit() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.api.listLines().subscribe({
      next: (rows) => {
        this.lines.set(rows);
        this.loading.set(false);
      },
      error: (error) => {
        this.errorMessage.set(toUserMessage(error));
        this.loading.set(false);
      },
    });
  }

  create(): void {
    this.editingId.set(null);
    this.draft.set(emptyDraft());
    this.errorMessage.set('');
    this.formDialog?.open();
  }

  openInfo(line: PipingLineSummary): void {
    this.selectedLine.set(null);
    this.infoLoading.set(true);
    this.infoError.set('');
    this.infoDialog?.open();
    this.api.getLine(line.id).subscribe({
      next: (detail) => {
        this.selectedLine.set(detail);
        this.infoLoading.set(false);
      },
      error: (error) => {
        this.infoError.set(toUserMessage(error));
        this.infoLoading.set(false);
      },
    });
  }

  onInfoClosed(): void {
    this.selectedLine.set(null);
    this.infoLoading.set(false);
    this.infoError.set('');
    this.deleting.set(false);
  }

  editSelectedLine(detail: PipingLineDetail): void {
    this.editingId.set(detail.id);
    this.draft.set(this.toWrite(detail));
    this.errorMessage.set('');
    this.infoDialog?.close();
    this.formDialog?.open();
  }

  onLineFormClosed(): void {
    this.editingId.set(null);
    this.draft.set(emptyDraft());
  }

  setDraft(draft: PipingLineWrite): void {
    this.draft.set(draft);
  }

  save(): void {
    const draft = this.draft();
    const editingId = this.editingId();
    if (!draft.lineNumber.trim() || draft.pipeSizeInch === null) return;
    this.saving.set(true);
    this.errorMessage.set('');
    const request: Observable<unknown> =
      editingId === null ? this.api.createLine(draft) : this.api.updateLine(editingId, draft);
    request.subscribe({
      next: () => {
        this.saving.set(false);
        this.formDialog?.close();
        this.load();
      },
      error: (error) => {
        this.saving.set(false);
        this.errorMessage.set(toUserMessage(error));
      },
    });
  }

  removeSelectedLine(): void {
    const line = this.selectedLine();
    if (!line) return;
    if (!window.confirm(`ลบ Line Number ${line.lineNumber}?`)) return;
    this.deleting.set(true);
    this.infoError.set('');
    this.api.deleteLine(line.id).subscribe({
      next: () => {
        this.deleting.set(false);
        this.infoDialog?.close();
        this.load();
      },
      error: (error) => {
        this.deleting.set(false);
        this.infoError.set(toUserMessage(error));
      },
    });
  }

  private toWrite(value: PipingLineDetail): PipingLineWrite {
    const { id: _id, cmls: _cmls, ...write } = value;
    return write;
  }
}
