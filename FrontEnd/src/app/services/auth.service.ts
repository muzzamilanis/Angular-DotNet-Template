import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { LoginDto, AuthenticateResultModel } from '../models/auth.model';
import { environment } from '../../environments/environment.hmr'; // <-- Add this import

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl + '/TokenAuth';
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
    // Temporary mock authentication for demonstration
    if (loginDto.username === 'admin' && loginDto.password === '123qwe') {
      const mockResult: AuthenticateResultModel = {
        accessToken: 'mock-token-' + Date.now(),
        encryptedAccessToken: 'encrypted-mock-token',
        expireInSeconds: 3600,
        userId: 1
      };
      
      localStorage.setItem('token', mockResult.accessToken);
      this.currentUserSubject.next({ token: mockResult.accessToken });
      
      return new Observable(observer => {
        observer.next(mockResult);
        observer.complete();
      });
    } else {
      return new Observable(observer => {
        observer.error({ error: { message: 'Invalid credentials' } });
      });
    }
    
    // Original implementation (commented for demo):
    /*
    return this.http.post<any>(`${this.apiUrl}/Authenticate`, loginDto)
      .pipe(
        map(response => {
          const authResult = response.result;
          if (authResult && authResult.accessToken) {
            localStorage.setItem('token', authResult.accessToken);
            this.currentUserSubject.next({ token: authResult.accessToken });
          }
          return authResult;
        })
      );
    */
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
}