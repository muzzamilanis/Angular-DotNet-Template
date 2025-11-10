import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ConstructionProjectService } from '../../services/construction-project.service';
import { ConstructionDashboard } from '../../models/construction.model';

@Component({
    selector: 'app-construction-dashboard',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './construction-dashboard.component.html',
    styleUrls: ['./construction-dashboard.component.css'],
})
export class ConstructionDashboardComponent implements OnInit {
    dashboard: ConstructionDashboard | null = null;
    loading = true;
    error: string | null = null;

    constructor(
        private constructionService: ConstructionProjectService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.loadDashboard();
    }

    loadDashboard(): void {
        this.loading = true;
        this.error = null;
        
        this.constructionService.getDashboard().subscribe({
            next: (data) => {
                this.dashboard = data;
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading dashboard:', error);
                this.error = 'Failed to load dashboard data';
                this.loading = false;
            },
        });
    }

    navigateToProjects(): void {
        this.router.navigate(['/construction/projects']);
    }

    navigateToClients(): void {
        this.router.navigate(['/construction/clients']);
    }

    navigateToInvestors(): void {
        this.router.navigate(['/construction/investors']);
    }

    navigateToPayments(): void {
        this.router.navigate(['/construction/payments']);
    }

    navigateToExpenses(): void {
        this.router.navigate(['/construction/expenses']);
    }

    getStatusColor(profitOrLoss: number): string {
        return profitOrLoss >= 0 ? 'text-success' : 'text-danger';
    }

    formatCurrency(value: number): string {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
        }).format(value);
    }
}
