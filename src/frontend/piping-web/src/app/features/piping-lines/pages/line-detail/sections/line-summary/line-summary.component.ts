import { Component, Input } from '@angular/core';
import { PipingLineDetail } from '../../../../../../models';

@Component({
  selector: 'app-line-summary',
  standalone: true,
  templateUrl: './line-summary.component.html',
  styleUrl: './line-summary.component.css',
})
export class LineSummaryComponent {
  @Input() line!: PipingLineDetail;
}
