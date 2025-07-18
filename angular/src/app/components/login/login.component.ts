import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: false
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  loading = false;
  error = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      userNameOrEmailAddress: ['', [Validators.required]],
      password: ['', [Validators.required]],
      rememberMe: [false]
    });
  }

  ngOnInit(): void {
    // Redirect to dashboard if already logged in
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/']);
    }
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.loading = true;
      this.error = '';
      
      const loginDto = this.loginForm.value;
      
      this.authService.login(loginDto).subscribe({
        next: (result) => {
          this.loading = false;
          if (result) {
            this.router.navigate(['/']);
          }
        },
        error: (error) => {
          this.loading = false;
          this.error = error.error?.error?.message || 'Login failed. Please try again.';
        }
      });
    }
  }
}