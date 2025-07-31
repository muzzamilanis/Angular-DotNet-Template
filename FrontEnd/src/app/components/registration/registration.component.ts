import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { RegistrationService, RegistrationDto } from '../../services/registration.service';
import { NotificationService } from '../../services/notification.service';
import { BusinessNatureService } from '../../services/business-nature.service';
import { BusinessNature } from '../../models/multi-tenant.model';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css'],
  standalone: false
})
export class RegistrationComponent implements OnInit {
  currentStep = 1;
  userForm!: FormGroup;
  companyForm!: FormGroup;
  loading = false;
  error = '';
  businessNatures: BusinessNature[] = [];
  selectedBusinessNature: BusinessNature | null = null;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private registrationService: RegistrationService,
    private router: Router,
    private notificationService: NotificationService,
    private businessNatureService: BusinessNatureService
  ) {}

  ngOnInit(): void {
    this.initializeForms();
    this.loadBusinessNatures();
  }

  private initializeForms(): void {
    // User registration form
    this.userForm = this.formBuilder.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(200)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, {
      validators: this.passwordMatchValidator
    });

    // Company registration form
    this.companyForm = this.formBuilder.group({
      companyName: ['', [Validators.required, Validators.maxLength(200)]],
      address: ['', [Validators.maxLength(500)]],
      phone: ['', [Validators.maxLength(50)]],
      email: ['', [Validators.email, Validators.maxLength(200)]],
      contactPerson: ['', [Validators.maxLength(100)]],
      businessNatureId: ['', [Validators.required]]
    });
  }

  private passwordMatchValidator(group: FormGroup) {
    const password = group.get('password');
    const confirmPassword = group.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    
    return null;
  }

  private loadBusinessNatures(): void {
    this.businessNatureService.getAll().subscribe({
      next: (businessNatures) => {
        this.businessNatures = businessNatures;
      },
      error: (error) => {
        console.error('Error loading business natures:', error);
      }
    });
  }

  onBusinessNatureChange(businessNatureId: string): void {
    const id = Number(businessNatureId);
    this.selectedBusinessNature = this.businessNatures.filter(bn => bn.id === id)[0] || null;
  }

  nextStep(): void {
    if (this.currentStep === 1 && this.userForm.valid) {
      this.currentStep = 2;
      this.error = '';
    }
  }

  previousStep(): void {
    if (this.currentStep === 2) {
      this.currentStep = 1;
      this.error = '';
    }
  }

  onSubmit(): void {
    if (this.currentStep === 1) {
      this.nextStep();
      return;
    }

    if (this.userForm.valid && this.companyForm.valid) {
      this.loading = true;
      this.error = '';

      const registrationData: RegistrationDto = {
        user: {
          name: this.userForm.value.name,
          email: this.userForm.value.email,
          password: this.userForm.value.password
        },
        company: {
          companyName: this.companyForm.value.companyName,
          address: this.companyForm.value.address,
          phone: this.companyForm.value.phone,
          email: this.companyForm.value.email,
          contactPerson: this.companyForm.value.contactPerson,
          businessNatureId: this.companyForm.value.businessNatureId
        }
      };

      this.registrationService.register(registrationData).subscribe({
        next: (response) => {
          this.loading = false;
          if (response.success) {
            this.notificationService.showSuccess(response.message).then(() => {
              this.router.navigate(['/login']);
            });
          } else {
            this.error = response.message;
          }
        },
        error: (error) => {
          this.loading = false;
          console.error('Registration error:', error);
          this.error = error.error?.message || 'An error occurred during registration. Please try again.';
        }
      });
    }
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}