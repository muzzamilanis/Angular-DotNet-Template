import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = localStorage.getItem('authToken') || localStorage.getItem('token');
    
    let clonedReq: HttpRequest<any>;
    
    if (req.body instanceof FormData) {
      // CRITICAL: Do NOT set Content-Type for FormData
      // Browser will automatically set: multipart/form-data; boundary=...
      clonedReq = req.clone({
        setHeaders: token ? { Authorization: `Bearer ${token}` } : {}
      });
    } else {
      // For JSON requests only
      clonedReq = req.clone({
        setHeaders: {
          'Content-Type': 'application/json',
          ...(token ? { Authorization: `Bearer ${token}` } : {})
        }
      });
    }
    
    return next.handle(clonedReq).pipe(
      catchError((error: HttpErrorResponse) => {
        // Handle 401 Unauthorized errors
        const isAuthRequest = req.url.includes('/auth/') || 
                              req.url.includes('/login') || 
                              req.url.includes('/register');

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
}
