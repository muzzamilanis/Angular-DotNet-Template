import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Observable, map, catchError, of } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class SuperAdminGuard implements CanActivate {
    constructor(
        private authService: AuthService,
        private router: Router
    ) {}

    canActivate(): Observable<boolean> {
        // First check if user is authenticated
        if (!this.authService.isAuthenticated()) {
            this.router.navigate(['/login']);
            return of(false);
        }

        // If we have current user info cached, check immediately
        if (this.authService.getCurrentUser()) {
            if (this.authService.isSuperAdmin()) {
                return of(true);
            } else {
                this.router.navigate(['/dashboard']);
                return of(false);
            }
        }

        // Otherwise, fetch current user info from server
        return this.authService.getCurrentLoginInformation().pipe(
            map((response) => {
                if (response.success && response.result.user.isSuperAdmin) {
                    return true;
                } else {
                    this.router.navigate(['/dashboard']);
                    return false;
                }
            }),
            catchError((error) => {
                console.error('Error checking super admin status:', error);
                this.router.navigate(['/login']);
                return of(false);
            })
        );
    }
}
