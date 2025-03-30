import { Component, OnInit } from '@angular/core';
import { LeadService } from './services/lead.service';
import { CommonModule } from '@angular/common';
import { LeadCardComponent } from './lead-card/lead-card.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, LeadCardComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  activeTab: 'invited' | 'accepted' = 'invited';
  invitedLeads: any[] = [];
  acceptedLeads: any[] = [];

  constructor(private leadService: LeadService) {}

  ngOnInit(): void {
    this.loadLeads();
  }

  loadLeads(): void {
    this.leadService.getLeads('Invited').subscribe({
      next: (data) => (this.invitedLeads = data),
      error: (err) => console.error('Error loading invited leads:', err),
    });

    this.leadService.getLeads('Accepted').subscribe({
      next: (data) => (this.acceptedLeads = data),
      error: (err) => console.error('Error loading accepted leads:', err),
    });
  }

  onLeadAccepted(leadId: number): void {
    this.invitedLeads = this.invitedLeads.filter((lead) => lead.id !== leadId);
    this.leadService
      .getLeads('Accepted')
      .subscribe((data) => (this.acceptedLeads = data));
  }

  onLeadDeclined(leadId: number): void {
    this.invitedLeads = this.invitedLeads.filter((lead) => lead.id !== leadId);
  }

  switchTab(tabName: 'invited' | 'accepted'): void {
    this.activeTab = tabName;
  }
}
