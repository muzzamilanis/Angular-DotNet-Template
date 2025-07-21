import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Company, CreateCompanyDto, UpdateCompanyDto } from '../../models/company.model';
import { CompanyService } from '../../services/company.service';

@Component({
  selector: 'app-company-setup',
  templateUrl: './company-setup.component.html',
  styleUrls: ['./company-setup.component.css'],
  standalone: false
})
export class CompanySetupComponent implements OnInit {
  companyForm: FormGroup;
  companies: Company[] = [];
  selectedCompany: Company | null = null;
  isEditing = false;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private companyService: CompanyService
  ) {
    this.companyForm = this.createForm();
  }

  ngOnInit(): void {
    this.loadCompanies();
  }

  createForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      code: ['', [Validators.required, Validators.maxLength(20)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required]],
      address: ['', [Validators.required]],
      city: ['', [Validators.required]],
      state: ['', [Validators.required]],
      country: ['', [Validators.required]],
      zipCode: ['', [Validators.required]],
      website: [''],
      taxNumber: [''],
      registrationNumber: [''],
      isActive: [true]
    });
  }

  loadCompanies(): void {
    this.isLoading = true;
    this.companyService.getAll().subscribe({
      next: (companies) => {
        this.companies = companies;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading companies:', error);
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.companyForm.valid) {
      const formValue = this.companyForm.value;
      
      if (this.isEditing && this.selectedCompany) {
        const updateDto: UpdateCompanyDto = {
          id: this.selectedCompany.id!,
          ...formValue
        };
        this.updateCompany(updateDto);
      } else {
        const createDto: CreateCompanyDto = formValue;
        this.createCompany(createDto);
      }
    } else {
      this.markFormGroupTouched();
    }
  }

  createCompany(company: CreateCompanyDto): void {
    this.isLoading = true;
    this.companyService.create(company).subscribe({
      next: (newCompany) => {
        this.companies.push(newCompany);
        this.resetForm();
        this.isLoading = false;
        alert('Company created successfully!');
      },
      error: (error) => {
        console.error('Error creating company:', error);
        this.isLoading = false;
        alert('Error creating company');
      }
    });
  }

  updateCompany(company: UpdateCompanyDto): void {
    this.isLoading = true;
    this.companyService.update(company).subscribe({
      next: (updatedCompany) => {
        const index = this.companies.findIndex(c => c.id === updatedCompany.id);
        if (index !== -1) {
          this.companies[index] = updatedCompany;
        }
        this.resetForm();
        this.isLoading = false;
        alert('Company updated successfully!');
      },
      error: (error) => {
        console.error('Error updating company:', error);
        this.isLoading = false;
        alert('Error updating company');
      }
    });
  }

  editCompany(company: Company): void {
    this.selectedCompany = company;
    this.isEditing = true;
    this.companyForm.patchValue(company);
  }

  deleteCompany(company: Company): void {
    if (confirm(`Are you sure you want to delete ${company.name}?`)) {
      this.companyService.delete(company.id!).subscribe({
        next: () => {
          this.companies = this.companies.filter(c => c.id !== company.id);
          alert('Company deleted successfully!');
        },
        error: (error) => {
          console.error('Error deleting company:', error);
          alert('Error deleting company');
        }
      });
    }
  }

  toggleCompanyStatus(company: Company): void {
    const action = company.isActive ? 'deactivate' : 'activate';
    if (confirm(`Are you sure you want to ${action} ${company.name}?`)) {
      const request = company.isActive 
        ? this.companyService.deactivate(company.id!)
        : this.companyService.activate(company.id!);
      
      request.subscribe({
        next: () => {
          company.isActive = !company.isActive;
          alert(`Company ${action}d successfully!`);
        },
        error: (error) => {
          console.error(`Error ${action}ing company:`, error);
          alert(`Error ${action}ing company`);
        }
      });
    }
  }

  resetForm(): void {
    this.companyForm.reset();
    this.companyForm.patchValue({ isActive: true });
    this.selectedCompany = null;
    this.isEditing = false;
  }

  private markFormGroupTouched(): void {
    Object.keys(this.companyForm.controls).forEach(key => {
      const control = this.companyForm.get(key);
      control?.markAsTouched();
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.companyForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.companyForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return `${fieldName} is required`;
      if (field.errors['email']) return 'Invalid email format';
      if (field.errors['maxlength']) return `${fieldName} is too long`;
    }
    return '';
  }
}