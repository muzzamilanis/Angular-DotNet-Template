import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface NotificationDto {
    saleOrderId: number;
    orderNumber: string;
    clientName?: string;
    deadline?: string;
    daysUntilDeadline: number;
    assignedTo?: string;
    totalAmount: number;
    status?: string;
    urgencyLevel: string;
    message: string;
}

export interface DashboardNotificationResponseDto {
    notifications: NotificationDto[];
    totalCount: number;
    hasCriticalNotifications: boolean;
}

@Injectable({
    providedIn: 'root',
})
export class DeadlineNotificationService {
    private apiUrl = `${environment.apiUrl}/api/notifications`;

    constructor(private http: HttpClient) {}

    getDashboardNotifications(): Observable<DashboardNotificationResponseDto> {
        return this.http.get<DashboardNotificationResponseDto>(`${this.apiUrl}/dashboard`);
    }
}