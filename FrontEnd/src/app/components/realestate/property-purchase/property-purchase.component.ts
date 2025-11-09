import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PropertyService, PropertyPurchaseService } from '../../../services/property.service';
import { Property, PropertyPurchase, CreatePropertyPurchaseDto } from '../../../models/property.model';
import { NotificationService } from '../../../services/notification.service';

@Component({
    selector: 'app-property-purchase',
    templateUrl: './property-purchase.component.html',
    styleUrls: ['./property-purchase.component.css'],
    standalone: false,
})
export class PropertyPurchaseComponent implements OnInit {
    purchaseForm: FormGroup;
    purchases: PropertyPurchase[] = [];
    properties: Property[] = [];
    editingPurchase: PropertyPurchase | null = null;
    loading = false;

    paymentStatuses = ['Pending', 'Partial', 'Completed'];
    paymentMethods = ['Cash', 'Bank', 'Cheque'];

    constructor(
        private fb: FormBuilder,
        private propertyService: PropertyService,
        private purchaseService: PropertyPurchaseService,
        private notificationService: NotificationService
    ) {
        this.initializeForm();
    }

    ngOnInit(): void {
        this.loadProperties();
        this.loadPurchases();
    }

    private initializeForm(): void {
        this.purchaseForm = this.fb.group({
            propertyId: [null, [Validators.required]],
            purchaseNumber: ['', [Validators.required, Validators.maxLength(50)]],
            sellerName: ['', [Validators.required, Validators.maxLength(200)]],
            sellerAddress: ['', [Validators.maxLength(500)]],
            sellerPhone: ['', [Validators.maxLength(20)]],
            sellerEmail: ['', [Validators.email, Validators.maxLength(100)]],
            purchaseDate: [new Date().toISOString().split('T')[0], [Validators.required]],
            purchasePrice: [0, [Validators.required, Validators.min(0)]],
            advancePayment: [0, [Validators.min(0)]],
            paymentStatus: ['Pending'],
            paymentMethod: ['Cash'],
            chequeNumber: [''],
            bankName: [''],
            notes: [''],
        });

        // Auto-calculate remaining payment
        this.purchaseForm.get('purchasePrice')?.valueChanges.subscribe(() => this.calculateRemaining());
        this.purchaseForm.get('advancePayment')?.valueChanges.subscribe(() => this.calculateRemaining());
    }

    calculateRemaining(): void {
        const price = this.purchaseForm.get('purchasePrice')?.value || 0;
        const advance = this.purchaseForm.get('advancePayment')?.value || 0;
        // Display only, backend will calculate
    }

    getRemainingPayment(): number {
        const price = this.purchaseForm.get('purchasePrice')?.value || 0;
        const advance = this.purchaseForm.get('advancePayment')?.value || 0;
        return price - advance;
    }

    onSubmit(): void {
        if (this.purchaseForm.valid) {
            this.loading = true;
            const formValue = this.purchaseForm.value;
            const purchaseDto: CreatePropertyPurchaseDto = {
                ...formValue,
                purchaseDate: new Date(formValue.purchaseDate),
            };

            if (this.editingPurchase) {
                this.purchaseService.updatePurchase(this.editingPurchase.id, purchaseDto).subscribe({
                    next: () => {
                        this.loadPurchases();
                        this.resetForm();
                        this.loading = false;
                        this.notificationService.showSuccess('Purchase updated successfully!');
                    },
                    error: (error) => {
                        console.error('Error updating purchase:', error);
                        this.loading = false;
                        this.notificationService.showError('Error updating purchase. Please try again.');
                    },
                });
            } else {
                this.purchaseService.createPurchase(purchaseDto).subscribe({
                    next: () => {
                        this.loadPurchases();
                        this.resetForm();
                        this.loading = false;
                        this.notificationService.showSuccess('Purchase recorded successfully! Accounting voucher created automatically.');
                    },
                    error: (error) => {
                        console.error('Error creating purchase:', error);
                        this.loading = false;
                        this.notificationService.showError('Error recording purchase. Please try again.');
                    },
                });
            }
        }
    }

    editPurchase(purchase: PropertyPurchase): void {
        this.editingPurchase = purchase;
        this.purchaseForm.patchValue({
            ...purchase,
            purchaseDate: new Date(purchase.purchaseDate).toISOString().split('T')[0],
        });
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    deletePurchase(purchase: PropertyPurchase): void {
        this.notificationService
            .showConfirm(
                `Are you sure you want to delete this purchase record?`,
                'Delete Purchase',
                'Yes, delete it!',
                'Cancel'
            )
            .then((confirmed) => {
                if (confirmed) {
                    this.purchaseService.deletePurchase(purchase.id).subscribe({
                        next: () => {
                            this.loadPurchases();
                            this.notificationService.showSuccess('Purchase deleted successfully!');
                        },
                        error: (error) => {
                            console.error('Error deleting purchase:', error);
                            this.notificationService.showError('Error deleting purchase. Please try again.');
                        },
                    });
                }
            });
    }

    resetForm(): void {
        this.editingPurchase = null;
        this.purchaseForm.reset({
            paymentStatus: 'Pending',
            paymentMethod: 'Cash',
            purchaseDate: new Date().toISOString().split('T')[0],
            purchasePrice: 0,
            advancePayment: 0,
        });
    }

    private loadProperties(): void {
        this.propertyService.getActiveProperties().subscribe({
            next: (properties) => {
                this.properties = properties;
            },
            error: (error) => {
                console.error('Error loading properties:', error);
            },
        });
    }

    private loadPurchases(): void {
        this.purchaseService.getPurchases().subscribe({
            next: (response) => {
                this.purchases = response.items || [];
            },
            error: (error) => {
                console.error('Error loading purchases:', error);
            },
        });
    }

    getPropertyName(propertyId: number): string {
        const property = this.properties.find(p => p.id === propertyId);
        return property ? property.propertyName : 'Unknown';
    }
}
