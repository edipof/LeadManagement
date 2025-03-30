import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectorRef,
} from '@angular/core';
import { LeadService } from '../services/lead.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-lead-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './lead-card.component.html',
  styleUrls: ['./lead-card.component.scss'],
})
export class LeadCardComponent {
  @Output() leadAccepted = new EventEmitter<number>();
  @Output() leadDeclined = new EventEmitter<number>();

  @Input() activeTab: 'invited' | 'accepted' = 'invited';
  @Input() lead?: {
    id: number;
    firstName: string;
    lastName?: string;
    suburb?: string;
    jobTitle: string;
    description: string;
    price: number;
    dateCreated?: Date;
    jobId?: number;
    phoneNumber?: string;
    email?: string;
  };

  isLoading = false;

  constructor(private leadService: LeadService) {}

  acceptLead(id: number) {
    this.isLoading = true;
    this.leadService.acceptLead(id).subscribe({
      next: (response) => {
        this.leadAccepted.emit(id);
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error accepting lead:', error);
        this.isLoading = false;
      },
    });
  }

  declineLead(id: number) {
    this.isLoading = true;
    this.leadService.declineLead(id).subscribe({
      next: (response) => {
        this.leadDeclined.emit(id);
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error declining lead:', error);
        this.isLoading = false;
      },
    });
  }
}
