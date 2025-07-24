import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.css'],
  standalone: false
})
export class MainLayoutComponent implements OnInit {
  sidebarCollapsed = false;
  currentUser: any = null; // Add this property
  isLoadingUser = true; // Add loading state

  // Track open/closed state of menus
  openMenus: {[key: string]: boolean} = {};

  constructor(
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadCurrentUser(); // Add this method call
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
      }
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

  toggleSidebar(): void {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
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
}