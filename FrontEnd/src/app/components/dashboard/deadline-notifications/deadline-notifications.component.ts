import { Component, OnInit } from '@angular/core';
import { DeadlineNotificationService, NotificationDto, DashboardNotificationResponseDto } from '../../../services/deadline-notification.service';

@Component({
    selector: 'app-deadline-notifications',
    templateUrl: './deadline-notifications.component.html',
    styleUrls: ['./deadline-notifications.component.css'],
    standalone: false,
})
export class DeadlineNotificationsComponent implements OnInit {
    notifications: NotificationDto[] = [];
    isLoading = false;
    error: string | null = null;
    hasNotifications = false;
    hasCriticalNotifications = false;

    constructor(private deadlineNotificationService: DeadlineNotificationService) {}

    ngOnInit(): void {
        this.loadNotifications();
    }

    loadNotifications(): void {
        this.isLoading = true;
        this.error = null;

        this.deadlineNotificationService.getDashboardNotifications().subscribe({
            next: (response: DashboardNotificationResponseDto) => {
                this.notifications = response.notifications;
                this.hasNotifications = response.totalCount > 0;
                this.hasCriticalNotifications = response.hasCriticalNotifications;
                this.isLoading = false;
            },
            error: (error) => {
                console.error('Error loading notifications:', error);
                this.error = 'Failed to load notifications';
                this.isLoading = false;
            }
        });
    }

    getUrgencyClass(urgencyLevel: string): string {
        switch (urgencyLevel.toLowerCase()) {
            case 'critical':
                return 'alert-danger';
            case 'high':
                return 'alert-warning';
            case 'medium':
                return 'alert-info';
            case 'low':
                return 'alert-light';
            default:
                return 'alert-secondary';
        }
    }

    getUrgencyIcon(urgencyLevel: string): string {
        switch (urgencyLevel.toLowerCase()) {
            case 'critical':
                return 'fas fa-exclamation-triangle';
            case 'high':
                return 'fas fa-exclamation-circle';
            case 'medium':
                return 'fas fa-clock';
            case 'low':
                return 'fas fa-info-circle';
            default:
                return 'fas fa-bell';
        }
    }

    formatDeadline(deadline: string): string {
        if (!deadline) return 'No deadline';
        const date = new Date(deadline);
        return date.toLocaleDateString();
    }

    refreshNotifications(): void {
        this.loadNotifications();
    }
}