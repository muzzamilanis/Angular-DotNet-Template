import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Observable, map, catchError, of } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class AdminDashboardGuard implements CanActivate {
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
            if (this.authService.isWriter()) {
                // Redirect writers to their specific dashboard
                this.router.navigate(['/writer/dashboard']);
                return of(false);
            } else if (this.authService.isTeamLead() && !this.authService.isSuperAdmin()) {
                // Redirect team leads to their specific dashboard (unless they are also SuperAdmin)
                this.router.navigate(['/team-lead/dashboard']);
                return of(false);
            } else if (this.authService.getBusinessNatureId() === 6) {
                // Redirect RealEstate users to their specific dashboard
                this.router.navigate(['/realestate/dashboard']);
                return of(false);
            } else {
                return of(true);
            }
        }

        // Otherwise, fetch current user info from server
        return this.authService.getCurrentLoginInformation().pipe(
            map((response) => {
                if (response.success) {
                    const user = response.result.user;
                    if (user.role === 'Writer' || user.role === 4) {
                        // Redirect writers to their specific dashboard
                        this.router.navigate(['/writer/dashboard']);
                        return false;
                    } else if ((user.role === 'Manager' || user.role === 1) && !user.isSuperAdmin) {
                        // Redirect team leads to their specific dashboard (unless they are also SuperAdmin)
                        this.router.navigate(['/team-lead/dashboard']);
                        return false;
                    } else if (user.businessNatureId === 6) {
                        // Redirect RealEstate users to their specific dashboard
                        this.router.navigate(['/realestate/dashboard']);
                        return false;
                    } else {
                        return true;
                    }
                } else {
                    this.router.navigate(['/login']);
                    return false;
                }
            }),
            catchError((error) => {
                console.error('Error checking user role:', error);
                this.router.navigate(['/login']);
                return of(false);
            })
        );
    }
}
