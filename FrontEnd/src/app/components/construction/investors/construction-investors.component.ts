import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ConstructionInvestorService } from '../../../services/construction-services';
import { ConstructionProjectService } from '../../../services/construction-project.service';
import { ProjectInvestor, CreateProjectInvestor, UpdateProjectInvestor } from '../../../models/construction.model';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-construction-investors',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule],
    templateUrl: './construction-investors.component.html',
    styleUrls: ['./construction-investors.component.css'],
})
export class ConstructionInvestorsComponent implements OnInit {
    investorForm: FormGroup;
    investors: ProjectInvestor[] = [];
    projects: any[] = [];
    editingInvestor: ProjectInvestor | null = null;
    loading = false;
    showForm = false;
    selectedProjectId: number = 0;

    constructor(
        private fb: FormBuilder,
        private investorService: ConstructionInvestorService,
        private projectService: ConstructionProjectService
    ) {
        this.investorForm = this.fb.group({
            investorName: ['', [Validators.required, Validators.maxLength(200)]],
            address: ['', Validators.maxLength(500)],
            phone: ['', Validators.maxLength(50)],
            email: ['', [Validators.email, Validators.maxLength(200)]],
            contactPerson: ['', Validators.maxLength(100)],
            cnicOrNtn: ['', Validators.maxLength(100)],
            investmentAmount: [0, [Validators.required, Validators.min(0)]],
            sharePercentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
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
                    this.loadInvestors();
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
            this.loadInvestors();
        }
    }

    loadInvestors(): void {
        if (!this.selectedProjectId) return;
        
        this.loading = true;
        this.investorService.getByProject(this.selectedProjectId).subscribe({
            next: (data) => {
                this.investors = data;
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading investors:', error);
                this.showError('Failed to load investors');
                this.loading = false;
            },
        });
    }

    openForm(investor?: ProjectInvestor): void {
        this.showForm = true;
        if (investor) {
            this.editingInvestor = investor;
            this.investorForm.patchValue(investor);
        } else {
            this.editingInvestor = null;
            this.investorForm.reset({
                investmentAmount: 0,
                sharePercentage: 0,
                constructionProjectId: this.selectedProjectId || 0,
            });
        }
    }

    closeForm(): void {
        this.showForm = false;
        this.editingInvestor = null;
        this.investorForm.reset({ investmentAmount: 0, sharePercentage: 0 });
    }

    onSubmit(): void {
        if (this.investorForm.invalid) {
            this.showError('Please fill all required fields correctly');
            return;
        }

        this.loading = true;
        const formValue = this.investorForm.value;

        if (this.editingInvestor) {
            const updateData: UpdateProjectInvestor = {
                id: this.editingInvestor.id,
                ...formValue,
                isActive: this.editingInvestor.isActive,
            };
            
            this.investorService.update(this.editingInvestor.id, updateData).subscribe({
                next: () => {
                    this.showSuccess('Investor updated successfully!');
                    this.loadInvestors();
                    this.closeForm();
                    this.loading = false;
                },
                error: (error) => {
                    console.error('Error updating investor:', error);
                    this.showError('Failed to update investor');
                    this.loading = false;
                },
            });
        } else {
            const createData: CreateProjectInvestor = formValue;
            
            this.investorService.create(createData).subscribe({
                next: () => {
                    this.showSuccess('Investor created successfully!');
                    this.loadInvestors();
                    this.closeForm();
                    this.loading = false;
                },
                error: (error) => {
                    console.error('Error creating investor:', error);
                    this.showError('Failed to create investor');
                    this.loading = false;
                },
            });
        }
    }

    deleteInvestor(investor: ProjectInvestor): void {
        Swal.fire({
            title: 'Delete Investor?',
            text: `Are you sure you want to delete "${investor.investorName}"?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, delete it!',
        }).then((result) => {
            if (result.isConfirmed) {
                this.investorService.delete(investor.id).subscribe({
                    next: () => {
                        this.showSuccess('Investor deleted successfully!');
                        this.loadInvestors();
                    },
                    error: (error) => {
                        console.error('Error deleting investor:', error);
                        this.showError('Failed to delete investor');
                    },
                });
            }
        });
    }

    formatCurrency(value: number | undefined): string {
        if (!value) return '$0.00';
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD',
        }).format(value);
    }

    private showSuccess(message: string): void {
        Swal.fire({
            icon: 'success',
            title: 'Success!',
            text: message,
            timer: 2000,
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
