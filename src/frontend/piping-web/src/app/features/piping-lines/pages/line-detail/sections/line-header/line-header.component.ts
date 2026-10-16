import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PipingLineDetail } from '../../../../../../models';

@Component({
  selector: 'app-line-header',
  standalone: true,
  templateUrl: './line-header.component.html',
  styleUrl: './line-header.component.css',
})
export class LineHeaderComponent {
  @Input() line!: PipingLineDetail;
  @Output() back = new EventEmitter<void>();
}
