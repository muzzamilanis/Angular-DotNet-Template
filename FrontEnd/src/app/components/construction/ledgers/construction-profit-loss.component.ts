import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ConstructionProjectService } from '../../../services/construction-project.service';
import { ProjectProfitLoss } from '../../../models/construction.model';

@Component({
    selector: 'app-construction-profit-loss',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './construction-profit-loss.component.html',
    styleUrls: ['./construction-profit-loss.component.css'],
})
export class ConstructionProfitLossComponent implements OnInit {
    projects: any[] = [];
    selectedProjectId: number = 0;
    profitLoss: ProjectProfitLoss | null = null;
    loading = false;

    constructor(private projectService: ConstructionProjectService) {}

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

    onProjectChange(): void {
        if (this.selectedProjectId) {
            this.loadProfitLoss();
        }
    }

    loadProfitLoss(): void {
        this.loading = true;
        this.projectService.getProfitLoss(this.selectedProjectId).subscribe({
            next: (data) => {
                this.profitLoss = data;
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading P&L:', error);
                this.loading = false;
            },
        });
    }

    formatCurrency(value: number): string {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
        }).format(value);
    }

    formatPercentage(value: number): string {
        return value.toFixed(2) + '%';
    }

    getProfitLossClass(): string {
        return this.profitLoss && this.profitLoss.profitOrLoss >= 0 ? 'text-success' : 'text-danger';
    }
}
