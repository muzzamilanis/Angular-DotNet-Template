import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { CompanyService } from '../../services/company.service';
import { ThemeService } from '../../services/theme.service';

@Component({
    selector: 'app-main-layout',
    templateUrl: './main-layout.component.html',
    styleUrls: ['./main-layout.component.css'],
    standalone: false,
})
export class MainLayoutComponent implements OnInit {
    sidebarCollapsed = false;
    currentUser: any = null; // Add this property
    isLoadingUser = true; // Add loading state
    currentCompany: any = null;
    companyLogo: string | null = null;

    // Track open/closed state of menus
    openMenus: { [key: string]: boolean } = {};

    constructor(
        private router: Router,
        private authService: AuthService,
        private notificationService: NotificationService,
        private companyService: CompanyService,
        private themeService: ThemeService
    ) {}

    ngOnInit(): void {
        this.loadCurrentUser(); // Add this method call
        this.loadCurrentCompany();
    }

    // Add this new method to load current user
    loadCurrentUser(): void {
        this.authService.getCurrentLoginInformation().subscribe({
            next: (response) => {
                if (response.success) {
                    this.currentUser = response.result.user;
                }
                this.isLoadingUser = false;
            },
            error: (error) => {
                console.error('Error loading current user:', error);
                this.isLoadingUser = false;
            },
        });
    }

    // Load current company for branding
    loadCurrentCompany(): void {
        this.companyService.getCurrentCompany().subscribe({
            next: (company) => {
                if (company) {
                    this.currentCompany = company;
                    this.companyLogo = company.logoUrl;
                    
                    // Apply company theme if available
                    if (company.primaryColor) {
                        this.themeService.setTheme(company.primaryColor);
                    }
                }
            },
            error: (error) => {
                console.log('No company found or error loading company:', error);
            },
        });
    }

    // Add method to get display name
    getUserDisplayName(): string {
        if (this.currentUser) {
            return this.currentUser.name || this.currentUser.userName || 'User';
        }
        return 'Loading...';
    }

    // Add method to get user email
    getUserEmail(): string {
        if (this.currentUser) {
            return this.currentUser.emailAddress || this.currentUser.email || '';
        }
        return '';
    }

    // Get company name for display
    getCompanyName(): string | null {
        return this.currentCompany?.name || null;
    }

    // Get company logo URL
    getCompanyLogo(): string | null {
        return this.companyLogo;
    }

    // Add method to check if user is super admin
    isSuperAdmin(): boolean {
        return this.authService.isSuperAdmin();
    }

    // Add method to check if user is writer
    isWriter(): boolean {
        return this.authService.isWriter();
    }

    toggleSidebar(): void {
        this.sidebarCollapsed = !this.sidebarCollapsed;
    }

    // Enhanced version with better UX
    logout(): void {
        this.notificationService
            .showConfirm(
                'Are you sure you want to logout? Your current session will be terminated.',
                'Logout Confirmation'
            )
            .then((confirmed) => {
                if (confirmed) {
                    // Positive case - User confirmed logout
                    this.performLogout();
                } else {
                    // Negative case - User cancelled logout
                    this.handleLogoutCancellation();
                }
            })
            .catch((error) => {
                // Error case - Dialog error
                console.error('Confirmation dialog error:', error);
                this.notificationService.showError('Unable to show logout confirmation', 'Error');
            });
    }

    private performLogout(): void {
        console.log('Performing logout...');

        // Show loading state
        this.isLoadingUser = true;

        // Optional: Show loading notification
        this.notificationService.showInfo('Signing you out...', 'Please wait');

        this.authService.logout().subscribe({
            next: (result) => {
                console.log('Logout successful:', result);

                // Show success message
                this.notificationService.showSuccess(
                    'You have been successfully logged out. Thank you for using our application!',
                    'Goodbye! 👋'
                );

                // Clear user data and navigate
                this.currentUser = null;
                this.isLoadingUser = false;

                // Small delay for better UX
                setTimeout(() => {
                    this.router.navigate(['/login']);
                }, 1000);
            },
            error: (error) => {
                console.error('Logout error:', error);

                // Show warning for forced logout
                this.notificationService.showWarning(
                    'There was an issue during logout, but your session has been cleared for security.',
                    'Session Ended'
                );

                // Force logout
                this.authService.clearToken();
                this.currentUser = null;
                this.isLoadingUser = false;
                this.router.navigate(['/login']);
            },
        });
    }

    private handleLogoutCancellation(): void {
        console.log('Logout cancelled by user');

        // Option 1: Show subtle info message
        this.notificationService.showInfo('Logout cancelled. Welcome back!', 'Stayed Logged In');

        // Option 2: No notification (silent cancellation)
        // Just log it and continue

        // Option 3: Show a friendly message
        // this.notificationService.showSuccess('Great! You\'re still here with us.', 'Logout Cancelled');
    }

    // Toggle menu state and prevent default navigation
    toggleMenuState(menu: string, event: Event): void {
        event.preventDefault();
        this.openMenus[menu] = !this.openMenus[menu];
    }

    // Check if a menu is open
    isMenuOpen(menu: string): boolean {
        return this.openMenus[menu] === true;
    }

    // Add these new methods here:
    getCurrentPageTitle(): string {
        const url = this.router.url;
        if (url.includes('/administration/company-setup')) {
            return 'Company Setup';
        } else if (url.includes('/masters/products')) {
            return 'Product Management';
        } else if (url.includes('/masters/customers')) {
            return 'Customer Management';
        } else if (url.includes('/transactions/sale-orders')) {
            return 'Sale Orders';
        } else if (url.includes('/administration/user-management')) {
            return 'User Management';
        }
        return 'Dashboard';
    }

    shouldShowBreadcrumb(): boolean {
        const url = this.router.url;
        // return !url.includes('/administration/company-setup'); // Hide breadcrumb on setup pages
        return true;
    }

    // Get current year for dynamic copyright display
    getCurrentYear(): number {
        return new Date().getFullYear();
    }
}
