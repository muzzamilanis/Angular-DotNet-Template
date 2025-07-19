import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { SaleOrderService } from '../../services/sale-order.service';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.model';
import { SaleOrder, CreateSaleOrderDto } from '../../models/sale-order.model';
import { CreateSaleOrderItemDto } from '../../models/sale-order-item.model';
import { PaginatedResponse } from '../../models/common.model';

@Component({
  selector: 'app-sale-order',
  templateUrl: './sale-order.component.html',
  styleUrls: ['./sale-order.component.css'],
  standalone: false
})
export class SaleOrderComponent implements OnInit {
  saleOrderForm: FormGroup;
  products: Product[] = [];
  saleOrders: SaleOrder[] = [];
  editingOrder: SaleOrder | null = null;

  constructor(
    private fb: FormBuilder,
    private saleOrderService: SaleOrderService,
    private productService: ProductService
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    this.loadProducts();
    this.loadSaleOrders();
  }

  private initializeForm(): void {
    this.saleOrderForm = this.fb.group({
      orderNumber: ['', [Validators.required, Validators.maxLength(50)]],
      orderDate: [new Date().toISOString().substring(0, 10), Validators.required],
      customerName: ['', [Validators.required, Validators.maxLength(200)]],
      customerAddress: ['', [Validators.maxLength(500)]],
      items: this.fb.array([])
    });
  }

  get items(): FormArray {
    return this.saleOrderForm.get('items') as FormArray;
  }

  addItem(): void {
    const itemForm = this.fb.group({
      productId: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      date: [new Date().toISOString().substring(0, 10), Validators.required],
      price: [0, [Validators.required, Validators.min(0)]]
    });

    this.items.push(itemForm);
  }

  removeItem(index: number): void {
    this.items.removeAt(index);
  }

  onProductChange(index: number, productId: number): void {
    const product = this.products.find(p => p.id === productId);
    if (product) {
      const itemForm = this.items.at(index) as FormGroup;
      itemForm.get('price')?.setValue(product.price);
    }
  }

  getProductName(productId: number): string {
    const product = this.products.find(p => p.id === productId);
    return product ? product.name : '';
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
        customerName: formValue.customerName,
        customerAddress: formValue.customerAddress,
        items: formValue.items.map((item: any) => ({
          productId: item.productId,
          quantity: item.quantity,
          date: new Date(item.date),
          price: item.price
        }))
      };

      if (this.editingOrder) {
        // Update existing order
        this.saleOrderService.updateSaleOrder(this.editingOrder.id, {
          ...saleOrder,
          id: this.editingOrder.id
        }).subscribe({
          next: (response) => {
            this.loadSaleOrders();
            this.resetForm();
            alert('Sale order updated successfully!');
          },
          error: (error) => {
            console.error('Error updating sale order:', error);
            alert('Error updating sale order. Please try again.');
          }
        });
      } else {
        // Create new order
        this.saleOrderService.createSaleOrder(saleOrder).subscribe({
          next: (response) => {
            this.loadSaleOrders();
            this.resetForm();
            alert('Sale order created successfully!');
          },
          error: (error) => {
            console.error('Error creating sale order:', error);
            alert('Error creating sale order. Please try again.');
          }
        });
      }
    }
  }

  editOrder(order: SaleOrder): void {
    this.editingOrder = order;
    this.saleOrderForm.patchValue({
      orderNumber: order.orderNumber,
      orderDate: new Date(order.orderDate).toISOString().substring(0, 10),
      customerName: order.customerName,
      customerAddress: order.customerAddress
    });

    // Clear existing items
    while (this.items.length !== 0) {
      this.items.removeAt(0);
    }

    // Add items from the order
    order.items.forEach(item => {
      const itemForm = this.fb.group({
        productId: [item.productId, Validators.required],
        quantity: [item.quantity, [Validators.required, Validators.min(1)]],
        date: [new Date(item.date).toISOString().substring(0, 10), Validators.required],
        price: [item.price, [Validators.required, Validators.min(0)]]
      });
      this.items.push(itemForm);
    });
  }

  deleteOrder(order: SaleOrder): void {
    if (confirm(`Are you sure you want to delete order ${order.orderNumber}?`)) {
      this.saleOrderService.deleteSaleOrder(order.id).subscribe({
        next: () => {
          this.loadSaleOrders();
          alert('Sale order deleted successfully!');
        },
        error: (error) => {
          console.error('Error deleting sale order:', error);
          alert('Error deleting sale order. Please try again.');
        }
      });
    }
  }

  resetForm(): void {
    this.editingOrder = null;
    this.saleOrderForm.reset();
    this.initializeForm();
    this.addItem(); // Add one empty item by default
  }

  private loadProducts(): void {
    this.productService.getProducts().subscribe({
      next: (response: PaginatedResponse<Product>) => {
        console.log('Loaded products:', JSON.stringify(response));
        
        // Now TypeScript knows response.items exists and is an array
        this.products = response.items.filter(p => p.isActive);
      },
      error: (error) => {
        console.error('Error loading products:', error);
      }
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
      }
    });
  }
}