import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BusinessNature, CreateCompanyProfileRequest } from '../../models/multi-tenant.model';
import { BusinessNatureService } from '../../services/business-nature.service';
import { CompanyService } from '../../services/company.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { ThemeService } from '../../services/theme.service';
import { environment } from '../../../environments/environment';

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

    // Logo upload properties
    selectedLogoFile: File | null = null;
    logoPreviewUrl: string | null = null;
    isUploadingLogo = false;

    // Color picker properties
    colorPresets = [
        '#667eea',
        '#764ba2',
        '#ff6b6b',
        '#4ecdc4',
        '#45b7d1',
        '#96ceb4',
        '#ffeaa7',
        '#dda0dd',
        '#98d8c8',
        '#f7dc6f',
        '#bb8fce',
        '#85c1e9',
    ];

    constructor(
        private fb: FormBuilder,
        private router: Router,
        private businessNatureService: BusinessNatureService,
        private companyService: CompanyService,
        private authService: AuthService,
        private notificationService: NotificationService,
        private themeService: ThemeService
    ) {
        this.companyForm = this.fb.group({
            businessNatureId: ['', Validators.required],
            companyName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(200)]],
            address: ['', Validators.maxLength(500)],
            phone: ['', Validators.maxLength(50)],
            email: ['', [Validators.email, Validators.maxLength(200)]],
            contactPerson: ['', Validators.maxLength(100)],
            customFields: [''],
            logoUrl: [''],
            primaryColor: ['#667eea'],
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
            logoUrl: company.logoUrl || '',
            primaryColor: company.primaryColor || '#667eea',
        });

        // Set logo preview if available
        if (company.logoUrl) {
            const companyLogo = this.companyService.getLogo(company.logoUrl).subscribe({
                next: (logo) => {
                    this.logoPreviewUrl = URL.createObjectURL(logo);
                    console.log('Logo URL:', this.logoPreviewUrl);
                },
                error: (error) => {
                    console.error('Error loading logo:', error);
                },
            });
        }

        // Apply company theme
        if (company.primaryColor) {
            this.themeService.setTheme(company.primaryColor);
        }

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
        console.log('CompanyForm: ', this.companyForm);
        const updateRequest = {
            id: this.currentCompany.id,
            name: this.companyForm.value.companyName,
            address: this.companyForm.value.address,
            phone: this.companyForm.value.phone,
            email: this.companyForm.value.email,
            contactPerson: this.companyForm.value.contactPerson,
            businessNatureId: parseInt(this.companyForm.value.businessNatureId),
            logoUrl: this.companyForm.value.logoUrl,
            primaryColor: this.companyForm.value.primaryColor,
            isActive: true,
        };

        this.companyService.update(this.currentCompany.id, updateRequest).subscribe({
            next: (updatedCompany) => {
                console.log('Company profile updated:', updatedCompany);
                this.notificationService.showSuccess('Company updated successfully!');
                this.currentCompany = updatedCompany;

                // Use server-confirmed value to ensure consistency
                this.themeService.setTheme(updatedCompany?.primaryColor || '#667eea');

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

    onLogoFileSelected(event: any): void {
        const file = event.target.files[0];
        if (file) {
            // Validate file type
            const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/svg+xml'];
            if (!allowedTypes.includes(file.type)) {
                this.notificationService.showError('Invalid file type. Please select an image file.');
                return;
            }

            // Validate file size (5MB max)
            if (file.size > 5 * 1024 * 1024) {
                this.notificationService.showError('File size too large. Maximum 5MB allowed.');
                return;
            }

            this.selectedLogoFile = file;

            // Create preview
            const reader = new FileReader();
            reader.onload = (e) => {
                this.logoPreviewUrl = e.target?.result as string;
            };
            reader.readAsDataURL(file);

            // Upload the logo
            this.uploadLogo();
        }
    }

    uploadLogo(): void {
        if (!this.selectedLogoFile) return;

        this.isUploadingLogo = true;
        this.companyService.uploadLogo(this.selectedLogoFile).subscribe({
            next: (response) => {
                this.companyForm.patchValue({ logoUrl: response.logoUrl });
                this.notificationService.showSuccess('Logo uploaded successfully!');
                this.isUploadingLogo = false;
            },
            error: (error) => {
                console.error('Error uploading logo:', error);
                this.notificationService.showError('Error uploading logo: ' + (error.error?.message || error.message));
                this.isUploadingLogo = false;
                this.selectedLogoFile = null;
                this.logoPreviewUrl = null;
            },
        });
    }

    removeLogo(): void {
        this.selectedLogoFile = null;
        this.logoPreviewUrl = null;
        this.companyForm.patchValue({ logoUrl: '' });
    }

    onColorChange(color: string): void {
        this.companyForm.patchValue({ primaryColor: color });
        // Apply theme preview
        this.themeService.setTheme(color);
    }

    selectColorPreset(color: string): void {
        this.onColorChange(color);
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
        this.selectedLogoFile = null;
        this.logoPreviewUrl = null;

        // Reset form to initial state with default values
        this.companyForm.patchValue({
            businessNatureId: '',
            companyName: '',
            address: '',
            phone: '',
            email: '',
            contactPerson: '',
            logoUrl: '',
            primaryColor: '#667eea',
        });

        // Clear all validation states
        this.companyForm.markAsUntouched();
        this.companyForm.markAsPristine();

        // Reset theme to default
        this.themeService.setTheme('#667eea');

        // If editing, reload the current company data
        if (this.isEditing && this.currentCompany) {
            this.populateForm(this.currentCompany);
        }
    }

    get f() {
        return this.companyForm.controls;
    }
}
