import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

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

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initializeForms();
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
      contactPerson: ['', [Validators.maxLength(100)]]
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

      const registrationData = {
        user: this.userForm.value,
        company: this.companyForm.value
      };

      // TODO: Call registration service when backend is ready
      console.log('Registration data:', registrationData);
      
      // For now, simulate successful registration
      setTimeout(() => {
        this.loading = false;
        alert('Registration successful! You can now login with your credentials.');
        this.router.navigate(['/login']);
      }, 2000);
    }
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}