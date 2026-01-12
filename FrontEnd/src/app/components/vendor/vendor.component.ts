import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { VendorService } from '../../services/vendor.service';
import { Vendor, CreateVendorDto } from '../../models/vendor.model';
import { NotificationService } from '../../services/notification.service';

@Component({
    selector: 'app-vendor',
    templateUrl: './vendor.component.html',
    styleUrls: ['./vendor.component.css'],
    standalone: false,
})
export class VendorComponent implements OnInit {
    vendorForm: FormGroup;
    vendors: Vendor[] = [];
    editingVendor: Vendor | null = null;
    loading = false;

    constructor(
        private fb: FormBuilder,
        private vendorService: VendorService,
        private notificationService: NotificationService
    ) {
        this.initializeForm();
    }

    ngOnInit(): void {
        this.loadVendors();
    }

    private initializeForm(): void {
        this.vendorForm = this.fb.group({
            name: ['', [Validators.required, Validators.maxLength(200)]],
            address: ['', [Validators.maxLength(500)]],
            phone: ['', [Validators.maxLength(50)]],
            email: ['', [Validators.email, Validators.maxLength(200)]],
            contactPerson: ['', [Validators.maxLength(100)]],
            description: ['', [Validators.maxLength(500)]],
            isActive: [true],
        });
    }

    onSubmit(): void {
        if (this.vendorForm.valid) {
            this.loading = true;
            const formValue = this.vendorForm.value;

            if (this.editingVendor) {
                // Update existing vendor
                this.vendorService
                    .updateVendor(this.editingVendor.id, {
                        ...formValue,
                        id: this.editingVendor.id,
                    })
                    .subscribe({
                        next: (response) => {
                            this.loadVendors();
                            this.resetForm();
                            this.loading = false;
                            this.notificationService.showSuccess('Vendor updated successfully!');
                        },
                        error: (error) => {
                            console.error('Error updating vendor:', error);
                            this.loading = false;
                            this.notificationService.showError('Error updating vendor. Please try again.');
                        },
                    });
            } else {
                // Create new vendor
                const createVendorDto: CreateVendorDto = formValue;
                this.vendorService.createVendor(createVendorDto).subscribe({
                    next: (response) => {
                        this.loadVendors();
                        this.resetForm();
                        this.loading = false;
                        this.notificationService.showSuccess('Vendor created successfully!');
                    },
                    error: (error) => {
                        console.error('Error creating vendor:', error);
                        this.loading = false;
                        this.notificationService.showError('Error creating vendor. Please try again.');
                    },
                });
            }
        }
    }

    editVendor(vendor: Vendor): void {
        this.editingVendor = vendor;
        this.vendorForm.patchValue({
            name: vendor.name,
            address: vendor.address,
            phone: vendor.phone,
            email: vendor.email,
            contactPerson: vendor.contactPerson,
            description: vendor.description,
            isActive: vendor.isActive,
        });
    }

    deleteVendor(vendor: Vendor): void {
        this.notificationService
            .showConfirm(
                `Are you sure you want to delete vendor ${vendor.name}?`,
                'Delete Vendor',
                'Yes, delete it!',
                'Cancel'
            )
            .then((confirmed) => {
                if (confirmed) {
                    this.vendorService.deleteVendor(vendor.id).subscribe({
                        next: () => {
                            this.loadVendors();
                            this.notificationService.showSuccess('Vendor deleted successfully!');
                        },
                        error: (error) => {
                            console.error('Error deleting vendor:', error);
                            this.notificationService.showError('Error deleting vendor. Please try again.');
                        },
                    });
                }
            });
    }

    resetForm(): void {
        this.editingVendor = null;
        this.vendorForm.reset({ isActive: true });
    }

    private loadVendors(): void {
        this.vendorService.getVendors().subscribe({
            next: (response: any) => {
                this.vendors = response.result || response || [];
            },
            error: (error) => {
                console.error('Error loading vendors:', error);
            },
        });
    }
}
