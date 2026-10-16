import { TestBed } from '@angular/core/testing';
import { describe, expect, it } from 'vitest';
import { provideRouter } from '@angular/router';
import { AppComponent } from './app.component';

describe('AppComponent', () => {
  it('renders the product identity and routed application outlet', async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [provideRouter([])],
    }).compileComponents();
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Piping Integrity');
    expect(fixture.nativeElement.querySelector('router-outlet')).toBeTruthy();
  });
});
