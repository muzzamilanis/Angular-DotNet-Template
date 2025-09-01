import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProjectService } from '../../services/project.service';
import { ServiceTypeService } from '../../services/service-type.service';
import { CompanyService } from '../../services/company.service';
import { Project, ProjectStatus, CreateProjectDto, UpdateProjectDto } from '../../models/project.model';
import { ServiceType } from '../../models/service-type.model';
import { Company } from '../../models/company.model';
import { BusinessNatureType } from '../../models/multi-tenant.model';

@Component({
    selector: 'app-project-form',
    templateUrl: './project-form.component.html',
    styleUrls: ['./project-form.component.css'],
    standalone: false,
})
export class ProjectFormComponent implements OnInit {
    projectForm: FormGroup;
    isEditMode = false;
    projectId?: number;
    loading = false;
    submitting = false;

    // Alert state
    alertMessage = '';
    alertType = '';
    showAlert = false;

    // Service Types and Company info
    serviceTypes: ServiceType[] = [];
    currentCompany: Company | null = null;
    isDigitalMarketing = false;

    // Project Status options for dropdown
    statusOptions = [
        { label: 'Active', value: ProjectStatus.Active },
        { label: 'Inactive', value: ProjectStatus.Inactive },
        { label: 'Completed', value: ProjectStatus.Completed },
    ];

    constructor(
        private fb: FormBuilder,
        private projectService: ProjectService,
        private serviceTypeService: ServiceTypeService,
        private companyService: CompanyService,
        private route: ActivatedRoute,
        private router: Router
    ) {
        this.projectForm = this.createForm();
    }

    ngOnInit(): void {
        this.loadCurrentCompany();
        this.route.params.subscribe((params) => {
            if (params['id']) {
                this.isEditMode = true;
                this.projectId = +params['id'];
                this.loadProject();
            }
        });
    }

    private createForm(): FormGroup {
        return this.fb.group({
            projectName: ['', [Validators.required, Validators.maxLength(200)]],
            projectDescription: ['', Validators.maxLength(1000)],
            projectStatus: [ProjectStatus.Active, Validators.required],
            serviceTypeId: [''],
            isActive: [true],
        });
    }

    private loadCurrentCompany(): void {
        this.companyService.getCurrentCompany().subscribe({
            next: (company) => {
                this.currentCompany = company;
                this.isDigitalMarketing = company.businessNatureId === BusinessNatureType.DIGITALMARKETINGAGENCY;

                if (this.isDigitalMarketing) {
                    this.loadServiceTypes();
                }
            },
            error: (error) => {
                console.error('Error loading current company:', error);
                this.isDigitalMarketing = false;
            },
        });
    }

    private loadServiceTypes(): void {
        if (this.currentCompany) {
            this.serviceTypeService.getServiceTypesByCompany(this.currentCompany.id).subscribe({
                next: (serviceTypes: ServiceType[]) => {
                    this.serviceTypes = serviceTypes || [];
                },
                error: (error) => {
                    console.error('Error loading service types:', error);
                },
            });
        }
    }

    private loadProject(): void {
        if (!this.projectId) return;

        this.loading = true;
        this.projectService.getById(this.projectId).subscribe({
            next: (project: Project) => {
                this.projectForm.patchValue({
                    projectName: project.projectName,
                    projectDescription: project.projectDescription,
                    projectStatus: this.normalizeStatus(project.projectStatus), // <-- normalize for edit mode
                    serviceTypeId: project.serviceTypeId || '',
                    isActive: project.isActive,
                });
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading project:', error);
                this.showAlertMessage('Failed to load project', 'danger');
                this.loading = false;
                this.router.navigate(['/masters/projects']);
            },
        });
    }

    // Normalize API status (numbers 0..2 or 1..3, or strings) to ProjectStatus enum
    private normalizeStatus(status: ProjectStatus | number | string | null | undefined): ProjectStatus {
        if (typeof status === 'number') {
            if (status >= 1 && status <= 3) return status as ProjectStatus;
            if (status >= 0 && status <= 2) return (status + 1) as ProjectStatus;
        }
        if (typeof status === 'string') {
            const s = status.trim().toLowerCase();
            if (/^\d+$/.test(s)) {
                const n = parseInt(s, 10);
                if (n >= 1 && n <= 3) return n as ProjectStatus;
                if (n >= 0 && n <= 2) return (n + 1) as ProjectStatus;
            }
            if (s === 'active') return ProjectStatus.Active;
            if (s === 'inactive') return ProjectStatus.Inactive;
            if (s === 'completed' || s === 'complete') return ProjectStatus.Completed;
        }
        return ProjectStatus.Active;
    }

    onSubmit(): void {
        if (this.projectForm.invalid) {
            this.markFormGroupTouched(this.projectForm);
            return;
        }

        this.submitting = true;
        const formValue = this.projectForm.value;

        if (this.isEditMode && this.projectId) {
            const updateDto: UpdateProjectDto = {
                id: this.projectId,
                ...formValue,
            };

            this.projectService.update(this.projectId, updateDto).subscribe({
                next: () => {
                    this.showAlertMessage('Project updated successfully', 'success');
                    setTimeout(() => {
                        this.router.navigate(['/masters/projects']);
                    }, 1500);
                },
                error: (error) => {
                    console.error('Error updating project:', error);
                    this.showAlertMessage('Failed to update project', 'danger');
                    this.submitting = false;
                },
            });
        } else {
            const createDto: CreateProjectDto = formValue;

            this.projectService.create(createDto).subscribe({
                next: () => {
                    this.showAlertMessage('Project created successfully', 'success');
                    setTimeout(() => {
                        this.router.navigate(['/masters/projects']);
                    }, 1500);
                },
                error: (error) => {
                    console.error('Error creating project:', error);
                    this.showAlertMessage('Failed to create project', 'danger');
                    this.submitting = false;
                },
            });
        }
    }

    onCancel(): void {
        this.router.navigate(['/masters/projects']);
    }

    private markFormGroupTouched(formGroup: FormGroup): void {
        Object.keys(formGroup.controls).forEach((key) => {
            const control = formGroup.get(key);
            control?.markAsTouched();

            if (control instanceof FormGroup) {
                this.markFormGroupTouched(control);
            }
        });
    }

    isFieldInvalid(fieldName: string): boolean {
        const field = this.projectForm.get(fieldName);
        return !!(field && field.invalid && (field.dirty || field.touched));
    }

    getFieldError(fieldName: string): string {
        const field = this.projectForm.get(fieldName);

        if (field?.errors) {
            if (field.errors['required']) {
                return `${this.getFieldDisplayName(fieldName)} is required`;
            }
            if (field.errors['maxlength']) {
                return `${this.getFieldDisplayName(fieldName)} cannot exceed ${field.errors['maxlength'].requiredLength} characters`;
            }
        }

        return '';
    }

    private getFieldDisplayName(fieldName: string): string {
        const displayNames: { [key: string]: string } = {
            projectName: 'Project Name',
            projectDescription: 'Project Description',
            projectStatus: 'Project Status',
        };

        return displayNames[fieldName] || fieldName;
    }

    private showAlertMessage(message: string, type: string): void {
        this.alertMessage = message;
        this.alertType = type;
        this.showAlert = true;

        // Auto-hide after 5 seconds
        setTimeout(() => {
            this.showAlert = false;
        }, 5000);
    }

    closeAlert(): void {
        this.showAlert = false;
    }
}
