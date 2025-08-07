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

export interface TestEmailDto {
    toEmail?: string;
    subject?: string;
    body?: string;
}

export interface ManualNotificationResponseDto {
    message: string;
    ordersFound?: number;
    notificationsSent?: number;
    sentTo?: string;
    sentAt?: string;
    count?: number;
}

@Injectable({
    providedIn: 'root',
})
export class DeadlineNotificationService {
    private apiUrl = `${environment.apiUrl}/notifications`;

    constructor(private http: HttpClient) {}

    getDashboardNotifications(): Observable<DashboardNotificationResponseDto> {
        return this.http.get<DashboardNotificationResponseDto>(`${this.apiUrl}/dashboard`);
    }

    sendDeadlineNotifications(): Observable<ManualNotificationResponseDto> {
        return this.http.post<ManualNotificationResponseDto>(`${this.apiUrl}/send-deadline-notifications`, {});
    }

    testEmail(testEmailDto?: TestEmailDto): Observable<ManualNotificationResponseDto> {
        return this.http.post<ManualNotificationResponseDto>(`${this.apiUrl}/test-email`, testEmailDto || {});
    }
}