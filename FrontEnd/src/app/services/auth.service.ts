import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { LoginDto, AuthenticateResultModel } from '../models/auth.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    private apiUrl = environment.apiUrl;
    private currentUserSubject = new BehaviorSubject<any>(null);
    public currentUser = this.currentUserSubject.asObservable();
    private currentUserInfo: any = null;

    constructor(private http: HttpClient) {
        // Check if user is already logged in
        const token = localStorage.getItem('token');
        if (token) {
            this.currentUserSubject.next({ token });
            // Load current user info
            this.getCurrentLoginInformation().subscribe({
                next: (response) => {
                    if (response.success) {
                        this.currentUserInfo = response.result.user;
                    }
                },
                error: (error) => {
                    console.error('Error loading current user info:', error);
                },
            });
        }
    }

    login(loginDto: LoginDto): Observable<AuthenticateResultModel> {
        return this.http
            .post<any>(`${this.apiUrl}/TokenAuth/authenticate`, {
                usernameOrEmail: loginDto.usernameOrEmail,
                password: loginDto.password,
            })
            .pipe(
                map((response) => {
                    const authResult = response.result;
                    if (authResult && authResult.accessToken) {
                        localStorage.setItem('token', authResult.accessToken);
                        this.currentUserSubject.next({ token: authResult.accessToken });
                        this.currentUserInfo = authResult.user;

                        // Map the response to match the expected interface
                        return {
                            accessToken: authResult.accessToken,
                            encryptedAccessToken: authResult.accessToken, // Using same token for simplicity
                            expireInSeconds: Math.floor(
                                (new Date(authResult.expireTime).getTime() - new Date().getTime()) / 1000
                            ),
                            userId: authResult.user.id,
                        };
                    }
                    throw new Error('Invalid response from server');
                })
            );
    }

    // Logout method
  logout(): Observable<any> {
    // Call backend logout endpoint (if exists)
    return new Observable(observer => {
      try {
        // Clear tokens from storage
        this.clearToken();
        // Optionally call backend logout
        // this.http.post(`${this.apiUrl}/auth/logout`, {}).subscribe();
        
        observer.next({ success: true });
        observer.complete();
      } catch (error) {
        observer.error(error);
      }
    });
  }

  // Clear authentication token
  clearToken(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('token');
    localStorage.removeItem('access_token');
    sessionStorage.removeItem('authToken');
    sessionStorage.removeItem('token');
  }

    isAuthenticated(): boolean {
        return !!localStorage.getItem('token');
    }

    // Get token method (this is safe to use)
    getToken(): string | null {
        return localStorage.getItem('token');
    }

    getCurrentLoginInformation(): Observable<any> {
        return this.http.get(`${this.apiUrl}/session/GetCurrentLoginInformations`).pipe(
            tap((response) => {
                if (response && (response as any).success) {
                    this.currentUserInfo = (response as any).result.user;
                }
            })
        );
    }

    // Helper methods for role-based access control
    isSuperAdmin(): boolean {
        return this.currentUserInfo?.isSuperAdmin === true;
    }

    isWriter(): boolean {
        return this.currentUserInfo?.role === 'Writer' || this.currentUserInfo?.role === 4; // Writer enum value
    }

    isTeamLead(): boolean {
        return this.currentUserInfo?.role === 'Manager' || this.currentUserInfo?.role === 1; // Manager/Team Lead enum value
    }

    getUserRole(): string | number {
        return this.currentUserInfo?.role;
    }

    getCurrentUser(): any {
        return this.currentUserInfo;
    }

    getCurrentUserCompanyId(): number | null {
        return this.currentUserInfo?.companyId ? parseInt(this.currentUserInfo.companyId) : null;
    }
}
