import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User, CreateUserDto, UpdateUserDto, ChangePasswordDto, UserRole } from '../models/user.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = `${environment.apiUrl}/services/app/User`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<User[]> {
    // Mock data for demonstration
    const mockUsers: User[] = [
      {
        id: 1,
        userName: 'admin',
        firstName: 'John',
        lastName: 'Admin',
        email: 'admin@acme.com',
        phone: '+1-555-0111',
        isActive: true,
        role: UserRole.SuperAdmin,
        companyId: 1,
        companyName: 'Acme Corporation',
        createdDate: new Date('2024-01-01'),
        lastLoginDate: new Date()
      },
      {
        id: 2,
        userName: 'manager1',
        firstName: 'Sarah',
        lastName: 'Manager',
        email: 'sarah@acme.com',
        phone: '+1-555-0222',
        isActive: true,
        role: UserRole.Manager,
        companyId: 1,
        companyName: 'Acme Corporation',
        createdDate: new Date('2024-01-15')
      },
      {
        id: 3,
        userName: 'user1',
        firstName: 'Mike',
        lastName: 'User',
        email: 'mike@techsolutions.com',
        phone: '+1-555-0333',
        isActive: true,
        role: UserRole.User,
        companyId: 2,
        companyName: 'Tech Solutions Ltd',
        createdDate: new Date('2024-02-01')
      }
    ];
    
    return new Observable(observer => {
      observer.next(mockUsers);
      observer.complete();
    });
    
    // Original implementation (commented for demo):
    // return this.http.get<User[]>(`${this.apiUrl}/GetAll`);
  }

  getById(id: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/Get?id=${id}`);
  }

  getByCompany(companyId: number): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/GetByCompany?companyId=${companyId}`);
  }

  create(user: CreateUserDto): Observable<User> {
    // Mock implementation for demonstration
    const newUser: User = {
      id: Date.now(), // Mock ID
      userName: user.userName,
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      phone: user.phone,
      isActive: true,
      role: user.role,
      companyId: user.companyId,
      createdDate: new Date()
    };
    
    return new Observable(observer => {
      // Simulate API delay
      setTimeout(() => {
        observer.next(newUser);
        observer.complete();
      }, 500);
    });
    
    // Original implementation (commented for demo):
    // return this.http.post<User>(`${this.apiUrl}/Create`, user);
  }

  update(user: UpdateUserDto): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/Update`, user);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Delete?id=${id}`);
  }

  activate(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/Activate`, { id });
  }

  deactivate(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/Deactivate`, { id });
  }

  changePassword(changePasswordDto: ChangePasswordDto): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/ChangePassword`, changePasswordDto);
  }

  resetPassword(userId: number, newPassword: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/ResetPassword`, { userId, newPassword });
  }
}