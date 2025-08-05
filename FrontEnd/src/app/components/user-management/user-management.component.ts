import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BackendUser, CreateBackendUserDto, UpdateBackendUserDto } from '../../models/user.model';
import { Company } from '../../models/company.model';
import { UserService } from '../../services/user.service';
import { CompanyService } from '../../services/company.service';
import { NotificationService } from '../../services/notification.service';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-user-management',
    templateUrl: './user-management.component.html',
    styleUrls: ['./user-management.component.css'],
    standalone: false,
})
export class UserManagementComponent implements OnInit {
    userForm: FormGroup;
    users: BackendUser[] = [];
    companies: Company[] = [];
    selectedUser: BackendUser | null = null;
    isEditing = false;
    isLoading = false;
    currentUserIsSuperAdmin = false;

    constructor(
        private fb: FormBuilder,
        private userService: UserService,
        private companyService: CompanyService,
        private notificationService: NotificationService,
        private authService: AuthService
    ) {
        this.userForm = this.createForm();
    }

    ngOnInit(): void {
        this.currentUserIsSuperAdmin = this.authService.isSuperAdmin();
        this.loadUsers();
        this.loadCompanies();
    }

    createForm(): FormGroup {
        return this.fb.group({
            name: ['', [Validators.required, Validators.minLength(2)]],
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required, Validators.minLength(6)]],
            isSuperAdmin: [false],
            companyId: ['', [Validators.required]],
            isActive: [true],
        });
    }

    loadUsers(): void {
        this.isLoading = true;
        this.userService.getAll().subscribe({
            next: (users) => {
                console.log('users: ', users)
                this.users = users;
                this.isLoading = false;
            },
            error: (error) => {
                console.error('Error loading users:', error);
                this.notificationService.showError('Error loading users');
                this.isLoading = false;
            },
        });
    }

    loadCompanies(): void {
        this.companyService.getAll().subscribe({
            next: (companies) => {
                this.companies = companies.filter((c) => c.isActive);
            },
            error: (error) => {
                console.error('Error loading companies:', error);
            },
        });
    }

    onSubmit(): void {
        if (this.userForm.valid) {
            const formValue = this.userForm.value;

            if (this.isEditing && this.selectedUser) {
                const updateDto: UpdateBackendUserDto = {
                    id: this.selectedUser.id!,
                    name: formValue.name,
                    email: formValue.email,
                    isActive: formValue.isActive,
                    isSuperAdmin: formValue.isSuperAdmin,
                    companyId: parseInt(formValue.companyId),
                };
                this.updateUser(updateDto);
            } else {
                const createDto: CreateBackendUserDto = {
                    name: formValue.name,
                    email: formValue.email,
                    password: formValue.password,
                    isSuperAdmin: formValue.isSuperAdmin,
                    companyId: parseInt(formValue.companyId),
                };
                this.createUser(createDto);
            }
        } else {
            this.markFormGroupTouched();
        }
    }

    createUser(user: CreateBackendUserDto): void {
        this.isLoading = true;
        this.userService.create(user).subscribe({
            next: (newUser) => {
                this.users.push(newUser);
                this.resetForm();
                this.isLoading = false;
                this.notificationService.showSuccess('User created successfully!');
            },
            error: (error) => {
                console.error('Error creating user:', error);
                this.isLoading = false;
                this.notificationService.showError('Error creating user: ' + (error.error?.message || error.message));
            },
        });
    }

    updateUser(user: UpdateBackendUserDto): void {
        this.isLoading = true;
        this.userService.update(user).subscribe({
            next: (updatedUser) => {
                const index = this.users.findIndex((u) => u.id === updatedUser.id);
                if (index !== -1) {
                    this.users[index] = updatedUser;
                }
                this.resetForm();
                this.isLoading = false;
                this.notificationService.showSuccess('User updated successfully!');
            },
            error: (error) => {
                console.error('Error updating user:', error);
                this.isLoading = false;
                this.notificationService.showError('Error updating user: ' + (error.error?.message || error.message));
            },
        });
    }

    editUser(user: BackendUser): void {
        this.selectedUser = user;
        this.isEditing = true;
        this.userForm.patchValue({
            name: user.name,
            email: user.email,
            isSuperAdmin: user.isSuperAdmin,
            companyId: user.companyId,
            isActive: user.isActive,
        });
        // Remove password requirement for editing
        this.userForm.get('password')?.clearValidators();
        this.userForm.get('password')?.updateValueAndValidity();
    }

    deleteUser(user: BackendUser): void {
        this.notificationService
            .showConfirm(`Are you sure you want to delete ${user.name}?`, 'Delete User', 'Yes, delete it!', 'Cancel')
            .then((confirmed) => {
                if (confirmed) {
                    this.userService.delete(user.id!).subscribe({
                        next: () => {
                            this.users = this.users.filter((u) => u.id !== user.id);
                            this.notificationService.showSuccess('User deleted successfully!');
                        },
                        error: (error) => {
                            console.error('Error deleting user:', error);
                            this.notificationService.showError(
                                'Error deleting user: ' + (error.error?.message || error.message)
                            );
                        },
                    });
                }
            });
    }

    toggleUserStatus(user: BackendUser): void {
        const action = user.isActive ? 'deactivate' : 'activate';
        this.notificationService
            .showConfirm(
                `Are you sure you want to ${action} ${user.name}?`,
                `${action.charAt(0).toUpperCase() + action.slice(1)} User`,
                `Yes, ${action} it!`,
                'Cancel'
            )
            .then((confirmed) => {
                if (confirmed) {
                    const request = user.isActive
                        ? this.userService.deactivate(user.id!)
                        : this.userService.activate(user.id!);

                    request.subscribe({
                        next: (updatedUser) => {
                            const index = this.users.findIndex((u) => u.id === updatedUser.id);
                            if (index !== -1) {
                                this.users[index] = updatedUser;
                            }
                            this.notificationService.showSuccess(`User ${action}d successfully!`);
                        },
                        error: (error) => {
                            console.error(`Error ${action}ing user:`, error);
                            this.notificationService.showError(
                                `Error ${action}ing user: ` + (error.error?.message || error.message)
                            );
                        },
                    });
                }
            });
    }

    resetPassword(user: BackendUser): void {
        this.notificationService
            .showPrompt(
                'Password must be at least 6 characters long.',
                'Enter new password for the user',
                'Enter new password...',
                'Reset Password',
                'Cancel'
            )
            .then((newPassword) => {
                if (newPassword && newPassword.length >= 6) {
                    this.userService.resetPassword(user.id!, newPassword).subscribe({
                        next: () => {
                            this.notificationService.showSuccess('Password reset successfully!');
                        },
                        error: (error) => {
                            console.error('Error resetting password:', error);
                            this.notificationService.showError(
                                'Error resetting password: ' + (error.error?.message || error.message)
                            );
                        },
                    });
                } else if (newPassword) {
                    this.notificationService.showError('Password must be at least 6 characters long');
                }
            });
    }

    resetForm(): void {
        this.userForm.reset();
        this.userForm.patchValue({
            isSuperAdmin: false,
            isActive: true,
        });
        this.selectedUser = null;
        this.isEditing = false;
        // Re-add password requirement for new users
        this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
        this.userForm.get('password')?.updateValueAndValidity();
    }

    private markFormGroupTouched(): void {
        Object.keys(this.userForm.controls).forEach((key) => {
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
            if (field.errors['minlength'])
                return `${fieldName} must be at least ${field.errors['minlength'].requiredLength} characters`;
        }
        return '';
    }

    getCompanyName(companyId: number | undefined): string {
        if (!companyId) return 'No Company';
        const company = this.companies.find((c) => c.id === companyId);
        return company ? company.name : 'Unknown Company';
    }

    getUserStatusText(user: BackendUser): string {
        return user.isActive ? 'Active' : 'Inactive';
    }

    getUserRoleText(user: BackendUser): string {
        return user.isSuperAdmin ? 'Super Admin' : 'User';
    }
}
