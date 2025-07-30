import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { Product, CreateProductDto } from '../../models/product.model';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css'],
  standalone: false
})
export class ProductComponent implements OnInit {
  productForm: FormGroup;
  products: Product[] = [];
  editingProduct: Product | null = null;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private notificationService: NotificationService
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  private initializeForm(): void {
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(500)]],
      price: [0, [Validators.required, Validators.min(0)]],
      isActive: [true]
    });
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      this.loading = true;
      const formValue = this.productForm.value;
      
      if (this.editingProduct) {
        // Update existing product
        this.productService.updateProduct(this.editingProduct.id, {
          ...formValue,
          id: this.editingProduct.id
        }).subscribe({
          next: (response) => {
            this.loadProducts();
            this.resetForm();
            this.loading = false;
            this.notificationService.showSuccess('Product updated successfully!');
          },
          error: (error) => {
            console.error('Error updating product:', error);
            this.loading = false;
            this.notificationService.showError('Error updating product. Please try again.');
          }
        });
      } else {
        // Create new product
        const createProductDto: CreateProductDto = formValue;
        this.productService.createProduct(createProductDto).subscribe({
          next: (response) => {
            this.loadProducts();
            this.resetForm();
            this.loading = false;
            this.notificationService.showSuccess('Product created successfully!');
          },
          error: (error) => {
            console.error('Error creating product:', error);
            this.loading = false;
            this.notificationService.showError('Error creating product. Please try again.');
          }
        });
      }
    }
  }

  editProduct(product: Product): void {
    this.editingProduct = product;
    this.productForm.patchValue({
      name: product.name,
      description: product.description,
      price: product.price,
      isActive: product.isActive
    });
  }

  deleteProduct(product: Product): void {
    this.notificationService.showConfirm(
      `Are you sure you want to delete product ${product.name}?`,
      'Delete Product',
      'Yes, delete it!',
      'Cancel'
    ).then((confirmed) => {
      if (confirmed) {
        this.productService.deleteProduct(product.id).subscribe({
          next: () => {
            this.loadProducts();
            this.notificationService.showSuccess('Product deleted successfully!');
          },
          error: (error) => {
            console.error('Error deleting product:', error);
            this.notificationService.showError('Error deleting product. Please try again.');
          }
        });
      }
    });
  }

  resetForm(): void {
    this.editingProduct = null;
    this.productForm.reset({ isActive: true, price: 0 });
  }

  private loadProducts(): void {
    this.productService.getProducts().subscribe({
      next: (response) => {
        this.products = response.items;
      },
      error: (error) => {
        console.error('Error loading products:', error);
      }
    });
  }
}