import { TestBed } from '@angular/core/testing';
import { describe, expect, it, vi } from 'vitest';
import { provideRouter } from '@angular/router';
import { PipingApiService } from '../../../../piping-api.service';
import { LineIndexComponent } from './line-index.component';
import { of } from 'rxjs';

describe('LineIndexComponent', () => {
  it('shows an empty state when no piping lines exist', async () => {
    await TestBed.configureTestingModule({
      imports: [LineIndexComponent],
      providers: [
        provideRouter([]),
        { provide: PipingApiService, useValue: { listLines: () => of([]) } },
      ],
    }).compileComponents();
    const fixture = TestBed.createComponent(LineIndexComponent);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('ยังไม่มี Piping line');
  });

  it('filters lines by line number and service', async () => {
    await TestBed.configureTestingModule({
      imports: [LineIndexComponent],
      providers: [
        provideRouter([]),
        {
          provide: PipingApiService,
          useValue: {
            listLines: () =>
              of([
                {
                  id: 1,
                  lineNumber: '6-PL-01007',
                  location: 'A',
                  fromDescription: 'Pump',
                  toDescription: 'Tank',
                  pipeSizeInch: 6,
                  service: 'PL',
                },
                {
                  id: 2,
                  lineNumber: '3-GC-10017',
                  location: 'B',
                  fromDescription: 'Well',
                  toDescription: 'Sep',
                  pipeSizeInch: 3,
                  service: 'GC',
                },
              ]),
          },
        },
      ],
    }).compileComponents();
    const fixture = TestBed.createComponent(LineIndexComponent);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();
    fixture.componentInstance.search.set('GC');
    expect(fixture.componentInstance.filteredLines().map((line) => line.id)).toEqual([2]);
  });
});
