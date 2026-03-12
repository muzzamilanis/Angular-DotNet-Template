import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

// ── Stage definitions ────────────────────────────────────────────────────────

export interface WorkflowStage {
    key: string;
    label: string;
    icon: string;
}

export const WORKFLOW_STAGES: WorkflowStage[] = [
    { key: 'OrderReceived', label: 'Order Received', icon: 'fas fa-inbox' },
    { key: 'AIProcessing', label: 'AI Processing', icon: 'fas fa-robot' },
    { key: 'WriterAssigned', label: 'Writer Assigned', icon: 'fas fa-user-edit' },
    { key: 'InProgress', label: 'In Progress', icon: 'fas fa-pen-nib' },
    { key: 'UnderReview', label: 'Under Review', icon: 'fas fa-search' },
    { key: 'Delivered', label: 'Delivered', icon: 'fas fa-check-double' },
];

// ── DTOs ─────────────────────────────────────────────────────────────────────

export interface WorkflowStatus {
    id: number;
    orderId: number;
    stage: string;
    assignedTo?: string;
    updatedBy: string;
    timestamp: string;
    notes?: string;
}

// ── Service ──────────────────────────────────────────────────────────────────

@Injectable({
    providedIn: 'root',
})
export class OrderWorkflowService {
    private apiUrl = environment.apiUrl + '/orderworkflow';

    // SignalR connection — lazily imported to avoid bundling issues if not installed
    private hubConnection: any = null;
    private activeOrderId: string | null = null;

    constructor(private http: HttpClient) { }

    // ── HTTP Methods ────────────────────────────────────────────────────────────

    getOrderWorkflow(orderId: number): Observable<WorkflowStatus[]> {
        return this.http.get<WorkflowStatus[]>(`${this.apiUrl}/${orderId}`);
    }

    parseOrder(text: string): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/parse-order`, { text });
    }

    updateStatus(payload: {
        orderId: number;
        stage: string;
        updatedBy?: string;
        assignedTo?: string;
        notes?: string;
    }): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/update-status`, payload);
    }

    // ── SignalR Methods ──────────────────────────────────────────────────────────

    startConnection(orderId: string): void {
        // Dynamically import @microsoft/signalr to avoid hard dependency if not installed
        import('@microsoft/signalr').then(({ HubConnectionBuilder, LogLevel }) => {
            if (this.hubConnection && this.activeOrderId === orderId) return; // already connected

            const token = localStorage.getItem('token');
            this.hubConnection = new HubConnectionBuilder()
                .withUrl(`${environment.apiUrl.replace('/api', '')}/hubs/orderworkflow`, {
                    accessTokenFactory: () => token ?? '',
                })
                .withAutomaticReconnect()
                .configureLogging(LogLevel.Warning)
                .build();

            this.hubConnection
                .start()
                .then(() => {
                    this.activeOrderId = orderId;
                    this.hubConnection.invoke('JoinOrderGroup', orderId).catch(console.error);
                })
                .catch((err: any) => console.error('SignalR connection error:', err));
        }).catch(() => {
            console.warn('SignalR package not installed. Run: npm install @microsoft/signalr');
        });
    }

    stopConnection(orderId: string): void {
        if (this.hubConnection) {
            this.hubConnection.invoke('LeaveOrderGroup', orderId).catch(() => { });
            this.hubConnection.stop().catch(() => { });
            this.hubConnection = null;
            this.activeOrderId = null;
        }
    }

    onWorkflowUpdated(callback: (data: any) => void): void {
        if (this.hubConnection) {
            this.hubConnection.on('WorkflowUpdated', callback);
        } else {
            // Retry once the connection is established
            setTimeout(() => this.onWorkflowUpdated(callback), 500);
        }
    }
}
