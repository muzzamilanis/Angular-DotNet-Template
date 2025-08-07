import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LeadService } from '../../services/crm/lead.service';
import { Lead, CreateLeadDto, LeadStatus } from '../../models/crm/crm.model';
import { NotificationService } from '../../services/notification.service';
import { BusinessNatureService } from '../../services/business-nature.service';

@Component({
    selector: 'app-lead',
    templateUrl: './lead.component.html',
    styleUrls: ['./lead.component.css'],
    standalone: false,
})
export class LeadComponent implements OnInit {
    leadForm: FormGroup;
    leads: Lead[] = [];
    businessNatures: any[] = [];
    editingLead: Lead | null = null;
    loading = false;
    LeadStatus = LeadStatus;

    constructor(
        private fb: FormBuilder,
        private leadService: LeadService,
        private businessNatureService: BusinessNatureService,
        private notificationService: NotificationService
    ) {
        this.initializeForm();
    }

    ngOnInit(): void {
        this.loadLeads();
        this.loadBusinessNatures();
    }

    private initializeForm(): void {
        this.leadForm = this.fb.group({
            fullName: ['', [Validators.required, Validators.maxLength(200)]],
            email: ['', [Validators.required, Validators.email, Validators.maxLength(200)]],
            phone: ['', [Validators.maxLength(50)]],
            source: ['', [Validators.maxLength(200)]],
            notes: ['', [Validators.maxLength(500)]],
            status: [LeadStatus.New, [Validators.required]],
            businessNatureId: ['', [Validators.required]],
            isActive: [true],
        });
    }

    private loadLeads(): void {
        this.loading = true;
        this.leadService.getLeads(0, 100).subscribe({
            next: (response) => {
                this.leads = response.result || [];
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading leads:', error);
                this.notificationService.showError('Failed to load leads');
                this.loading = false;
            }
        });
    }

    private loadBusinessNatures(): void {
        this.businessNatureService.getBusinessNatures().subscribe({
            next: (businessNatures) => {
                this.businessNatures = businessNatures;
            },
            error: (error) => {
                console.error('Error loading business natures:', error);
            }
        });
    }

    onSubmit(): void {
        if (this.leadForm.valid) {
            this.loading = true;
            const formValue = this.leadForm.value;

            if (this.editingLead) {
                // Update existing lead
                this.leadService
                    .updateLead(this.editingLead.id, {
                        ...formValue,
                        id: this.editingLead.id,
                    })
                    .subscribe({
                        next: () => {
                            this.notificationService.showSuccess('Lead updated successfully');
                            this.loadLeads();
                            this.resetForm();
                        },
                        error: (error) => {
                            console.error('Error updating lead:', error);
                            this.notificationService.showError('Failed to update lead');
                            this.loading = false;
                        }
                    });
            } else {
                // Create new lead
                const createLeadDto: CreateLeadDto = formValue;
                this.leadService.createLead(createLeadDto).subscribe({
                    next: () => {
                        this.notificationService.showSuccess('Lead created successfully');
                        this.loadLeads();
                        this.resetForm();
                    },
                    error: (error) => {
                        console.error('Error creating lead:', error);
                        this.notificationService.showError('Failed to create lead');
                        this.loading = false;
                    }
                });
            }
        }
    }

    editLead(lead: Lead): void {
        this.editingLead = lead;
        this.leadForm.patchValue({
            fullName: lead.fullName,
            email: lead.email,
            phone: lead.phone,
            source: lead.source,
            notes: lead.notes,
            status: lead.status,
            businessNatureId: lead.businessNatureId,
            isActive: lead.isActive,
        });
    }

    deleteLead(id: number): void {
        if (confirm('Are you sure you want to delete this lead?')) {
            this.leadService.deleteLead(id).subscribe({
                next: () => {
                    this.notificationService.showSuccess('Lead deleted successfully');
                    this.loadLeads();
                },
                error: (error) => {
                    console.error('Error deleting lead:', error);
                    this.notificationService.showError('Failed to delete lead');
                }
            });
        }
    }

    resetForm(): void {
        this.editingLead = null;
        this.leadForm.reset({
            status: LeadStatus.New,
            isActive: true
        });
        this.loading = false;
    }

    getStatusText(status: LeadStatus): string {
        switch (status) {
            case LeadStatus.New: return 'New';
            case LeadStatus.Contacted: return 'Contacted';
            case LeadStatus.Qualified: return 'Qualified';
            case LeadStatus.Proposal: return 'Proposal';
            case LeadStatus.Negotiation: return 'Negotiation';
            case LeadStatus.Converted: return 'Converted';
            case LeadStatus.Lost: return 'Lost';
            case LeadStatus.Unqualified: return 'Unqualified';
            default: return 'Unknown';
        }
    }

    getStatusClass(status: LeadStatus): string {
        switch (status) {
            case LeadStatus.New: return 'badge bg-primary';
            case LeadStatus.Contacted: return 'badge bg-info';
            case LeadStatus.Qualified: return 'badge bg-warning';
            case LeadStatus.Proposal: return 'badge bg-secondary';
            case LeadStatus.Negotiation: return 'badge bg-warning';
            case LeadStatus.Converted: return 'badge bg-success';
            case LeadStatus.Lost: return 'badge bg-danger';
            case LeadStatus.Unqualified: return 'badge bg-dark';
            default: return 'badge bg-light';
        }
    }
}