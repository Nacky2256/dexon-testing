import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TestPoint } from '../../../../../../models';

@Component({
  selector: 'app-test-point-section',
  standalone: true,
  templateUrl: './test-point-section.component.html',
  styleUrl: './test-point-section.component.css',
})
export class TestPointSectionComponent {
  @Input() cmlNumber = 0;
  @Input() testPoints: TestPoint[] = [];
  @Input() selectedTestPointId: number | null = null;

  @Output() add = new EventEmitter<void>();
  @Output() select = new EventEmitter<TestPoint>();
  @Output() edit = new EventEmitter<TestPoint>();
  @Output() remove = new EventEmitter<TestPoint>();
}
