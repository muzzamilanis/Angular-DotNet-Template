import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PropertyService, PropertySaleService } from '../../../services/property.service';
import { Property, PropertySale, CreatePropertySaleDto } from '../../../models/property.model';
import { NotificationService } from '../../../services/notification.service';

@Component({
    selector: 'app-property-sale',
    templateUrl: './property-sale.component.html',
    styleUrls: ['./property-sale.component.css'],
    standalone: false,
})
export class PropertySaleComponent implements OnInit {
    saleForm: FormGroup;
    sales: PropertySale[] = [];
    properties: Property[] = [];
    editingSale: PropertySale | null = null;
    loading = false;

    paymentStatuses = ['Pending', 'Partial', 'Completed'];
    paymentMethods = ['Cash', 'Bank', 'Cheque'];

    constructor(
        private fb: FormBuilder,
        private propertyService: PropertyService,
        private saleService: PropertySaleService,
        private notificationService: NotificationService
    ) {
        this.initializeForm();
    }

    ngOnInit(): void {
        this.loadProperties();
        this.loadSales();
    }

    private initializeForm(): void {
        this.saleForm = this.fb.group({
            propertyId: [null, [Validators.required]],
            saleNumber: ['', [Validators.required, Validators.maxLength(50)]],
            buyerName: ['', [Validators.required, Validators.maxLength(200)]],
            buyerAddress: ['', [Validators.maxLength(500)]],
            buyerPhone: ['', [Validators.maxLength(20)]],
            buyerEmail: ['', [Validators.email, Validators.maxLength(100)]],
            saleDate: [new Date().toISOString().split('T')[0], [Validators.required]],
            salePrice: [0, [Validators.required, Validators.min(0)]],
            costPrice: [0, [Validators.required, Validators.min(0)]],
            advanceReceived: [0, [Validators.min(0)]],
            paymentStatus: ['Pending'],
            paymentMethod: ['Cash'],
            chequeNumber: [''],
            bankName: [''],
            notes: [''],
        });

        // Auto-calculate profit/loss and remaining
        this.saleForm.get('salePrice')?.valueChanges.subscribe(() => this.calculateProfitLoss());
        this.saleForm.get('costPrice')?.valueChanges.subscribe(() => this.calculateProfitLoss());
        this.saleForm.get('advanceReceived')?.valueChanges.subscribe(() => this.calculateRemaining());
    }

    calculateProfitLoss(): void {
        const salePrice = this.saleForm.get('salePrice')?.value || 0;
        const costPrice = this.saleForm.get('costPrice')?.value || 0;
        // Display only, backend will calculate
    }

    calculateRemaining(): void {
        const salePrice = this.saleForm.get('salePrice')?.value || 0;
        const advance = this.saleForm.get('advanceReceived')?.value || 0;
        // Display only, backend will calculate
    }

    getProfitLoss(): number {
        const salePrice = this.saleForm.get('salePrice')?.value || 0;
        const costPrice = this.saleForm.get('costPrice')?.value || 0;
        return salePrice - costPrice;
    }

    getRemainingReceivable(): number {
        const salePrice = this.saleForm.get('salePrice')?.value || 0;
        const advance = this.saleForm.get('advanceReceived')?.value || 0;
        return salePrice - advance;
    }

    onSubmit(): void {
        if (this.saleForm.valid) {
            this.loading = true;
            const formValue = this.saleForm.value;
            const saleDto: CreatePropertySaleDto = {
                ...formValue,
                saleDate: new Date(formValue.saleDate),
            };

            if (this.editingSale) {
                this.saleService.updateSale(this.editingSale.id, saleDto).subscribe({
                    next: () => {
                        this.loadSales();
                        this.resetForm();
                        this.loading = false;
                        this.notificationService.showSuccess('Sale updated successfully!');
                    },
                    error: (error) => {
                        console.error('Error updating sale:', error);
                        this.loading = false;
                        this.notificationService.showError('Error updating sale. Please try again.');
                    },
                });
            } else {
                this.saleService.createSale(saleDto).subscribe({
                    next: () => {
                        this.loadSales();
                        this.loadProperties(); // Refresh to update sold properties
                        this.resetForm();
                        this.loading = false;
                        this.notificationService.showSuccess('Sale recorded successfully! Accounting voucher created automatically with profit/loss entry.');
                    },
                    error: (error) => {
                        console.error('Error creating sale:', error);
                        this.loading = false;
                        this.notificationService.showError('Error recording sale. Please try again.');
                    },
                });
            }
        }
    }

    editSale(sale: PropertySale): void {
        this.editingSale = sale;
        this.saleForm.patchValue({
            ...sale,
            saleDate: new Date(sale.saleDate).toISOString().split('T')[0],
        });
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    deleteSale(sale: PropertySale): void {
        this.notificationService
            .showConfirm(
                `Are you sure you want to delete this sale record?`,
                'Delete Sale',
                'Yes, delete it!',
                'Cancel'
            )
            .then((confirmed) => {
                if (confirmed) {
                    this.saleService.deleteSale(sale.id).subscribe({
                        next: () => {
                            this.loadSales();
                            this.loadProperties();
                            this.notificationService.showSuccess('Sale deleted successfully!');
                        },
                        error: (error) => {
                            console.error('Error deleting sale:', error);
                            this.notificationService.showError('Error deleting sale. Please try again.');
                        },
                    });
                }
            });
    }

    resetForm(): void {
        this.editingSale = null;
        this.saleForm.reset({
            paymentStatus: 'Pending',
            paymentMethod: 'Cash',
            saleDate: new Date().toISOString().split('T')[0],
            salePrice: 0,
            costPrice: 0,
            advanceReceived: 0,
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

    private loadSales(): void {
        this.saleService.getSales().subscribe({
            next: (response) => {
                this.sales = response.items || [];
            },
            error: (error) => {
                console.error('Error loading sales:', error);
            },
        });
    }

    getPropertyName(propertyId: number): string {
        const property = this.properties.find(p => p.id === propertyId);
        return property ? property.propertyName : 'Unknown';
    }
}
