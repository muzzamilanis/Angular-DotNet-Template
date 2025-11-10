import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ConstructionPaymentService, ConstructionClientService, ConstructionInvestorService } from '../../../services/construction-services';
import { ConstructionProjectService } from '../../../services/construction-project.service';
import { ProjectPayment, CreateProjectPayment, PaymentSource } from '../../../models/construction.model';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-construction-payments',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule],
    templateUrl: './construction-payments.component.html',
    styleUrls: ['./construction-payments.component.css'],
})
export class ConstructionPaymentsComponent implements OnInit {
    paymentForm: FormGroup;
    payments: ProjectPayment[] = [];
    projects: any[] = [];
    clients: any[] = [];
    investors: any[] = [];
    loading = false;
    showForm = false;
    selectedProjectId: number = 0;

    paymentSources = Object.values(PaymentSource);
    paymentMethods = ['Cash', 'Bank Transfer', 'Cheque', 'Online Transfer'];

    constructor(
        private fb: FormBuilder,
        private paymentService: ConstructionPaymentService,
        private projectService: ConstructionProjectService,
        private clientService: ConstructionClientService,
        private investorService: ConstructionInvestorService
    ) {
        this.paymentForm = this.fb.group({
            paymentSource: [PaymentSource.Client, Validators.required],
            projectClientId: [null],
            projectInvestorId: [null],
            amount: [0, [Validators.required, Validators.min(0.01)]],
            paymentDate: [new Date().toISOString().split('T')[0], Validators.required],
            paymentMethod: ['Cash'],
            referenceNumber: ['', Validators.maxLength(100)],
            description: ['', Validators.maxLength(1000)],
            constructionProjectId: [0, [Validators.required, Validators.min(1)]],
        });
    }

    ngOnInit(): void {
        this.loadProjects();
        this.setupFormListeners();
    }

    setupFormListeners(): void {
        this.paymentForm.get('paymentSource')?.valueChanges.subscribe((source) => {
            if (source === PaymentSource.Client) {
                this.paymentForm.get('projectClientId')?.setValidators([Validators.required]);
                this.paymentForm.get('projectInvestorId')?.clearValidators();
                this.paymentForm.patchValue({ projectInvestorId: null });
            } else {
                this.paymentForm.get('projectInvestorId')?.setValidators([Validators.required]);
                this.paymentForm.get('projectClientId')?.clearValidators();
                this.paymentForm.patchValue({ projectClientId: null });
            }
            this.paymentForm.get('projectClientId')?.updateValueAndValidity();
            this.paymentForm.get('projectInvestorId')?.updateValueAndValidity();
        });
    }

    loadProjects(): void {
        this.projectService.getAll({ skipCount: 0, maxResultCount: 100 }).subscribe({
            next: (response) => {
                this.projects = response.items;
                if (this.projects.length > 0 && !this.selectedProjectId) {
                    this.selectedProjectId = this.projects[0].id;
                    this.onProjectChange();
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
            this.loadPayments();
            this.loadClients();
            this.loadInvestors();
        }
    }

    loadClients(): void {
        this.clientService.getByProject(this.selectedProjectId).subscribe({
            next: (data) => {
                this.clients = data;
            },
            error: (error) => console.error('Error loading clients:', error),
        });
    }

    loadInvestors(): void {
        this.investorService.getByProject(this.selectedProjectId).subscribe({
            next: (data) => {
                this.investors = data;
            },
            error: (error) => console.error('Error loading investors:', error),
        });
    }

    loadPayments(): void {
        if (!this.selectedProjectId) return;
        
        this.loading = true;
        this.paymentService.getByProject(this.selectedProjectId).subscribe({
            next: (data) => {
                this.payments = data;
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading payments:', error);
                this.showError('Failed to load payments');
                this.loading = false;
            },
        });
    }

    openForm(): void {
        this.showForm = true;
        this.paymentForm.reset({
            paymentSource: PaymentSource.Client,
            amount: 0,
            paymentDate: new Date().toISOString().split('T')[0],
            paymentMethod: 'Cash',
            constructionProjectId: this.selectedProjectId || 0,
        });
    }

    closeForm(): void {
        this.showForm = false;
        this.paymentForm.reset();
    }

    onSubmit(): void {
        if (this.paymentForm.invalid) {
            this.showError('Please fill all required fields correctly');
            return;
        }

        this.loading = true;
        const formValue = this.paymentForm.value;
        
        const paymentData: CreateProjectPayment = {
            ...formValue,
            paymentDate: new Date(formValue.paymentDate),
        };

        this.paymentService.create(paymentData).subscribe({
            next: () => {
                this.showSuccess('Payment recorded successfully! Ledger entries created.');
                this.loadPayments();
                this.closeForm();
                this.loading = false;
            },
            error: (error) => {
                console.error('Error recording payment:', error);
                this.showError('Failed to record payment');
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

    formatDate(date: Date): string {
        return new Date(date).toLocaleDateString();
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
