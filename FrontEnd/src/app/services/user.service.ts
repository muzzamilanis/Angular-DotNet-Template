import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
    User,
    CreateUserDto,
    UpdateUserDto,
    ChangePasswordDto,
    UserRole,
    BackendUser,
    CreateBackendUserDto,
    UpdateBackendUserDto,
} from '../models/user.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class UserService {
    private apiUrl = `${environment.apiUrl}/api/Users`;

    constructor(private http: HttpClient) {}

    getAll(): Observable<BackendUser[]> {
        return this.http.get<BackendUser[]>(this.apiUrl);
    }

    getById(id: number): Observable<BackendUser> {
        return this.http.get<BackendUser>(`${this.apiUrl}/${id}`);
    }

    create(user: CreateBackendUserDto): Observable<BackendUser> {
        return this.http.post<BackendUser>(this.apiUrl, user);
    }

    update(user: UpdateBackendUserDto): Observable<BackendUser> {
        return this.http.put<BackendUser>(`${this.apiUrl}/${user.id}`, user);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

    activate(id: number): Observable<BackendUser> {
        return this.http.post<BackendUser>(`${this.apiUrl}/${id}/activate`, {});
    }

    deactivate(id: number): Observable<BackendUser> {
        return this.http.post<BackendUser>(`${this.apiUrl}/${id}/deactivate`, {});
    }

    resetPassword(userId: number, newPassword: string): Observable<void> {
        const resetDto = {
            userId: userId,
            newPassword: newPassword,
        };
        return this.http.post<void>(`${this.apiUrl}/${userId}/reset-password`, resetDto);
    }
}
