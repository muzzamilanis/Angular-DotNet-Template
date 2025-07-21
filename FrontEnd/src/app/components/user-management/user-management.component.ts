import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { User, CreateUserDto, UpdateUserDto, UserRole } from '../../models/user.model';
import { Company } from '../../models/company.model';
import { UserService } from '../../services/user.service';
import { CompanyService } from '../../services/company.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css'],
  standalone: false
})
export class UserManagementComponent implements OnInit {
  userForm: FormGroup;
  users: User[] = [];
  companies: Company[] = [];
  selectedUser: User | null = null;
  isEditing = false;
  isLoading = false;
  userRoles = Object.values(UserRole);

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private companyService: CompanyService
  ) {
    this.userForm = this.createForm();
  }

  ngOnInit(): void {
    this.loadUsers();
    this.loadCompanies();
  }

  createForm(): FormGroup {
    return this.fb.group({
      userName: ['', [Validators.required, Validators.minLength(3)]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      password: ['', [Validators.required, Validators.minLength(6)]],
      role: [UserRole.User, [Validators.required]],
      companyId: [''],
      isActive: [true]
    });
  }

  loadUsers(): void {
    this.isLoading = true;
    this.userService.getAll().subscribe({
      next: (users) => {
        this.users = users;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.isLoading = false;
      }
    });
  }

  loadCompanies(): void {
    this.companyService.getAll().subscribe({
      next: (companies) => {
        this.companies = companies.filter(c => c.isActive);
      },
      error: (error) => {
        console.error('Error loading companies:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.userForm.valid) {
      const formValue = this.userForm.value;
      
      if (this.isEditing && this.selectedUser) {
        const updateDto: UpdateUserDto = {
          id: this.selectedUser.id!,
          userName: formValue.userName,
          firstName: formValue.firstName,
          lastName: formValue.lastName,
          email: formValue.email,
          phone: formValue.phone,
          isActive: formValue.isActive,
          role: formValue.role,
          companyId: formValue.companyId || null
        };
        this.updateUser(updateDto);
      } else {
        const createDto: CreateUserDto = {
          userName: formValue.userName,
          firstName: formValue.firstName,
          lastName: formValue.lastName,
          email: formValue.email,
          phone: formValue.phone,
          password: formValue.password,
          role: formValue.role,
          companyId: formValue.companyId || null
        };
        this.createUser(createDto);
      }
    } else {
      this.markFormGroupTouched();
    }
  }

  createUser(user: CreateUserDto): void {
    this.isLoading = true;
    this.userService.create(user).subscribe({
      next: (newUser) => {
        this.users.push(newUser);
        this.resetForm();
        this.isLoading = false;
        alert('User created successfully!');
      },
      error: (error) => {
        console.error('Error creating user:', error);
        this.isLoading = false;
        alert('Error creating user');
      }
    });
  }

  updateUser(user: UpdateUserDto): void {
    this.isLoading = true;
    this.userService.update(user).subscribe({
      next: (updatedUser) => {
        const index = this.users.findIndex(u => u.id === updatedUser.id);
        if (index !== -1) {
          this.users[index] = updatedUser;
        }
        this.resetForm();
        this.isLoading = false;
        alert('User updated successfully!');
      },
      error: (error) => {
        console.error('Error updating user:', error);
        this.isLoading = false;
        alert('Error updating user');
      }
    });
  }

  editUser(user: User): void {
    this.selectedUser = user;
    this.isEditing = true;
    this.userForm.patchValue({
      userName: user.userName,
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      phone: user.phone,
      role: user.role,
      companyId: user.companyId,
      isActive: user.isActive
    });
    // Remove password requirement for editing
    this.userForm.get('password')?.clearValidators();
    this.userForm.get('password')?.updateValueAndValidity();
  }

  deleteUser(user: User): void {
    if (confirm(`Are you sure you want to delete ${user.firstName} ${user.lastName}?`)) {
      this.userService.delete(user.id!).subscribe({
        next: () => {
          this.users = this.users.filter(u => u.id !== user.id);
          alert('User deleted successfully!');
        },
        error: (error) => {
          console.error('Error deleting user:', error);
          alert('Error deleting user');
        }
      });
    }
  }

  toggleUserStatus(user: User): void {
    const action = user.isActive ? 'deactivate' : 'activate';
    if (confirm(`Are you sure you want to ${action} ${user.firstName} ${user.lastName}?`)) {
      const request = user.isActive 
        ? this.userService.deactivate(user.id!)
        : this.userService.activate(user.id!);
      
      request.subscribe({
        next: () => {
          user.isActive = !user.isActive;
          alert(`User ${action}d successfully!`);
        },
        error: (error) => {
          console.error(`Error ${action}ing user:`, error);
          alert(`Error ${action}ing user`);
        }
      });
    }
  }

  resetPassword(user: User): void {
    const newPassword = prompt('Enter new password for the user:');
    if (newPassword && newPassword.length >= 6) {
      this.userService.resetPassword(user.id!, newPassword).subscribe({
        next: () => {
          alert('Password reset successfully!');
        },
        error: (error) => {
          console.error('Error resetting password:', error);
          alert('Error resetting password');
        }
      });
    } else if (newPassword) {
      alert('Password must be at least 6 characters long');
    }
  }

  resetForm(): void {
    this.userForm.reset();
    this.userForm.patchValue({ 
      role: UserRole.User,
      isActive: true 
    });
    this.selectedUser = null;
    this.isEditing = false;
    // Re-add password requirement for new users
    this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.userForm.get('password')?.updateValueAndValidity();
  }

  private markFormGroupTouched(): void {
    Object.keys(this.userForm.controls).forEach(key => {
      const control = this.userForm.get(key);
      control?.markAsTouched();
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.userForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.userForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return `${fieldName} is required`;
      if (field.errors['email']) return 'Invalid email format';
      if (field.errors['minlength']) return `${fieldName} must be at least ${field.errors['minlength'].requiredLength} characters`;
    }
    return '';
  }

  getCompanyName(companyId: number | undefined): string {
    if (!companyId) return 'No Company';
    const company = this.companies.find(c => c.id === companyId);
    return company ? company.name : 'Unknown Company';
  }

  getRoleDisplayName(role: UserRole): string {
    switch (role) {
      case UserRole.SuperAdmin: return 'Super Admin';
      case UserRole.CompanyAdmin: return 'Company Admin';
      case UserRole.Manager: return 'Manager';
      case UserRole.User: return 'User';
      default: return role;
    }
  }
}