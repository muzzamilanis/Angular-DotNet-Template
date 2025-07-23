import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BusinessNature, CreateCompanyProfileRequest } from '../../models/multi-tenant.model';
import { BusinessNatureService } from '../../services/business-nature.service';
import { CompanyService } from '../../services/company.service';

@Component({
  selector: 'app-company-setup',
  templateUrl: './company-setup.component.html',
  styleUrls: ['./company-setup.component.css'],
  standalone: false
})
export class CompanySetupComponent implements OnInit {
  companyForm: FormGroup;
  businessNatures: BusinessNature[] = [];
  isLoading = false;
  isSubmitting = false;
  selectedBusinessNature?: BusinessNature;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private businessNatureService: BusinessNatureService,
    private companyService: CompanyService
  ) {
    this.companyForm = this.fb.group({
      businessNatureId: ['', Validators.required],
      companyName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(200)]],
      address: ['', Validators.maxLength(500)],
      phone: ['', Validators.maxLength(50)],
      email: ['', [Validators.email, Validators.maxLength(200)]],
      contactPerson: ['', Validators.maxLength(100)], // Add this field
      customFields: ['']
    });
  }

  ngOnInit(): void {
    this.loadBusinessNatures();
  }

  loadBusinessNatures(): void {
    this.isLoading = true;
    this.businessNatureService.getAll().subscribe({
      next: (businessNatures) => {
        this.businessNatures = businessNatures;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading business natures:', error);
        this.isLoading = false;
      }
    });
  }

  onBusinessNatureChange(): void {
    const businessNatureId = this.companyForm.get('businessNatureId')?.value;
    this.selectedBusinessNature = this.businessNatures.find(bn => bn.id === parseInt(businessNatureId));
  }

  onSubmit(): void {
    if (this.companyForm.valid && !this.isSubmitting) {
      this.isSubmitting = true;
      const request: CreateCompanyProfileRequest = this.companyForm.value;
      
      this.companyService.createProfile(request).subscribe({
        next: (profile) => {
          console.log('Company profile created:', profile);
          this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          console.error('Error creating company profile:', error);
          this.isSubmitting = false;
        }
      });
    }
  }

// Add this method to your existing component class
resetForm(): void {
  this.companyForm.reset();
  this.selectedBusinessNature = null;
  
  // Reset form to initial state with default values
  this.companyForm.patchValue({
    businessNatureId: '',
    companyName: '',
    address: '',
    phone: '',
    email: '',
    contactPerson: ''
  });
  
  // Clear all validation states
  this.companyForm.markAsUntouched();
  this.companyForm.markAsPristine();
}

  get f() {
    return this.companyForm.controls;
  }
}