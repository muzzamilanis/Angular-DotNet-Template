import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomerService } from '../../services/customer.service';
import { Customer, CreateCustomerDto } from '../../models/customer.model';
import { NotificationService } from '../../services/notification.service';

@Component({
    selector: 'app-customer',
    templateUrl: './customer.component.html',
    styleUrls: ['./customer.component.css'],
    standalone: false,
})
export class CustomerComponent implements OnInit {
    customerForm: FormGroup;
    customers: Customer[] = [];
    editingCustomer: Customer | null = null;
    loading = false;

    constructor(
        private fb: FormBuilder,
        private customerService: CustomerService,
        private notificationService: NotificationService
    ) {
        this.initializeForm();
    }

    ngOnInit(): void {
        this.loadCustomers();
    }

    private initializeForm(): void {
        this.customerForm = this.fb.group({
            name: ['', [Validators.required, Validators.maxLength(200)]],
            address: ['', [Validators.maxLength(500)]],
            phone: ['', [Validators.maxLength(50)]],
            email: ['', [Validators.email, Validators.maxLength(200)]],
            isActive: [true],
        });
    }

    onSubmit(): void {
        if (this.customerForm.valid) {
            this.loading = true;
            const formValue = this.customerForm.value;

            if (this.editingCustomer) {
                // Update existing customer
                this.customerService
                    .updateCustomer(this.editingCustomer.id, {
                        ...formValue,
                        id: this.editingCustomer.id,
                    })
                    .subscribe({
                        next: (response) => {
                            this.loadCustomers();
                            this.resetForm();
                            this.loading = false;
                            this.notificationService.showSuccess('Customer updated successfully!');
                        },
                        error: (error) => {
                            console.error('Error updating customer:', error);
                            this.loading = false;
                            this.notificationService.showError('Error updating customer. Please try again.');
                        },
                    });
            } else {
                // Create new customer
                const createCustomerDto: CreateCustomerDto = formValue;
                this.customerService.createCustomer(createCustomerDto).subscribe({
                    next: (response) => {
                        this.loadCustomers();
                        this.resetForm();
                        this.loading = false;
                        this.notificationService.showSuccess('Customer created successfully!');
                    },
                    error: (error) => {
                        console.error('Error creating customer:', error);
                        this.loading = false;
                        this.notificationService.showError('Error creating customer. Please try again.');
                    },
                });
            }
        }
    }

    editCustomer(customer: Customer): void {
        this.editingCustomer = customer;
        this.customerForm.patchValue({
            name: customer.name,
            address: customer.address,
            phone: customer.phone,
            email: customer.email,
            isActive: customer.isActive,
        });
    }

    deleteCustomer(customer: Customer): void {
        this.notificationService
            .showConfirm(
                `Are you sure you want to delete customer ${customer.name}?`,
                'Delete Customer',
                'Yes, delete it!',
                'Cancel'
            )
            .then((confirmed) => {
                if (confirmed) {
                    this.customerService.deleteCustomer(customer.id).subscribe({
                        next: () => {
                            this.loadCustomers();
                            this.notificationService.showSuccess('Customer deleted successfully!');
                        },
                        error: (error) => {
                            console.error('Error deleting customer:', error);
                            this.notificationService.showError('Error deleting customer. Please try again.');
                        },
                    });
                }
            });
    }

    resetForm(): void {
        this.editingCustomer = null;
        this.customerForm.reset({ isActive: true });
    }

    private loadCustomers(): void {
        this.customerService.getCustomers().subscribe({
            next: (customers) => {
                this.customers = customers;
            },
            error: (error) => {
                console.error('Error loading customers:', error);
            },
        });
    }
}
