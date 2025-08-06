import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ServiceTypeService } from '../../services/service-type.service';
import { ServiceType, CreateServiceTypeDto } from '../../models/service-type.model';
import { NotificationService } from '../../services/notification.service';

@Component({
    selector: 'app-service-type',
    templateUrl: './service-type.component.html',
    styleUrls: ['./service-type.component.css'],
    standalone: false,
})
export class ServiceTypeComponent implements OnInit {
    serviceTypeForm: FormGroup;
    serviceTypes: ServiceType[] = [];
    editingServiceType: ServiceType | null = null;
    loading = false;

    constructor(
        private fb: FormBuilder,
        private serviceTypeService: ServiceTypeService,
        private notificationService: NotificationService
    ) {
        this.initializeForm();
    }

    ngOnInit(): void {
        this.loadServiceTypes();
    }

    private initializeForm(): void {
        this.serviceTypeForm = this.fb.group({
            name: ['', [Validators.required, Validators.maxLength(100)]],
            description: ['', [Validators.maxLength(200)]],
            isActive: [true],
        });
    }

    onSubmit(): void {
        if (this.serviceTypeForm.valid) {
            this.loading = true;
            const formValue = this.serviceTypeForm.value;

            if (this.editingServiceType) {
                // Update existing service type
                this.serviceTypeService
                    .updateServiceType(this.editingServiceType.id, formValue)
                    .subscribe({
                        next: (response) => {
                            this.loadServiceTypes();
                            this.resetForm();
                            this.loading = false;
                            this.notificationService.showSuccess('Service type updated successfully!');
                        },
                        error: (error) => {
                            console.error('Error updating service type:', error);
                            this.loading = false;
                            this.notificationService.showError('Error updating service type. Please try again.');
                        },
                    });
            } else {
                // Create new service type
                const createServiceTypeDto: CreateServiceTypeDto = formValue;
                this.serviceTypeService.createServiceType(createServiceTypeDto).subscribe({
                    next: (response) => {
                        this.loadServiceTypes();
                        this.resetForm();
                        this.loading = false;
                        this.notificationService.showSuccess('Service type created successfully!');
                    },
                    error: (error) => {
                        console.error('Error creating service type:', error);
                        this.loading = false;
                        this.notificationService.showError('Error creating service type. Please try again.');
                    },
                });
            }
        }
    }

    editServiceType(serviceType: ServiceType): void {
        this.editingServiceType = serviceType;
        this.serviceTypeForm.patchValue({
            name: serviceType.name,
            description: serviceType.description,
            isActive: serviceType.isActive,
        });
    }

    deleteServiceType(serviceType: ServiceType): void {
        this.notificationService
            .showConfirm(
                `Are you sure you want to delete service type "${serviceType.name}"?`,
                'Delete Service Type',
                'Yes, delete it!',
                'Cancel'
            )
            .then((confirmed) => {
                if (confirmed) {
                    this.serviceTypeService.deleteServiceType(serviceType.id).subscribe({
                        next: () => {
                            this.loadServiceTypes();
                            this.notificationService.showSuccess('Service type deleted successfully!');
                        },
                        error: (error) => {
                            console.error('Error deleting service type:', error);
                            this.notificationService.showError('Error deleting service type. Please try again.');
                        },
                    });
                }
            });
    }

    resetForm(): void {
        this.editingServiceType = null;
        this.serviceTypeForm.reset({ isActive: true });
    }

    private loadServiceTypes(): void {
        this.serviceTypeService.getServiceTypes().subscribe({
            next: (response) => {
                this.serviceTypes = response.items;
            },
            error: (error) => {
                console.error('Error loading service types:', error);
            },
        });
    }
}