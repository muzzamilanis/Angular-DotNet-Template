import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface StatusBreakdown {
    status: string;
    count: number;
}

export interface RecentOrder {
    id: number;
    orderNumber: string;
    clientName: string;
    status: string;
    orderDate: string;
    paymentStatus: string;
}

export interface DashboardStats {
    totalOrdersThisMonth: number;
    pendingOrders: number;
    deliveredOrders: number;
    revenueThisMonth: number;
    byStatus: StatusBreakdown[];
    recentOrders: RecentOrder[];
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
    private apiUrl = environment.apiUrl + '/dashboard';

    constructor(private http: HttpClient) {}

    getStats(): Observable<DashboardStats> {
        return this.http.get<DashboardStats>(`${this.apiUrl}/stats`);
    }
}
