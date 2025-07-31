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

    logout(): void {
        localStorage.removeItem('token');
        this.currentUserSubject.next(null);
        this.currentUserInfo = null;
    }

    isAuthenticated(): boolean {
        return !!localStorage.getItem('token');
    }

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

    getCurrentUser(): any {
        return this.currentUserInfo;
    }

    getCurrentUserCompanyId(): number | null {
        return this.currentUserInfo?.companyId ? parseInt(this.currentUserInfo.companyId) : null;
    }
}
