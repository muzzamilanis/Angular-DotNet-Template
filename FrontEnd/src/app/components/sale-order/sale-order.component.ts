import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { SaleOrderService } from '../../services/sale-order.service';
import { ProductService } from '../../services/product.service';
import { CompanyService } from '../../services/company.service';
import { ProjectService } from '../../services/project.service';
import { ServiceTypeService } from '../../services/service-type.service';
import { CurrencyService } from '../../services/currency.service';
import { Product } from '../../models/product.model';
import { ServiceType } from '../../models/service-type.model';
import { SaleOrder, CreateSaleOrderDto } from '../../models/sale-order.model';
import { CreateSaleOrderItemDto } from '../../models/sale-order-item.model';
import { PaginatedResponse } from '../../models/common.model';
import { Company } from '../../models/company.model';
import { Project } from '../../models/project.model';
import { NotificationService } from '../../services/notification.service';
import { BusinessNatureType } from '../../models/multi-tenant.model';

@Component({
    selector: 'app-sale-order',
    templateUrl: './sale-order.component.html',
    styleUrls: ['./sale-order.component.css'],
    standalone: false,
})
export class SaleOrderComponent implements OnInit {
    saleOrderForm: FormGroup;
    products: Product[] = [];
    projects: Project[] = [];
    serviceTypes: ServiceType[] = [];
    saleOrders: SaleOrder[] = [];
    editingOrder: SaleOrder | null = null;
    currentCompany: Company | null = null;
    isDigitalMarketing = false;
    
    // Currency-related properties
    countries: any[] = [];
    selectedCountry: string = '';
    selectedCurrencyCode: string = '';
    currentExchangeRate: number = 0;
    isLoadingRate: boolean = false;

    constructor(
        private fb: FormBuilder,
        private saleOrderService: SaleOrderService,
        private productService: ProductService,
        private companyService: CompanyService,
        private projectService: ProjectService,
        private serviceTypeService: ServiceTypeService,
        private currencyService: CurrencyService,
        private notificationService: NotificationService
    ) {
        this.initializeForm();
        this.initializeCountries();
    }

    ngOnInit(): void {
        this.loadCurrentCompany();
        this.loadProducts();
        this.loadSaleOrders();
    }

    private loadCurrentCompany(): void {
        this.companyService.getCurrentCompany().subscribe({
            next: (company) => {
                this.currentCompany = company;
                console.log('Business Nature', BusinessNatureType.DIGITALMARKETINGAGENCY);
                this.isDigitalMarketing = company.businessNatureId === BusinessNatureType.DIGITALMARKETINGAGENCY;
                console.log('Company: ', company);
                console.log('IsDigitalMarketing: ', this.isDigitalMarketing);
                // Reinitialize form with proper structure
                this.initializeForm();

                if (this.isDigitalMarketing) {
                    this.loadProjects();
                    this.loadServiceTypes();
                }

                // Add initial item only for non-digital marketing
                if (!this.isDigitalMarketing) {
                    this.addItem();
                }
            },
            error: (error) => {
                console.error('Error loading current company:', error);
                // Fallback to general mode
                this.isDigitalMarketing = false;
                this.initializeForm();
                this.addItem();
            },
        });
    }

