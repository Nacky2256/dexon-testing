import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PipingApiService } from '../../../../piping-api.service';
import {
  Cml,
  CmlWrite,
  PipingLineDetail,
  TestPoint,
  TestPointWrite,
  ThicknessReading,
  ThicknessReadingWrite,
} from '../../../../models';
import { toUserMessage } from '../../../../http-error';
import { DateFieldComponent } from '../../../../shared/components/date-field/date-field.component';
import { NumberFieldComponent } from '../../../../shared/components/number-field/number-field.component';
import {
  SelectFieldComponent,
  SelectOption,
} from '../../../../shared/components/select-field/select-field.component';
import { TextFieldComponent } from '../../../../shared/components/text-field/text-field.component';
import { CmlSectionComponent } from './sections/cml-section/cml-section.component';
import { LineHeaderComponent } from './sections/line-header/line-header.component';
import { LineSummaryComponent } from './sections/line-summary/line-summary.component';
import { ReadingSectionComponent } from './sections/reading-section/reading-section.component';
import { TestPointSectionComponent } from './sections/test-point-section/test-point-section.component';

@Component({
  selector: 'app-line-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    DateFieldComponent,
    NumberFieldComponent,
    SelectFieldComponent,
    TextFieldComponent,
    LineHeaderComponent,
    LineSummaryComponent,
    CmlSectionComponent,
    TestPointSectionComponent,
    ReadingSectionComponent,
  ],
  templateUrl: './line-detail.component.html',
  styleUrl: './line-detail.component.css',
})
export class LineDetailComponent implements OnInit {
  readonly testPointPositionOptions: readonly SelectOption[] = [
    { label: '0°', value: 0 },
    { label: '90°', value: 90 },
    { label: '180°', value: 180 },
    { label: '270°', value: 270 },
  ];
  private readonly api = inject(PipingApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  readonly line = signal<PipingLineDetail | null>(null);
  readonly loading = signal(true);
  readonly errorMessage = signal('');
  readonly selectedCmlId = signal<number | null>(null);
  readonly selectedTestPointId = signal<number | null>(null);
  readonly editingCmlId = signal<number | null>(null);
  readonly cmlDraft = signal<CmlWrite>(this.emptyCml());
  readonly editingTestPointId = signal<number | null>(null);
  readonly testPointDraft = signal<TestPointWrite>(this.emptyTestPoint());
  readonly editingReadingId = signal<number | null>(null);
  readonly readingDraft = signal<ThicknessReadingWrite>(this.emptyReading());
  readonly saving = signal(false);
  readonly selectedCml = computed(
    () => this.line()?.cmls.find((x) => x.id === this.selectedCmlId()) ?? null,
  );
  readonly selectedTestPoint = computed(
    () => this.selectedCml()?.testPoints.find((x) => x.id === this.selectedTestPointId()) ?? null,
  );

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!Number.isInteger(id) || id <= 0) {
      this.errorMessage.set('Line Number ไม่ถูกต้อง');
      this.loading.set(false);
      return;
    }
    this.loading.set(true);
    this.api.getLine(id).subscribe({
      next: (value) => {
        this.line.set(value);
        this.loading.set(false);
        this.errorMessage.set('');
        if (!value.cmls.some((x) => x.id === this.selectedCmlId()))
          this.selectedCmlId.set(value.cmls[0]?.id ?? null);
        const cml = value.cmls.find((x) => x.id === this.selectedCmlId());
        if (!cml?.testPoints.some((x) => x.id === this.selectedTestPointId()))
          this.selectedTestPointId.set(cml?.testPoints[0]?.id ?? null);
      },
      error: (error) => {
        this.errorMessage.set(toUserMessage(error));
        this.loading.set(false);
      },
    });
  }

  selectCml(cml: Cml): void {
    this.selectedCmlId.set(cml.id);
    this.selectedTestPointId.set(cml.testPoints[0]?.id ?? null);
  }
  selectTestPoint(point: TestPoint): void {
    this.selectedTestPointId.set(point.id);
  }
  updateCmlDraft<K extends keyof CmlWrite>(key: K, value: CmlWrite[K]): void {
    this.cmlDraft.update((draft) => ({ ...draft, [key]: value }));
  }
  updateTestPointDraft<K extends keyof TestPointWrite>(key: K, value: TestPointWrite[K]): void {
    this.testPointDraft.update((draft) => ({ ...draft, [key]: value }));
  }
  updateReadingDraft<K extends keyof ThicknessReadingWrite>(
    key: K,
    value: ThicknessReadingWrite[K],
  ): void {
    this.readingDraft.update((draft) => ({ ...draft, [key]: value }));
  }
  back(): void {
    void this.router.navigateByUrl('/');
  }

  addCml(dialog: HTMLDialogElement): void {
    this.editingCmlId.set(null);
    this.cmlDraft.set(this.emptyCml());
    this.errorMessage.set('');
    dialog.showModal();
  }
  editCml(value: Cml, dialog: HTMLDialogElement): void {
    this.errorMessage.set('');
    this.editingCmlId.set(value.id);
    this.cmlDraft.set({ cmlNumber: value.cmlNumber, description: value.description });
    dialog.showModal();
  }
  saveCml(dialog: HTMLDialogElement): void {
    const line = this.line();
    const draft = this.cmlDraft();
    const editingId = this.editingCmlId();
    if (!line || !draft.description.trim()) return;
    this.saveWith(
      editingId === null
        ? this.api.createCml(line.id, draft)
        : this.api.updateCml(line.id, editingId, draft),
      dialog,
    );
  }
  deleteCml(value: Cml): void {
    const line = this.line();
    if (!line || !window.confirm(`ลบ CML ${value.cmlNumber}?`)) return;
    this.api.deleteCml(line.id, value.id).subscribe({
      next: () => this.load(),
      error: (error) => this.errorMessage.set(toUserMessage(error)),
    });
  }

  addTestPoint(dialog: HTMLDialogElement): void {
    this.errorMessage.set('');
    this.editingTestPointId.set(null);
    this.testPointDraft.set(this.emptyTestPoint());
    dialog.showModal();
  }
  editTestPoint(value: TestPoint, dialog: HTMLDialogElement): void {
    this.errorMessage.set('');
    this.editingTestPointId.set(value.id);
    this.testPointDraft.set({
      tpNumber: value.tpNumber,
      tpDescription: value.tpDescription,
      note: value.note,
    });
    dialog.showModal();
  }
  saveTestPoint(dialog: HTMLDialogElement): void {
    const cml = this.selectedCml();
    const draft = this.testPointDraft();
    const editingId = this.editingTestPointId();
    if (!cml || draft.tpDescription < 0 || draft.tpDescription > 360) return;
    this.saveWith(
      editingId === null
        ? this.api.createTestPoint(cml.id, draft)
        : this.api.updateTestPoint(cml.id, editingId, draft),
      dialog,
    );
  }
  deleteTestPoint(value: TestPoint): void {
    const cml = this.selectedCml();
    if (!cml || !window.confirm(`ลบ Test Point ${value.tpNumber}?`)) return;
    this.api.deleteTestPoint(cml.id, value.id).subscribe({
      next: () => this.load(),
      error: (error) => this.errorMessage.set(toUserMessage(error)),
    });
  }

  addReading(dialog: HTMLDialogElement): void {
    this.errorMessage.set('');
    this.editingReadingId.set(null);
    this.readingDraft.set(this.emptyReading());
    dialog.showModal();
  }
  editReading(value: ThicknessReading, dialog: HTMLDialogElement): void {
    this.errorMessage.set('');
    this.editingReadingId.set(value.id);
    this.readingDraft.set({
      inspectionDate: value.inspectionDate,
      actualThicknessMm: value.actualThicknessMm,
    });
    dialog.showModal();
  }
  saveReading(dialog: HTMLDialogElement): void {
    const point = this.selectedTestPoint();
    const draft = this.readingDraft();
    const editingId = this.editingReadingId();
    if (!point || !draft.inspectionDate || draft.actualThicknessMm <= 0) return;
    this.saveWith(
      editingId === null
        ? this.api.createReading(point.id, draft)
        : this.api.updateReading(point.id, editingId, draft),
      dialog,
    );
  }
  deleteReading(value: ThicknessReading): void {
    const point = this.selectedTestPoint();
    if (!point || !window.confirm(`ลบผลตรวจวันที่ ${value.inspectionDate}?`)) return;
    this.api.deleteReading(point.id, value.id).subscribe({
      next: () => this.load(),
      error: (error) => this.errorMessage.set(toUserMessage(error)),
    });
  }

  cancelForms(dialog: HTMLDialogElement): void {
    dialog.close();
  }
  private saveWith(request: Observable<unknown>, dialog: HTMLDialogElement): void {
    this.saving.set(true);
    this.errorMessage.set('');
    request.subscribe({
      next: () => {
        this.saving.set(false);
        dialog.close();
        this.load();
      },
      error: (error) => {
        this.saving.set(false);
        this.errorMessage.set(toUserMessage(error));
      },
    });
  }
  private emptyCml(): CmlWrite {
    return { cmlNumber: 1, description: '' };
  }
  private emptyTestPoint(): TestPointWrite {
    return { tpNumber: 1, tpDescription: 0, note: null };
  }
  private emptyReading(): ThicknessReadingWrite {
    return { inspectionDate: new Date().toISOString().slice(0, 10), actualThicknessMm: 0 };
  }
}
