import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Cml } from '../../../../../../models';

@Component({
  selector: 'app-cml-section',
  standalone: true,
  templateUrl: './cml-section.component.html',
  styleUrl: './cml-section.component.css',
})
export class CmlSectionComponent {
  @Input() cmls: Cml[] = [];
  @Input() selectedCmlId: number | null = null;

  @Output() add = new EventEmitter<void>();
  @Output() select = new EventEmitter<Cml>();
  @Output() edit = new EventEmitter<Cml>();
  @Output() remove = new EventEmitter<Cml>();
}
