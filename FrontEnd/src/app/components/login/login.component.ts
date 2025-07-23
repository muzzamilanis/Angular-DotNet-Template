import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LoginDto } from '../../models/auth.model';

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
      usernameOrEmail: ['', [Validators.required]],
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
      
      // Transform the form values to match the expected DTO
      const loginDto: LoginDto = {
        usernameOrEmail: this.loginForm.get('usernameOrEmail')?.value,
        password: this.loginForm.get('password')?.value,
        rememberMe: this.loginForm.get('rememberMe')?.value
      };
      
      

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
          console.log(`Eror is : ${this.error}`);
        }
      });
    }
  }
}