    private loadProjects(): void {
        this.projectService.getAll().subscribe({
            next: (response: PaginatedResponse<any>) => {
                this.projects = response.items || [];
            },
            error: (error) => {
                console.error('Error loading projects:', error);
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

    private initializeForm(): void {
        const formControls: any = {
            orderNumber: ['', [Validators.required, Validators.maxLength(50)]],
            orderDate: [new Date().toISOString().substring(0, 10), Validators.required],
            items: this.fb.array([]),
        };

        // Add Digital Marketing specific fields if company is Digital Marketing
        if (this.isDigitalMarketing) {
            // For digital marketing, we use clientName instead of customerName
            formControls.clientName = ['', [Validators.required, Validators.maxLength(200)]];
            formControls.projectId = [''];
            formControls.serviceTypeId = [''];
            formControls.deadline = [''];
            formControls.status = ['', [Validators.maxLength(50)]];
            formControls.amountReceived = [0, [Validators.min(0)]];
            formControls.totalPayment = [0, [Validators.min(0)]];
            formControls.paymentStatus = ['', [Validators.maxLength(50)]];
            formControls.sourceOfPayment = ['', [Validators.maxLength(100)]];
            formControls.assignedTo = ['', [Validators.maxLength(200)]];
            formControls.contactEmail = ['', [Validators.email, Validators.maxLength(200)]];
            formControls.contactPhone = ['', [Validators.maxLength(50)]];
            formControls.orderDetails = ['', [Validators.maxLength(1000)]];
            formControls.wordsOrPages = ['', [Validators.maxLength(100)]];
            
            // Currency-related fields
            formControls.country = ['', [Validators.maxLength(100)]];
            formControls.currencyCode = ['', [Validators.maxLength(10)]];
            formControls.foreignCurrencyAmount = [0, [Validators.min(0)]];
            formControls.pkrAmount = [{ value: 0, disabled: true }];
            formControls.exchangeRateUsed = [0];
        } else {
            // For non-digital marketing companies, use traditional fields and items
            formControls.customerName = ['', [Validators.required, Validators.maxLength(200)]];
            formControls.customerAddress = ['', [Validators.maxLength(500)]];
            formControls.items = this.fb.array([]);
        }

        this.saleOrderForm = this.fb.group(formControls);
    }

    get items(): FormArray {
        return (this.saleOrderForm.get('items') as FormArray) || this.fb.array([]);
    }

    addItem(): void {
        const itemValue = {
            productId: '',
            quantity: 1,
            date: new Date().toISOString().substring(0, 10),
            price: 0,
            workTitle: '',
            paperType: '',
            serviceType: '',
            wordCount: null,
            pageCount: null,
            itemDeadline: '',
        };

        const itemForm = this.createItemFormGroup(itemValue);
        this.items.push(itemForm);
    }

    removeItem(index: number): void {
        this.items.removeAt(index);
    }

    onProductChange(index: number, productId: number): void {
        const product = this.products.find((p) => p.id === productId);
        if (product) {
            const itemForm = this.items.at(index) as FormGroup;
            itemForm.get('price')?.setValue(product.price);
        }
    }

    getProductName(productId: number): string {
        const product = this.products.find((p) => p.id === productId);
        return product ? product.name : '';
    }

    getProjectName(projectId: number | null | undefined): string {
        if (!projectId) return '';
        const project = this.projects.find((p) => p.id === projectId);
        return project ? project.projectName : '';
    }

    getServiceTypeName(serviceTypeId: number | null | undefined): string {
        if (!serviceTypeId) return '';
        const serviceType = this.serviceTypes.find((st) => st.id === serviceTypeId);
        return serviceType ? serviceType.name : '';
    }

    getLineTotal(index: number): number {
        const itemForm = this.items.at(index) as FormGroup;
        const quantity = itemForm.get('quantity')?.value || 0;
        const price = itemForm.get('price')?.value || 0;
        return quantity * price;
    }

    getTotalAmount(): number {
        let total = 0;
        for (let i = 0; i < this.items.length; i++) {
            total += this.getLineTotal(i);
        }
        return total;
    }

    onSubmit(): void {
        if (this.saleOrderForm.valid) {
            const formValue = this.saleOrderForm.value;
            const saleOrder: CreateSaleOrderDto = {
                orderNumber: formValue.orderNumber,
                orderDate: new Date(formValue.orderDate),
                customerName: this.isDigitalMarketing ? formValue.clientName || '' : formValue.customerName,
                customerAddress: this.isDigitalMarketing ? '' : formValue.customerAddress || '',
                items: this.isDigitalMarketing
                    ? []
                    : formValue.items.map((item: any) => {
                          const baseItem: CreateSaleOrderItemDto = {
                              productId: item.productId,
                              quantity: item.quantity,
                              date: new Date(item.date),
                              price: item.price,
                          };

                          // Add Digital Marketing specific fields if company is Digital Marketing
                          if (this.isDigitalMarketing) {
                              return {
                                  ...baseItem,
                                  workTitle: item.workTitle,
                                  paperType: item.paperType,
                                  serviceType: item.serviceType,
                                  wordCount: item.wordCount,
                                  pageCount: item.pageCount,
                                  itemDeadline: item.itemDeadline ? new Date(item.itemDeadline) : undefined,
                              };
                          }

                          return baseItem;
                      }),
            };

            // Add Digital Marketing specific fields to sale order if company is Digital Marketing
            if (this.isDigitalMarketing) {
                saleOrder.projectId = formValue.projectId || null;
                saleOrder.serviceTypeId = formValue.serviceTypeId || null;
                saleOrder.clientName = formValue.clientName;
                saleOrder.deadline = formValue.deadline ? new Date(formValue.deadline) : undefined;
                saleOrder.status = formValue.status;
                saleOrder.amountReceived = formValue.amountReceived;
                saleOrder.totalPayment = formValue.totalPayment;
                saleOrder.paymentStatus = formValue.paymentStatus;
                saleOrder.sourceOfPayment = formValue.sourceOfPayment;
                saleOrder.contactEmail = formValue.contactEmail;
                saleOrder.contactPhone = formValue.contactPhone;
                saleOrder.assignedTo = formValue.assignedTo;
                saleOrder.orderDetails = formValue.orderDetails;
                saleOrder.wordsOrPages = formValue.wordsOrPages;
                
                // Currency-related fields
                saleOrder.country = formValue.country;
                saleOrder.currencyCode = formValue.currencyCode;
                saleOrder.foreignCurrencyAmount = formValue.foreignCurrencyAmount;
                saleOrder.pkrAmount = formValue.pkrAmount;
                saleOrder.exchangeRateUsed = formValue.exchangeRateUsed;
            }

            if (this.editingOrder) {
                // Update existing order
                this.saleOrderService
                    .updateSaleOrder(this.editingOrder.id, {
                        ...saleOrder,
                        id: this.editingOrder.id,
                    })
                    .subscribe({
                        next: (response) => {
                            this.loadSaleOrders();
                            this.resetForm();
                            this.notificationService.showSuccess('Sale order updated successfully!');
                        },
                        error: (error) => {
                            console.error('Error updating sale order:', error);
                            this.notificationService.showError('Error updating sale order. Please try again.');
                        },
                    });
            } else {
                // Create new order
                this.saleOrderService.createSaleOrder(saleOrder).subscribe({
                    next: (response) => {
                        this.loadSaleOrders();
                        this.resetForm();
                        this.notificationService.showSuccess('Sale order created successfully!');
                    },
                    error: (error) => {
                        console.error('Error creating sale order:', error);
                        this.notificationService.showError('Error creating sale order. Please try again.');
                    },
                });
            }
        }
    }

    editOrder(order: SaleOrder): void {
        this.editingOrder = order;

        const formValue: any = {
            orderNumber: order.orderNumber,
            orderDate: new Date(order.orderDate).toISOString().substring(0, 10),
        };

        // Add Digital Marketing specific fields if company is Digital Marketing
        if (this.isDigitalMarketing) {
            formValue.clientName = order.clientName || order.customerName || '';
            formValue.projectId = order.projectId || '';
            formValue.serviceTypeId = order.serviceTypeId || '';
            formValue.deadline = order.deadline ? new Date(order.deadline).toISOString().substring(0, 10) : '';
            formValue.status = order.status || '';
            formValue.amountReceived = order.amountReceived || 0;
            formValue.totalPayment = order.totalPayment || 0;
            formValue.paymentStatus = order.paymentStatus || '';
            formValue.sourceOfPayment = order.sourceOfPayment || '';
            formValue.contactEmail = order.contactEmail || '';
            formValue.contactPhone = order.contactPhone || '';
            formValue.assignedTo = order.assignedTo || '';
            formValue.orderDetails = order.orderDetails || '';
            formValue.wordsOrPages = order.wordsOrPages || '';
            
            // Currency-related fields
            formValue.country = order.country || '';
            formValue.currencyCode = order.currencyCode || '';
            formValue.foreignCurrencyAmount = order.foreignCurrencyAmount || 0;
            formValue.pkrAmount = order.pkrAmount || 0;
            formValue.exchangeRateUsed = order.exchangeRateUsed || 0;
            
            // Set component properties for currency
            if (order.country) {
                const country = this.countries.find(c => c.name === order.country);
                if (country) {
                    this.selectedCountry = country.code;
                    this.selectedCurrencyCode = country.currency;
                    this.currentExchangeRate = order.exchangeRateUsed || 0;
                }
            }
        } else {
            formValue.customerName = order.customerName;
            formValue.customerAddress = order.customerAddress;
        }

        this.saleOrderForm.patchValue(formValue);

        // Clear existing items
        if (!this.isDigitalMarketing) {
            while (this.items.length !== 0) {
                this.items.removeAt(0);
            }

            // Add items from the order for non-digital marketing
            order.items.forEach((item) => {
                const itemFormValue: any = {
                    productId: item.productId,
                    quantity: item.quantity,
                    date: new Date(item.date).toISOString().substring(0, 10),
                    price: item.price,
                };

                // Add Digital Marketing specific fields if company is Digital Marketing
                if (this.isDigitalMarketing) {
                    itemFormValue.workTitle = item.workTitle || '';
                    itemFormValue.paperType = item.paperType || '';
                    itemFormValue.serviceType = item.serviceType || '';
                    itemFormValue.wordCount = item.wordCount || null;
                    itemFormValue.pageCount = item.pageCount || null;
                    itemFormValue.itemDeadline = item.itemDeadline
                        ? new Date(item.itemDeadline).toISOString().substring(0, 10)
                        : '';
                }

                const itemForm = this.createItemFormGroup(itemFormValue);
                this.items.push(itemForm);
            });
        }
    }

    private createItemFormGroup(itemValue: any): FormGroup {
        const formControls: any = {
            productId: [itemValue.productId, Validators.required],
            quantity: [itemValue.quantity, [Validators.required, Validators.min(1)]],
            date: [itemValue.date, Validators.required],
            price: [itemValue.price, [Validators.required, Validators.min(0)]],
        };

        // Add Digital Marketing specific fields if company is Digital Marketing
        if (this.isDigitalMarketing) {
            formControls.workTitle = [itemValue.workTitle || '', [Validators.maxLength(200)]];
            formControls.paperType = [itemValue.paperType || '', [Validators.maxLength(100)]];
            formControls.serviceType = [itemValue.serviceType || '', [Validators.maxLength(100)]];
            formControls.wordCount = [itemValue.wordCount || null, [Validators.min(0)]];
            formControls.pageCount = [itemValue.pageCount || null, [Validators.min(0)]];
            formControls.itemDeadline = [itemValue.itemDeadline || ''];
        }

        return this.fb.group(formControls);
    }

    deleteOrder(order: SaleOrder): void {
        this.notificationService
            .showConfirm(
                `Are you sure you want to delete order ${order.orderNumber}?`,
                'Delete Sale Order',
                'Yes, delete it!',
                'Cancel'
            )
            .then((confirmed) => {
                if (confirmed) {
                    this.saleOrderService.deleteSaleOrder(order.id).subscribe({
                        next: () => {
                            this.loadSaleOrders();
                            this.notificationService.showSuccess('Sale order deleted successfully!');
                        },
                        error: (error) => {
                            console.error('Error deleting sale order:', error);
                            this.notificationService.showError('Error deleting sale order. Please try again.');
                        },
                    });
                }
            });
    }

    resetForm(): void {
        this.editingOrder = null;
        this.initializeForm();
        // Add one empty item by default only for non-digital marketing
        if (!this.isDigitalMarketing) {
            this.addItem();
        }
    }

    private loadProducts(): void {
        this.productService.getProducts().subscribe({
            next: (response: PaginatedResponse<Product>) => {
                console.log('Loaded products:', JSON.stringify(response));

                // Now TypeScript knows response.items exists and is an array
                this.products = response.items.filter((p) => p.isActive);
            },
            error: (error) => {
                console.error('Error loading products:', error);
            },
        });
    }

    private loadSaleOrders(): void {
        this.saleOrderService.getSaleOrders().subscribe({
            next: (response) => {
                // Check if response has items property (paginated response)
                if (response && response.items && Array.isArray(response.items)) {
                    this.saleOrders = response.items;
                } else if (Array.isArray(response)) {
                    // If response is already an array
                    this.saleOrders = response;
                } else {
                    // Fallback to empty array
                    console.error('Unexpected response format:', response);
                    this.saleOrders = [];
                }
            },
            error: (error) => {
                console.error('Error loading sale orders:', error);
            },
        });
    }

    private initializeCountries(): void {
        this.countries = [
            { name: 'United Arab Emirates', code: 'UAE', currency: 'AED' },
            { name: 'Saudi Arabia', code: 'SA', currency: 'SAR' },
            { name: 'United States', code: 'US', currency: 'USD' },
            { name: 'United Kingdom', code: 'UK', currency: 'GBP' },
            { name: 'European Union', code: 'EU', currency: 'EUR' },
            { name: 'Canada', code: 'CA', currency: 'CAD' },
            { name: 'Australia', code: 'AU', currency: 'AUD' },
            { name: 'Switzerland', code: 'CH', currency: 'CHF' },
            { name: 'China', code: 'CN', currency: 'CNY' },
            { name: 'India', code: 'IN', currency: 'INR' },
            { name: 'Japan', code: 'JP', currency: 'JPY' }
        ];
    }

    onCountryChange(countryCode: string): void {
        const country = this.countries.find(c => c.code === countryCode);
        if (country) {
            this.selectedCountry = countryCode;
            this.selectedCurrencyCode = country.currency;
            this.saleOrderForm.get('country')?.setValue(country.name);
            this.saleOrderForm.get('currencyCode')?.setValue(country.currency);
            this.loadExchangeRate(country.currency);
        }
    }

    private loadExchangeRate(currencyCode: string): void {
        if (!currencyCode || currencyCode === 'PKR') {
            this.currentExchangeRate = 1;
            this.saleOrderForm.get('exchangeRateUsed')?.setValue(1);
            this.calculatePkrAmount();
            return;
        }

        this.isLoadingRate = true;
        this.currencyService.getExchangeRate(currencyCode).subscribe({
            next: (response) => {
                this.currentExchangeRate = response.exchangeRate;
                this.saleOrderForm.get('exchangeRateUsed')?.setValue(response.exchangeRate);
                this.calculatePkrAmount();
                this.isLoadingRate = false;
            },
            error: (error) => {
                console.error('Error loading exchange rate:', error);
                this.notificationService.showError('Failed to load exchange rate. Please try again.', 'Error');
                this.isLoadingRate = false;
            }
        });
    }

    onForeignAmountChange(): void {
        this.calculatePkrAmount();
    }

    private calculatePkrAmount(): void {
        const foreignAmount = this.saleOrderForm.get('foreignCurrencyAmount')?.value || 0;
        const exchangeRate = this.currentExchangeRate || 0;
        const pkrAmount = foreignAmount * exchangeRate;
        this.saleOrderForm.get('pkrAmount')?.setValue(pkrAmount.toFixed(2));
    }

    isFormReadonly(): boolean {
        return this.editingOrder?.status?.toLowerCase() === 'completed';
    }
}
