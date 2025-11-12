import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ConstructionExpenseService } from '../../../services/construction-services';
import { ConstructionProjectService } from '../../../services/construction-project.service';
import { ProjectExpense, CreateProjectExpense } from '../../../models/construction.model';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-construction-expenses',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule],
    templateUrl: './construction-expenses.component.html',
    styleUrls: ['./construction-expenses.component.css'],
})
export class ConstructionExpensesComponent implements OnInit {
    expenseForm: FormGroup;
    expenses: ProjectExpense[] = [];
    projects: any[] = [];
    loading = false;
    showForm = false;
    selectedProjectId: number = 0;

    expenseCategories = ['Material', 'Labor', 'Equipment', 'Services', 'Utilities', 'Transportation', 'Other'];
    paymentMethods = ['Cash', 'Bank Transfer', 'Cheque', 'Online Transfer'];

    constructor(
        private fb: FormBuilder,
        private expenseService: ConstructionExpenseService,
        private projectService: ConstructionProjectService
    ) {
        this.expenseForm = this.fb.group({
            expenseCategory: ['Material', [Validators.required, Validators.maxLength(200)]],
            description: ['', [Validators.required, Validators.maxLength(500)]],
            amount: [0, [Validators.required, Validators.min(0.01)]],
            expenseDate: [new Date().toISOString().split('T')[0], Validators.required],
            vendorName: ['', Validators.maxLength(100)],
            invoiceNumber: ['', Validators.maxLength(100)],
            paymentMethod: ['Cash'],
            referenceNumber: ['', Validators.maxLength(100)],
            constructionProjectId: [0, [Validators.required, Validators.min(1)]],
        });
    }

    ngOnInit(): void {
        this.loadProjects();
    }

    loadProjects(): void {
        this.projectService.getAll({ skipCount: 0, maxResultCount: 100 }).subscribe({
            next: (response) => {
                this.projects = response.items;
                if (this.projects.length > 0 && !this.selectedProjectId) {
                    this.selectedProjectId = this.projects[0].id;
                    this.loadExpenses();
                }
            },
            error: (error) => {
                console.error('Error loading projects:', error);
                this.showError('Failed to load projects');
            },
        });
    }

    onProjectChange(): void {
        if (this.selectedProjectId) {
            this.loadExpenses();
        }
    }

    loadExpenses(): void {
        if (!this.selectedProjectId) return;
        
        this.loading = true;
        this.expenseService.getByProject(this.selectedProjectId).subscribe({
            next: (data) => {
                this.expenses = data;
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading expenses:', error);
                this.showError('Failed to load expenses');
                this.loading = false;
            },
        });
    }

    openForm(): void {
        this.showForm = true;
        this.expenseForm.reset({
            expenseCategory: 'Material',
            amount: 0,
            expenseDate: new Date().toISOString().split('T')[0],
            paymentMethod: 'Cash',
            constructionProjectId: this.selectedProjectId || 0,
        });
    }

    closeForm(): void {
        this.showForm = false;
        this.expenseForm.reset();
    }

    onSubmit(): void {
        if (this.expenseForm.invalid) {
            this.showError('Please fill all required fields correctly');
            return;
        }

        this.loading = true;
        const formValue = this.expenseForm.value;
        
        const expenseData: CreateProjectExpense = {
            ...formValue,
            expenseDate: new Date(formValue.expenseDate),
        };

        this.expenseService.create(expenseData).subscribe({
            next: () => {
                this.showSuccess('Expense recorded successfully! Ledger entries created.');
                this.loadExpenses();
                this.closeForm();
                this.loading = false;
            },
            error: (error) => {
                console.error('Error recording expense:', error);
                this.showError('Failed to record expense');
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

    getCategoryBadgeClass(category: string): string {
        const colors: any = {
            'Material': 'badge-primary',
            'Labor': 'badge-success',
            'Equipment': 'badge-info',
            'Services': 'badge-warning',
            'Utilities': 'badge-secondary',
            'Transportation': 'badge-dark',
            'Other': 'badge-light'
        };
        return `badge ${colors[category] || 'badge-secondary'}`;
    }

    private showSuccess(message: string): void {
        Swal.fire({
            icon: 'success',
            title: 'Success!',
            text: message,
            timer: 2500,
            showConfirmButton: false,
        });
    }

    private showError(message: string): void {
        Swal.fire({
            icon: 'error',
            title: 'Error!',
            text: message,
        });
    }
}
