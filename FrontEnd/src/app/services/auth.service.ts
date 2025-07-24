import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { LoginDto, AuthenticateResultModel } from '../models/auth.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;
  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    // Check if user is already logged in
    const token = localStorage.getItem('token');
    if (token) {
      this.currentUserSubject.next({ token });
    }
  }

  login(loginDto: LoginDto): Observable<AuthenticateResultModel> {
    return this.http.post<any>(`${this.apiUrl}/TokenAuth/authenticate`, {
      usernameOrEmail: loginDto.usernameOrEmail,
      password: loginDto.password
    })
      .pipe(
        map(response => {
          const authResult = response.result;
          if (authResult && authResult.accessToken) {
            localStorage.setItem('token', authResult.accessToken);
            this.currentUserSubject.next({ token: authResult.accessToken });
            
            // Map the response to match the expected interface
            return {
              accessToken: authResult.accessToken,
              encryptedAccessToken: authResult.accessToken, // Using same token for simplicity
              expireInSeconds: Math.floor((new Date(authResult.expireTime).getTime() - new Date().getTime()) / 1000),
              userId: authResult.user.id
            };
          }
          throw new Error('Invalid response from server');
        })
      );
  }

  logout(): void {
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getCurrentLoginInformation(): Observable<any> {
    return this.http.get(`${this.apiUrl}/session/GetCurrentLoginInformations`);
  }
}