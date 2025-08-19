import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ValidatorFn, AbstractControl, ValidationErrors, FormArray } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router'; // <-- add
import { SaleOrderService } from '../../services/sale-order.service';
import { UserService } from '../../services/user.service';
import { ProductService } from '../../services/product.service';
import { CompanyService } from '../../services/company.service';
import { ProjectService } from '../../services/project.service';
import { ServiceTypeService } from '../../services/service-type.service';
import { CurrencyService } from '../../services/currency.service';
import { Product } from '../../models/product.model';
import { ServiceType } from '../../models/service-type.model';
import { SaleOrder, CreateSaleOrderDto } from '../../models/sale-order.model';
import { BackendUser, UserRole } from '../../models/user.model';
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
    currentOrder: SaleOrder | null = null;
    currentCompany: Company | null = null;
    isDigitalMarketing = false;

    // Writer Portal properties
    writers: BackendUser[] = [];
    selectedFiles: File[] = [];

    // Currency-related properties
    countries: any[] = [];
    selectedCountry = '';
    selectedCurrencyCode = '';
    selectedCurrencySymbol = '';
    currentExchangeRate = 0;
    isLoadingRate = false;

    readonly statusOptions = ['New', 'InProgress', 'Completed', 'Abandon', 'workSubmittedByWriter'];
    readonly paymentStatusOptions = ['Pending', 'PartialPaid', 'FullPaid'];
    readonly sourceOfPaymentOptions = ['Website', 'Payoneer', 'OnlineTransfer'];

    constructor(
        private fb: FormBuilder,
        private saleOrderService: SaleOrderService,
        private userService: UserService,
        private productService: ProductService,
        private companyService: CompanyService,
        private projectService: ProjectService,
        private serviceTypeService: ServiceTypeService,
        private currencyService: CurrencyService,
        private notificationService: NotificationService,
        private route: ActivatedRoute,   // <-- add
        private router: Router           // <-- optional (to clear query params)
    ) {
        this.initializeForm();
        this.initializeCountries();
    }

    ngOnInit(): void {
        this.loadCurrentCompany();
        this.loadProducts();
        this.loadWriters();
        this.loadSaleOrders();
        // do NOT handle params here yet; wait for company/form init inside loadCurrentCompany
    }

    private handleQueryParams(): void {
        this.route.queryParamMap.subscribe(params => {
            const idParam = params.get('id');
            const projectIdParam = params.get('projectId');

            // Prefill project for Digital Marketing flow
            if (projectIdParam && this.isDigitalMarketing) {
                this.saleOrderForm?.get('projectId')?.setValue(+projectIdParam);
            }

            // Open specific order for editing
            if (idParam) {
                const id = +idParam;
                if (!Number.isNaN(id)) {
                    this.saleOrderService.getSaleOrder(id).subscribe({
                        next: (order) => {
                            this.editOrder(order);
                            // optional: clear the query param after loading
                            this.router.navigate([], { queryParams: {}, replaceUrl: true });
                        },
                        error: (err) => {
                            console.error('Failed to load sale order by id from query params:', err);
                            this.notificationService.showError('Unable to load the requested order.');
                        }
                    });
                }
            }
        });
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

                // call handleQueryParams() so the form structure is ready:
                this.handleQueryParams(); // <-- add here
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

    // Group-level validator for payment rules
    private paymentRulesValidator(
        paymentCtrl: string,
        totalPkrCtrl: string,
        ...advanceCtrls: string[]
    ): ValidatorFn {
        return (group: AbstractControl): ValidationErrors | null => {
            const payment = (group.get(paymentCtrl)?.value ?? '').toString().trim().toLowerCase();
            const totalPkr = Number(group.get(totalPkrCtrl)?.value) || 0;
            const totalAdvance = advanceCtrls
                .map(c => Number(group.get(c)?.value) || 0)
                .reduce((a, b) => a + b, 0);

            // Rule 1: Pending cannot have any advance
            if (payment === 'pending' && totalAdvance > 0) {
                return { advancePendingConflict: true };
            }

            // Rule 2: FullPaid requires advance == total (use small epsilon for decimals)
            const approxEqual = totalAdvance < totalPkr;
            console.log('approx equal: ', approxEqual);
            if (payment === 'fullpaid' && approxEqual) {
                return { fullPaidMustEqualTotal: true };
            }

            return null;
        };
    }

    private initializeForm(): void {
        this.saleOrderForm = this.fb.group(
            {
                orderNumber: ['', [Validators.required, Validators.maxLength(50)]],
                orderDate: [new Date().toISOString().substring(0, 10), Validators.required],
                items: this.fb.array([]),
                // Make sure these control names match your form:
                // advanceAmount, advanceAmountPkr (or adjust names below)
                amountReceived: [0],
                amountReceivedInPkr: [0],
                paymentStatus: [this.saleOrderForm?.get('paymentStatus')?.value ?? ''],
                // Make sure these names match your form controls:
                advanceAmount: [this.saleOrderForm?.get('amountReceived')?.value ?? 0],
                advanceAmountPkr: [this.saleOrderForm?.get('amountReceivedInPkr')?.value ?? 0],
                pkrAmount: [this.saleOrderForm?.get('pkrAmount')?.value ?? 0], // total in PKR
            },
            {
                validators: this.paymentRulesValidator(
                    'paymentStatus',
                    'pkrAmount',            // total PKR control (change if yours is different)
                    'amountReceived',        // summed with...
                    'amountReceivedInPkr'      // ...this (adjust names if different)
                ),
            }
        );

        // Re-evaluate when relevant fields change
        ['paymentStatus', 'advanceAmount', 'advanceAmountPkr', 'pkrAmount'].forEach(name => {
            this.saleOrderForm.get(name)?.valueChanges.subscribe(() => {
                this.saleOrderForm.updateValueAndValidity({ onlySelf: false, emitEvent: false });
            });
        });

        // Add Digital Marketing specific fields if company is Digital Marketing
        if (this.isDigitalMarketing) {
            // For digital marketing, we use clientName instead of customerName
            this.saleOrderForm.addControl('clientName', this.fb.control('', [Validators.required, Validators.maxLength(200)]));
            this.saleOrderForm.addControl('projectId', this.fb.control(''));
            this.saleOrderForm.addControl('serviceTypeId', this.fb.control(''));
            this.saleOrderForm.addControl('deadline', this.fb.control(''));
            this.saleOrderForm.addControl('status', this.fb.control('', [Validators.maxLength(50)]));
            this.saleOrderForm.addControl('amountReceived', this.fb.control(0, [Validators.min(0)]));
            this.saleOrderForm.addControl('amountReceivedInPkr', this.fb.control(0, [Validators.min(0)]));
            this.saleOrderForm.addControl('totalPayment', this.fb.control(0, [Validators.min(0)]));
            this.saleOrderForm.addControl('paymentStatus', this.fb.control('', [Validators.maxLength(50)]));
            this.saleOrderForm.addControl('sourceOfPayment', this.fb.control('', [Validators.maxLength(100)]));
            this.saleOrderForm.addControl('assignedTo', this.fb.control('', [Validators.maxLength(200)]));
            this.saleOrderForm.addControl('contactEmail', this.fb.control('', [Validators.email, Validators.maxLength(200)]));
            this.saleOrderForm.addControl('contactPhone', this.fb.control('', [Validators.maxLength(50)]));
            this.saleOrderForm.addControl('orderDetails', this.fb.control('', [Validators.maxLength(1000)]));
            this.saleOrderForm.addControl('wordsOrPages', this.fb.control('', [Validators.maxLength(100)]));

            // Currency-related fields
            this.saleOrderForm.addControl('country', this.fb.control('', [Validators.maxLength(100)]));
            this.saleOrderForm.addControl('currencyCode', this.fb.control('', [Validators.maxLength(10)]));
            this.saleOrderForm.addControl('foreignCurrencyAmount', this.fb.control(0, [Validators.min(0)]));
            this.saleOrderForm.addControl('pkrAmount', this.fb.control(0, [Validators.min(0)]));
            this.saleOrderForm.addControl('exchangeRateUsed', this.fb.control(0));
        } else {
            // For non-digital marketing companies, use traditional fields and items
            this.saleOrderForm.addControl('customerName', this.fb.control('', [Validators.required, Validators.maxLength(200)]));
            this.saleOrderForm.addControl('customerAddress', this.fb.control('', [Validators.maxLength(500)]));
            this.saleOrderForm.addControl('items', this.fb.array([]));
        }
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
        if (!projectId) {
            return '';
        }
        const project = this.projects.find((p) => p.id === projectId);
        return project ? project.projectName : '';
    }

    getServiceTypeName(serviceTypeId: number | null | undefined): string {
        if (!serviceTypeId) {
            return '';
        }
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

    private isAdvancePendingConflict(): boolean {
        return this.saleOrderForm?.hasError('advancePendingConflict') ?? false;
    }
    private hasAdvancePendingConflict(): boolean {
        return this.saleOrderForm?.hasError('advancePendingConflict') ?? false;
    }
    private hasFullPaidMismatch(): boolean {
        return this.saleOrderForm?.hasError('fullPaidMustEqualTotal') ?? false;
    }

    onSubmit(): void {
        if (!this.saleOrderForm) return;

        this.saleOrderForm.updateValueAndValidity();

        if (this.saleOrderForm.invalid || this.hasAdvancePendingConflict() || this.hasFullPaidMismatch()) {
            if (this.hasAdvancePendingConflict()) {
                this.notificationService.showWarning(
                    'Payment status is Pending but an advance amount is greater than 0. Set advance to 0 or change payment status.',
                    'Validation'
                );
            }
            if (this.hasFullPaidMismatch()) {
                this.notificationService.showWarning(
                    'For Full Paid, Advance Amount must be equal to Total Amount (PKR).',
                    'Validation'
                );
            }
            // mark touched for related fields
            ['paymentStatus', 'advanceAmount', 'advanceAmountPkr', 'pkrAmount'].forEach(n =>
                this.saleOrderForm.get(n)?.markAsTouched()
            );
            return;
        }

        const formValue = this.saleOrderForm.getRawValue();
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
            saleOrder.amountReceivedInPkr = formValue.amountReceivedInPkr;
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
        console.log('sale order:', saleOrder);
        if (this.editingOrder) {
            // Update existing order
            this.saleOrderService
                .updateSaleOrder(this.editingOrder.id, {
                    ...saleOrder,
                    id: this.editingOrder.id,
                })
                .subscribe({
                    next: (response) => {
                        // Upload files if any were selected
                        if (this.selectedFiles.length > 0) {
                            this.uploadFiles(response.id).then(() => {
                                this.loadSaleOrders();
                                this.resetForm();
                                this.notificationService.showSuccess('Sale order updated successfully with attachments!');
                            }).catch(() => {
                                this.loadSaleOrders();
                                this.resetForm();
                                this.notificationService.showWarning('Sale order updated but file upload failed.');
                            });
                        } else {
                            this.loadSaleOrders();
                            this.resetForm();
                            this.notificationService.showSuccess('Sale order updated successfully!');
                        }
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
                    // Upload files if any were selected
                    if (this.selectedFiles.length > 0) {
                        this.uploadFiles(response.id).then(() => {
                            this.loadSaleOrders();
                            this.resetForm();
                            this.notificationService.showSuccess('Sale order created successfully with attachments!');
                        }).catch(() => {
                            this.loadSaleOrders();
                            this.resetForm();
                            this.notificationService.showWarning('Sale order created but file upload failed.');
                        });
                    } else {
                        this.loadSaleOrders();
                        this.resetForm();
                        this.notificationService.showSuccess('Sale order created successfully!');
                    }
                },
                error: (error) => {
                    console.error('Error creating sale order:', error);
                    this.notificationService.showError('Error creating sale order. Please try again.');
                },
            });
        }
    }

    editOrder(order: SaleOrder): void {
        this.editingOrder = order;
        this.currentOrder = order;

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
            formValue.amountReceivedInPkr = order.amountReceivedInPkr || 0;
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
                const country = this.countries.find((c) => c.name === order.country);
                if (country) {
                    this.selectedCountry = country.code;
                    this.selectedCurrencyCode = country.currency;
                    this.selectedCurrencySymbol = country.symbol || '';
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
        this.currentOrder = null;
        this.selectedFiles = [];
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
            { name: 'United Arab Emirates', code: 'UAE', currency: 'AED', symbol: 'د.إ' },
            { name: 'Saudi Arabia', code: 'SA', currency: 'SAR', symbol: '﷼' },
            { name: 'United States', code: 'US', currency: 'USD', symbol: '$' },
            { name: 'United Kingdom', code: 'UK', currency: 'GBP', symbol: '£' },
            { name: 'European Union', code: 'EU', currency: 'EUR', symbol: '€' },
            { name: 'Canada', code: 'CA', currency: 'CAD', symbol: 'C$' },
            { name: 'Australia', code: 'AU', currency: 'AUD', symbol: 'A$' },
            { name: 'Switzerland', code: 'CH', currency: 'CHF', symbol: 'CHF' },
            { name: 'China', code: 'CN', currency: 'CNY', symbol: '¥' },
            { name: 'India', code: 'IN', currency: 'INR', symbol: '₹' },
            { name: 'Japan', code: 'JP', currency: 'JPY', symbol: '¥' },
        ];
    }

    onCountryChange(countryCode: string): void {
        const country = this.countries.find((c) => c.code === countryCode);
        if (country) {
            this.selectedCountry = countryCode;
            this.selectedCurrencyCode = country.currency;
            this.selectedCurrencySymbol = country.symbol || '';
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
            },
        });
    }

    onAdvanceAmountChange(): void {
        this.calculateAdvanceInPkrAmount();
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

    private calculateAdvanceInPkrAmount(): void {
        const advanceAmount = this.saleOrderForm.get('amountReceived')?.value || 0;
        const exchangeRate = this.currentExchangeRate || 0;
        const advancePkrAmount = advanceAmount * exchangeRate;
        this.saleOrderForm.get('amountReceivedInPkr')?.setValue(advancePkrAmount.toFixed(2));
    }

    isFormReadonly(): boolean {
        return this.editingOrder?.status?.toLowerCase() === 'completed';
    }

    // Writer Portal methods
    private loadWriters(): void {
        this.userService.getAll().subscribe({
            next: (users) => {
                // Filter users with Writer role
                this.writers = users.filter(user => user.role === UserRole.Writer && user.isActive);
            },
            error: (error) => {
                console.error('Error loading writers:', error);
            }
        });
    }

    onFilesSelected(event: any): void {
        const files = event.target.files;
        if (files) {
            this.selectedFiles = Array.from(files);
        }
    }

    removeFile(index: number): void {
        this.selectedFiles.splice(index, 1);
    }

    formatFileSize(bytes: number): string {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    }

    getAttachmentsList(attachmentFilenames: string | undefined): string[] {
        if (!attachmentFilenames) return [];
        return attachmentFilenames.split(',').filter(name => name.trim());
    }

    downloadAttachment(filename: string): void {
        console.log('Attempting to download file:', filename);
        this.saleOrderService.downloadFile(filename).subscribe({
            next: (blob) => {
                console.log('File download successful, blob size:', blob.size);
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = filename;
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);
            },
            error: (error) => {
                console.error('Error downloading file:', error);
                console.error('Error status:', error.status);
                console.error('Error message:', error.message);
                
                let errorMessage = 'Error downloading file';
                if (error.status === 403) {
                    errorMessage = 'Access denied. You do not have permission to download this file.';
                } else if (error.status === 404) {
                    errorMessage = 'File not found. The attachment may have been deleted.';
                } else if (error.status === 401) {
                    errorMessage = 'Authentication required. Please log in again.';
                } else if (error.message) {
                    errorMessage = `Error downloading file: ${error.message}`;
                }
                
                this.notificationService.showError(errorMessage, 'Download Error');
            }
        });
    }

    getSubmissionStatusClass(submissionStatus: string | undefined): string {
        switch (submissionStatus) {
            case 'Completed': return 'badge-success';
            case 'Need clarity': return 'badge-warning';
            default: return 'badge-secondary';
        }
    }

    private uploadFiles(saleOrderId: number): Promise<void> {
        return new Promise((resolve, reject) => {
            if (this.selectedFiles.length === 0) {
                resolve();
                return;
            }

            this.saleOrderService.uploadAttachments(saleOrderId, this.selectedFiles).subscribe({
                next: (response) => {
                    console.log('Files uploaded successfully:', response);
                    this.selectedFiles = []; // Clear selected files
                    resolve();
                },
                error: (error) => {
                    console.error('Error uploading files:', error);
                    reject(error);
                }
            });
        });
    }
}
