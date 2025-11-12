import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ConstructionLedgerService, ConstructionClientService, ConstructionInvestorService } from '../../../services/construction-services';
import { ConstructionProjectService } from '../../../services/construction-project.service';
import { ClientLedgerEntry, InvestorLedgerEntry, ProjectLedgerEntry } from '../../../models/construction.model';

@Component({
    selector: 'app-construction-ledgers',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './construction-ledgers.component.html',
    styleUrls: ['./construction-ledgers.component.css'],
})
export class ConstructionLedgersComponent implements OnInit {
    ledgerType: 'client' | 'investor' | 'project' = 'project';
    projects: any[] = [];
    clients: any[] = [];
    investors: any[] = [];
    
    selectedProjectId: number = 0;
    selectedClientId: number = 0;
    selectedInvestorId: number = 0;
    
    clientEntries: ClientLedgerEntry[] = [];
    investorEntries: InvestorLedgerEntry[] = [];
    projectEntries: ProjectLedgerEntry[] = [];
    
    loading = false;

    constructor(
        private ledgerService: ConstructionLedgerService,
        private projectService: ConstructionProjectService,
        private clientService: ConstructionClientService,
        private investorService: ConstructionInvestorService
    ) {}

    ngOnInit(): void {
        this.loadProjects();
    }

    loadProjects(): void {
        this.projectService.getAll({ skipCount: 0, maxResultCount: 100 }).subscribe({
            next: (response) => {
                this.projects = response.items;
            },
            error: (error) => console.error('Error loading projects:', error),
        });
    }

    onLedgerTypeChange(): void {
        this.clientEntries = [];
        this.investorEntries = [];
        this.projectEntries = [];
        
        if (this.ledgerType === 'project' && this.selectedProjectId) {
            this.loadProjectLedger();
        } else if (this.ledgerType === 'client') {
            this.loadClientsForProject();
        } else if (this.ledgerType === 'investor') {
            this.loadInvestorsForProject();
        }
    }

    onProjectChange(): void {
        if (this.ledgerType === 'project' && this.selectedProjectId) {
            this.loadProjectLedger();
        } else if (this.ledgerType === 'client' && this.selectedProjectId) {
            this.loadClientsForProject();
        } else if (this.ledgerType === 'investor' && this.selectedProjectId) {
            this.loadInvestorsForProject();
        }
    }

    loadClientsForProject(): void {
        if (!this.selectedProjectId) return;
        this.clientService.getByProject(this.selectedProjectId).subscribe({
            next: (data) => {
                this.clients = data;
            },
            error: (error) => console.error('Error loading clients:', error),
        });
    }

    loadInvestorsForProject(): void {
        if (!this.selectedProjectId) return;
        this.investorService.getByProject(this.selectedProjectId).subscribe({
            next: (data) => {
                this.investors = data;
            },
            error: (error) => console.error('Error loading investors:', error),
        });
    }

    onClientChange(): void {
        if (this.selectedClientId) {
            this.loadClientLedger();
        }
    }

    onInvestorChange(): void {
        if (this.selectedInvestorId) {
            this.loadInvestorLedger();
        }
    }

    loadClientLedger(): void {
        this.loading = true;
        this.ledgerService.getClientLedger(this.selectedClientId).subscribe({
            next: (data) => {
                this.clientEntries = data;
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading client ledger:', error);
                this.loading = false;
            },
        });
    }

    loadInvestorLedger(): void {
        this.loading = true;
        this.ledgerService.getInvestorLedger(this.selectedInvestorId).subscribe({
            next: (data) => {
                this.investorEntries = data;
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading investor ledger:', error);
                this.loading = false;
            },
        });
    }

    loadProjectLedger(): void {
        this.loading = true;
        this.ledgerService.getProjectLedger(this.selectedProjectId).subscribe({
            next: (data) => {
                this.projectEntries = data;
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading project ledger:', error);
                this.loading = false;
            },
        });
    }

    formatCurrency(value: number): string {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'PKR',
        }).format(value);
    }

    formatDate(date: Date): string {
        return new Date(date).toLocaleDateString();
    }

    getEntryTypeClass(entryType: string): string {
        return entryType === 'Debit' ? 'text-danger' : 'text-success';
    }
}
