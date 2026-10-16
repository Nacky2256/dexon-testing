import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ThicknessReading } from '../../../../../../models';

@Component({
  selector: 'app-reading-section',
  standalone: true,
  templateUrl: './reading-section.component.html',
  styleUrl: './reading-section.component.css',
})
export class ReadingSectionComponent {
  @Input() testPointNumber = 0;
  @Input() readings: ThicknessReading[] = [];

  @Output() add = new EventEmitter<void>();
  @Output() edit = new EventEmitter<ThicknessReading>();
  @Output() remove = new EventEmitter<ThicknessReading>();
}
