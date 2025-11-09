import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PropertyService } from '../../../services/property.service';
import { Property, CreatePropertyDto } from '../../../models/property.model';
import { NotificationService } from '../../../services/notification.service';

@Component({
    selector: 'app-property-registration',
    templateUrl: './property-registration.component.html',
    styleUrls: ['./property-registration.component.css'],
    standalone: false,
})
export class PropertyRegistrationComponent implements OnInit {
    propertyForm: FormGroup;
    properties: Property[] = [];
    editingProperty: Property | null = null;
    loading = false;

    propertyTypes = ['Land', 'Building', 'Commercial', 'Residential'];
    propertyStatuses = ['Available', 'Sold', 'Rented', 'Under Development'];
    areaUnits = ['sqft', 'sqm', 'acres'];

    constructor(
        private fb: FormBuilder,
        private propertyService: PropertyService,
        private notificationService: NotificationService
    ) {
        this.initializeForm();
    }

    ngOnInit(): void {
        this.loadProperties();
    }

    private initializeForm(): void {
        this.propertyForm = this.fb.group({
            propertyName: ['', [Validators.required, Validators.maxLength(200)]],
            propertyType: ['Land', [Validators.required]],
            address: ['', [Validators.maxLength(500)]],
            city: ['', [Validators.maxLength(100)]],
            state: ['', [Validators.maxLength(100)]],
            zipCode: ['', [Validators.maxLength(20)]],
            totalArea: [0, [Validators.required, Validators.min(0)]],
            areaUnit: ['sqft'],
            numberOfRooms: [0, [Validators.min(0)]],
            numberOfFloors: [0, [Validators.min(0)]],
            hasLawn: [false],
            lawnArea: [0, [Validators.min(0)]],
            hasGallery: [false],
            numberOfGalleries: [0, [Validators.min(0)]],
            customStructureDetails: ['', [Validators.maxLength(2000)]],
            propertyStatus: ['Available'],
            currentValue: [0, [Validators.required, Validators.min(0)]],
            acquisitionDate: [new Date().toISOString().split('T')[0], [Validators.required]],
            description: ['', [Validators.maxLength(1000)]],
        });
    }

    onSubmit(): void {
        if (this.propertyForm.valid) {
            this.loading = true;
            const formValue = this.propertyForm.value;
            const propertyDto: CreatePropertyDto = {
                ...formValue,
                acquisitionDate: new Date(formValue.acquisitionDate),
            };

            if (this.editingProperty) {
                this.propertyService.updateProperty(this.editingProperty.id, propertyDto).subscribe({
                    next: () => {
                        this.loadProperties();
                        this.resetForm();
                        this.loading = false;
                        this.notificationService.showSuccess('Property updated successfully!');
                    },
                    error: (error) => {
                        console.error('Error updating property:', error);
                        this.loading = false;
                        this.notificationService.showError('Error updating property. Please try again.');
                    },
                });
            } else {
                this.propertyService.createProperty(propertyDto).subscribe({
                    next: () => {
                        this.loadProperties();
                        this.resetForm();
                        this.loading = false;
                        this.notificationService.showSuccess('Property registered successfully!');
                    },
                    error: (error) => {
                        console.error('Error creating property:', error);
                        this.loading = false;
                        this.notificationService.showError('Error registering property. Please try again.');
                    },
                });
            }
        }
    }

    editProperty(property: Property): void {
        this.editingProperty = property;
        this.propertyForm.patchValue({
            ...property,
            acquisitionDate: new Date(property.acquisitionDate).toISOString().split('T')[0],
        });
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    deleteProperty(property: Property): void {
        this.notificationService
            .showConfirm(
                `Are you sure you want to delete property "${property.propertyName}"?`,
                'Delete Property',
                'Yes, delete it!',
                'Cancel'
            )
            .then((confirmed) => {
                if (confirmed) {
                    this.propertyService.deleteProperty(property.id).subscribe({
                        next: () => {
                            this.loadProperties();
                            this.notificationService.showSuccess('Property deleted successfully!');
                        },
                        error: (error) => {
                            console.error('Error deleting property:', error);
                            this.notificationService.showError('Error deleting property. Please try again.');
                        },
                    });
                }
            });
    }

    resetForm(): void {
        this.editingProperty = null;
        this.propertyForm.reset({
            propertyType: 'Land',
            areaUnit: 'sqft',
            propertyStatus: 'Available',
            hasLawn: false,
            hasGallery: false,
            numberOfRooms: 0,
            numberOfFloors: 0,
            lawnArea: 0,
            numberOfGalleries: 0,
            totalArea: 0,
            currentValue: 0,
            acquisitionDate: new Date().toISOString().split('T')[0],
        });
    }

    private loadProperties(): void {
        this.propertyService.getProperties().subscribe({
            next: (response) => {
                this.properties = response.items || [];
            },
            error: (error) => {
                console.error('Error loading properties:', error);
            },
        });
    }
}
