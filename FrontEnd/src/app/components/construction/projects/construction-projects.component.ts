import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ConstructionProjectService } from '../../../services/construction-project.service';
import {
    ConstructionProject,
    CreateConstructionProject,
    UpdateConstructionProject,
    ConstructionProjectStatus,
} from '../../../models/construction.model';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-construction-projects',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule],
    templateUrl: './construction-projects.component.html',
    styleUrls: ['./construction-projects.component.css'],
})
export class ConstructionProjectsComponent implements OnInit {
    projectForm: FormGroup;
    projects: ConstructionProject[] = [];
    filteredProjects: ConstructionProject[] = [];
    editingProject: ConstructionProject | null = null;
    loading = false;
    showForm = false;
    searchTerm = '';
    Math = Math; // For template usage

    // Status options
    statusOptions = Object.values(ConstructionProjectStatus);

    // Pagination
    currentPage = 1;
    pageSize = 10;
    totalItems = 0;

    constructor(
        private fb: FormBuilder,
        private projectService: ConstructionProjectService
    ) {
        this.projectForm = this.fb.group({
            projectName: ['', [Validators.required, Validators.maxLength(200)]],
            projectDescription: ['', Validators.maxLength(1000)],
            location: ['', Validators.maxLength(500)],
            status: [ConstructionProjectStatus.Planning, Validators.required],
            estimatedBudget: [0, [Validators.required, Validators.min(0)]],
            startDate: [''],
            expectedEndDate: [''],
            actualEndDate: [''],
            projectManager: ['', Validators.maxLength(100)],
            contractor: ['', Validators.maxLength(100)],
        });
    }

    ngOnInit(): void {
        this.loadProjects();
    }

    loadProjects(): void {
        this.loading = true;
        const skip = (this.currentPage - 1) * this.pageSize;
        
        this.projectService.getAll({
            skipCount: skip,
            maxResultCount: this.pageSize,
            filter: this.searchTerm,
        }).subscribe({
            next: (response) => {
                this.projects = response.items;
                this.filteredProjects = response.items;
                this.totalItems = response.totalCount;
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading projects:', error);
                this.showError('Failed to load projects');
                this.loading = false;
            },
        });
    }

    onSearch(): void {
        this.currentPage = 1;
        this.loadProjects();
    }

    openForm(project?: ConstructionProject): void {
        this.showForm = true;
        if (project) {
            this.editingProject = project;
            this.projectForm.patchValue({
                projectName: project.projectName,
                projectDescription: project.projectDescription,
                location: project.location,
                status: project.status,
                estimatedBudget: project.estimatedBudget,
                startDate: project.startDate ? new Date(project.startDate).toISOString().split('T')[0] : '',
                expectedEndDate: project.expectedEndDate ? new Date(project.expectedEndDate).toISOString().split('T')[0] : '',
                actualEndDate: project.actualEndDate ? new Date(project.actualEndDate).toISOString().split('T')[0] : '',
                projectManager: project.projectManager,
                contractor: project.contractor,
            });
        } else {
            this.editingProject = null;
            this.projectForm.reset({
                status: ConstructionProjectStatus.Planning,
                estimatedBudget: 0,
            });
        }
    }

    closeForm(): void {
        this.showForm = false;
        this.editingProject = null;
        this.projectForm.reset({
            status: ConstructionProjectStatus.Planning,
            estimatedBudget: 0,
        });
    }

    onSubmit(): void {
        if (this.projectForm.invalid) {
            this.showError('Please fill all required fields correctly');
            return;
        }

        this.loading = true;
        const formValue = this.projectForm.value;
        
        const projectData = {
            ...formValue,
            startDate: formValue.startDate ? new Date(formValue.startDate) : undefined,
            expectedEndDate: formValue.expectedEndDate ? new Date(formValue.expectedEndDate) : undefined,
            actualEndDate: formValue.actualEndDate ? new Date(formValue.actualEndDate) : undefined,
        };

        if (this.editingProject) {
            const updateData: UpdateConstructionProject = {
                id: this.editingProject.id,
                ...projectData,
                isActive: this.editingProject.isActive,
            };
            
            this.projectService.update(this.editingProject.id, updateData).subscribe({
                next: () => {
                    this.showSuccess('Project updated successfully!');
                    this.loadProjects();
                    this.closeForm();
                    this.loading = false;
                },
                error: (error) => {
                    console.error('Error updating project:', error);
                    this.showError('Failed to update project');
                    this.loading = false;
                },
            });
        } else {
            const createData: CreateConstructionProject = projectData;
            
            this.projectService.create(createData).subscribe({
                next: () => {
                    this.showSuccess('Project created successfully!');
                    this.loadProjects();
                    this.closeForm();
                    this.loading = false;
                },
                error: (error) => {
                    console.error('Error creating project:', error);
                    this.showError('Failed to create project');
                    this.loading = false;
                },
            });
        }
    }

    deleteProject(project: ConstructionProject): void {
        Swal.fire({
            title: 'Delete Project?',
            text: `Are you sure you want to delete "${project.projectName}"? This action cannot be undone.`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, delete it!',
        }).then((result) => {
            if (result.isConfirmed) {
                this.projectService.delete(project.id).subscribe({
                    next: () => {
                        this.showSuccess('Project deleted successfully!');
                        this.loadProjects();
                    },
                    error: (error) => {
                        console.error('Error deleting project:', error);
                        this.showError('Failed to delete project');
                    },
                });
            }
        });
    }

    getStatusBadgeClass(status: ConstructionProjectStatus): string {
        switch (status) {
            case ConstructionProjectStatus.Planning:
                return 'badge badge-info';
            case ConstructionProjectStatus.InProgress:
                return 'badge badge-primary';
            case ConstructionProjectStatus.OnHold:
                return 'badge badge-warning';
            case ConstructionProjectStatus.Completed:
                return 'badge badge-success';
            case ConstructionProjectStatus.Cancelled:
                return 'badge badge-danger';
            default:
                return 'badge badge-secondary';
        }
    }

    formatCurrency(value: number): string {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'PKR',
        }).format(value);
    }

    formatDate(date: Date | undefined): string {
        if (!date) return 'N/A';
        return new Date(date).toLocaleDateString();
    }

    previousPage(): void {
        if (this.currentPage > 1) {
            this.currentPage--;
            this.loadProjects();
        }
    }

    nextPage(): void {
        const totalPages = Math.ceil(this.totalItems / this.pageSize);
        if (this.currentPage < totalPages) {
            this.currentPage++;
            this.loadProjects();
        }
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
