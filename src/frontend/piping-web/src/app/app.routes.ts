import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    title: 'Piping lines | Piping Integrity',
    loadComponent: () =>
      import('./features/piping-lines/pages/line-index/line-index.component').then(
        (m) => m.LineIndexComponent,
      ),
  },
  {
    path: 'lines/:id',
    title: 'Piping detail | Piping Integrity',
    loadComponent: () =>
      import('./features/piping-lines/pages/line-detail/line-detail.component').then(
        (m) => m.LineDetailComponent,
      ),
  },
  { path: '**', redirectTo: '' },
];
