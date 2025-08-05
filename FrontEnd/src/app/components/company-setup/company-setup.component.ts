import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BusinessNature, CreateCompanyProfileRequest } from '../../models/multi-tenant.model';
import { BusinessNatureService } from '../../services/business-nature.service';
import { CompanyService } from '../../services/company.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';

@Component({
    selector: 'app-company-setup',
    templateUrl: './company-setup.component.html',
    styleUrls: ['./company-setup.component.css'],
    standalone: false,
})
export class CompanySetupComponent implements OnInit {
    companyForm: FormGroup;
    businessNatures: BusinessNature[] = [];
    isLoading = false;
    isSubmitting = false;
    selectedBusinessNature?: BusinessNature;
    isEditing = false;
    currentCompany: any = null;
    currentUserIsSuperAdmin = false;

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private businessNatureService: BusinessNatureService,
        private companyService: CompanyService,
        private authService: AuthService,
        private notificationService: NotificationService
    ) {
        this.companyForm = this.fb.group({
            businessNatureId: ['', Validators.required],
            companyName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(200)]],
            address: ['', Validators.maxLength(500)],
            phone: ['', Validators.maxLength(50)],
            email: ['', [Validators.email, Validators.maxLength(200)]],
            contactPerson: ['', Validators.maxLength(100)],
            customFields: [''],
        });
    }

    ngOnInit(): void {
        this.currentUserIsSuperAdmin = this.authService.isSuperAdmin();
        this.loadBusinessNatures();
        this.loadCurrentCompany();
    }

    loadBusinessNatures(): void {
        this.isLoading = true;
        this.businessNatureService.getAll().subscribe({
            next: (businessNatures) => {
                this.businessNatures = businessNatures;
                this.isLoading = false;
            },
            error: (error) => {
                console.error('Error loading business natures:', error);
                this.notificationService.showError('Error loading business types');
                this.isLoading = false;
            },
        });
    }

    loadCurrentCompany(): void {
        // Try to load existing company first (for both super admins and regular users)
        this.companyService.getCurrentCompany().subscribe({
            next: (company) => {
                if (company) {
                    this.currentCompany = company;
                    this.isEditing = true;
                    this.populateForm(company);
                } else {
                    // No company exists, allow creation (only super admins can create new companies)
                    this.isEditing = false;
                }
            },
            error: (error) => {
                console.error('Error loading current company:', error);
                // If company loading fails, allow creation only for super admins
                this.isEditing = false;
            },
        });
    }

    populateForm(company: any): void {
        this.companyForm.patchValue({
            businessNatureId: company.businessNatureId || '',
            companyName: company.name || '',
            address: company.address || '',
            phone: company.phone || '',
            email: company.email || '',
            contactPerson: company.contactPerson || '',
        });
        this.onBusinessNatureChange();
    }

    onBusinessNatureChange(): void {
        const businessNatureId = this.companyForm.get('businessNatureId')?.value;
        this.selectedBusinessNature = this.businessNatures.find((bn) => bn.id === parseInt(businessNatureId));
    }

    onSubmit(): void {
        if (this.companyForm.valid && !this.isSubmitting) {
            this.isSubmitting = true;

            if (this.isEditing && this.currentCompany) {
                this.updateCompany();
            } else if (this.currentUserIsSuperAdmin) {
                // Only super admins can create new companies
                this.createCompany();
            } else {
                // Regular users should not be able to create companies
                this.notificationService.showError('You do not have permission to create companies.');
                this.isSubmitting = false;
            }
        } else {
            this.markFormGroupTouched();
        }
    }

    createCompany(): void {
        const request: CreateCompanyProfileRequest = this.companyForm.value;

        this.companyService.createProfile(request).subscribe({
            next: (profile) => {
                console.log('Company profile created:', profile);
                this.notificationService.showSuccess('Company created successfully!');
                this.router.navigate(['/dashboard']);
            },
            error: (error) => {
                console.error('Error creating company profile:', error);
                this.notificationService.showError(
                    'Error creating company: ' + (error.error?.message || error.message)
                );
                this.isSubmitting = false;
            },
        });
    }

    updateCompany(): void {
        console.log('CompanyForm: ', this.companyForm)
        const updateRequest = {
            id: this.currentCompany.id,
            name: this.companyForm.value.companyName,
            address: this.companyForm.value.address,
            phone: this.companyForm.value.phone,
            email: this.companyForm.value.email,
            contactPerson: this.companyForm.value.contactPerson,
            businessNatureId: parseInt(this.companyForm.value.businessNatureId),
            isActive: true,
        };

        this.companyService.update(this.currentCompany.id, updateRequest).subscribe({
            next: (updatedCompany) => {
                console.log('Company profile updated:', updatedCompany);
                this.notificationService.showSuccess('Company updated successfully!');
                this.currentCompany = updatedCompany;
                this.isSubmitting = false;
            },
            error: (error) => {
                console.error('Error updating company profile:', error);
                this.notificationService.showError(
                    'Error updating company: ' + (error.error?.message || error.message)
                );
                this.isSubmitting = false;
            },
        });
    }

    private markFormGroupTouched(): void {
        Object.keys(this.companyForm.controls).forEach((key) => {
            const control = this.companyForm.get(key);
            control?.markAsTouched();
        });
    }

    resetForm(): void {
        this.companyForm.reset();
        this.selectedBusinessNature = undefined;

        // Reset form to initial state with default values
        this.companyForm.patchValue({
            businessNatureId: '',
            companyName: '',
            address: '',
            phone: '',
            email: '',
            contactPerson: '',
        });

        // Clear all validation states
        this.companyForm.markAsUntouched();
        this.companyForm.markAsPristine();

        // If editing, reload the current company data
        if (this.isEditing && this.currentCompany) {
            this.populateForm(this.currentCompany);
        }
    }

    get f() {
        return this.companyForm.controls;
    }
}
