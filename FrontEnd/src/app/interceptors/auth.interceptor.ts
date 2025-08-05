import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private router: Router) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // Get token directly from localStorage
        const token = this.getStoredToken();

        // Skip token for login/register requests
        const isAuthRequest =
            request.url.includes('/auth/') || request.url.includes('/login') || request.url.includes('/register');

        if (token && !isAuthRequest) {
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json',
                },
            });
        }

        return next.handle(request).pipe(
            catchError((error: HttpErrorResponse) => {
                // Handle 401 Unauthorized errors
                if (error.status === 401 && !isAuthRequest) {
                    // Clear invalid token
                    localStorage.removeItem('authToken');
                    localStorage.removeItem('token');

                    // Redirect to login
                    this.router.navigate(['/login']);
                }

                return throwError(() => error);
            })
        );
    }

    private getStoredToken(): string | null {
        // Try different possible token keys
        return (
            localStorage.getItem('authToken') ||
            localStorage.getItem('token') ||
            localStorage.getItem('access_token') ||
            sessionStorage.getItem('authToken') ||
            null
        );
    }
}
