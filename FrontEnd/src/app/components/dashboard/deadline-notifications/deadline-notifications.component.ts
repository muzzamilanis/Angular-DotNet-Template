import { Component, OnInit } from '@angular/core';
import {
    DeadlineNotificationService,
    NotificationDto,
    DashboardNotificationResponseDto,
    TestEmailDto,
} from '../../../services/deadline-notification.service';
import { AuthService } from '../../../services/auth.service';
import { NotificationService } from '../../../services/notification.service';

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

    // SuperAdmin functionality
    isSendingNotifications = false;
    isTestingEmail = false;
    showTestEmailModal = false;
    testEmailForm: TestEmailDto = {};

    constructor(
        private deadlineNotificationService: DeadlineNotificationService,
        private authService: AuthService,
        private notificationService: NotificationService
    ) {}

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
            },
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

    // SuperAdmin methods
    isSuperAdmin(): boolean {
        return this.authService.isSuperAdmin();
    }

    sendManualNotifications(): void {
        if (!this.isSuperAdmin()) {
            this.notificationService.showError('Access denied. Only SuperAdmins can trigger manual notifications.');
            return;
        }

        this.notificationService
            .showConfirm(
                'This will send deadline notifications to all relevant users. Are you sure you want to proceed?',
                'Send Manual Notifications'
            )
            .then((confirmed) => {
                if (confirmed) {
                    this.isSendingNotifications = true;

                    this.deadlineNotificationService.sendDeadlineNotifications().subscribe({
                        next: (response) => {
                            this.isSendingNotifications = false;
                            this.notificationService.showSuccess(
                                `${response.message}. Orders found: ${response.ordersFound || 0}, Notifications sent: ${response.notificationsSent || 0}`,
                                'Notifications Sent'
                            );
                            // Refresh notifications after sending
                            this.loadNotifications();
                        },
                        error: (error) => {
                            this.isSendingNotifications = false;
                            const errorMessage =
                                error.error?.message || error.message || 'Failed to send notifications';
                            this.notificationService.showError(errorMessage, 'Error Sending Notifications');
                        },
                    });
                }
            });
    }

    openTestEmailModal(): void {
        if (!this.isSuperAdmin()) {
            this.notificationService.showError('Access denied. Only SuperAdmins can test email functionality.');
            return;
        }

        this.testEmailForm = {
            toEmail: '',
            subject: 'Test Email from SmartBizApp',
            body: '',
        };
        this.showTestEmailModal = true;
    }

    closeTestEmailModal(): void {
        this.showTestEmailModal = false;
        this.testEmailForm = {};
    }

    sendTestEmail(): void {
        this.isTestingEmail = true;

        this.deadlineNotificationService.testEmail(this.testEmailForm).subscribe({
            next: (response) => {
                this.isTestingEmail = false;
                this.closeTestEmailModal();
                this.notificationService.showSuccess(
                    `Test email sent successfully to: ${response.sentTo}`,
                    'Test Email Sent'
                );
            },
            error: (error) => {
                this.isTestingEmail = false;
                const errorMessage = error.error?.message || error.message || 'Failed to send test email';
                this.notificationService.showError(errorMessage, 'Error Sending Test Email');
            },
        });
    }
}
