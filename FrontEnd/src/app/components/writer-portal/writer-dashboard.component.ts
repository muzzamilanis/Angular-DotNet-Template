import { Component, OnInit } from '@angular/core';
import { WriterService } from '../../services/writer.service';
import { AuthService } from '../../services/auth.service';
import { WriterAssignmentDto } from '../../models/sale-order.model';

@Component({
    selector: 'app-writer-dashboard',
    templateUrl: './writer-dashboard.component.html',
    styleUrls: ['./writer-dashboard.component.css'],
    standalone: false,
})
export class WriterDashboardComponent implements OnInit {
    currentUser: any = null;
    isLoading = true;
    assignments: WriterAssignmentDto[] = [];
    assignmentStats = {
        total: 0,
        pending: 0,
        inProgress: 0,
        completed: 0,
        overdue: 0,
    };

    constructor(
        private writerService: WriterService,
        private authService: AuthService
    ) {}

    ngOnInit(): void {
        this.loadCurrentUser();
        this.loadAssignments();
    }

    loadCurrentUser(): void {
        this.authService.getCurrentLoginInformation().subscribe({
            next: (response) => {
                if (response.success) {
                    this.currentUser = response.result.user;
                }
            },
            error: (error) => {
                console.error('Error loading current user:', error);
            },
        });
    }

    loadAssignments(): void {
        this.isLoading = true;
        this.writerService.getAssignments().subscribe({
            next: (assignments) => {
                this.assignments = assignments;
                this.calculateAssignmentStats();
                this.isLoading = false;
            },
            error: (error) => {
                console.error('Error loading assignments:', error);
                this.isLoading = false;
            },
        });
    }

    calculateAssignmentStats(): void {
        this.assignmentStats.total = this.assignments.length;
        this.assignmentStats.pending = this.assignments.filter((a) => !a.status || a.status === 'Pending').length;
        this.assignmentStats.inProgress = this.assignments.filter((a) => a.status === 'In Progress').length;
        this.assignmentStats.completed = this.assignments.filter(
            (a) => a.status === 'Completed' || a.submissionStatus === 'Submitted'
        ).length;

        // Calculate overdue assignments
        const today = new Date();
        this.assignmentStats.overdue = this.assignments.filter((a) => {
            if (!a.deadline) return false;
            const deadline = new Date(a.deadline);
            return deadline < today && a.status !== 'Completed' && a.submissionStatus !== 'Submitted';
        }).length;
    }

    getUserDisplayName(): string {
        if (this.currentUser) {
            return this.currentUser.name || this.currentUser.userName || 'Writer';
        }
        return 'Writer';
    }

    getRecentAssignments(): WriterAssignmentDto[] {
        return this.assignments
            .sort((a, b) => new Date(b.orderDate || 0).getTime() - new Date(a.orderDate || 0).getTime())
            .slice(0, 5);
    }

    formatDate(date: Date | undefined): string {
        if (!date) return 'N/A';
        return new Date(date).toLocaleDateString();
    }

    getStatusClass(status: string | undefined): string {
        if (!status) return 'badge-secondary';

        switch (status.toLowerCase()) {
            case 'completed':
                return 'badge-success';
            case 'in progress':
                return 'badge-warning';
            case 'pending':
                return 'badge-info';
            default:
                return 'badge-secondary';
        }
    }

    isDeadlineUrgent(deadline: Date | undefined): boolean {
        if (!deadline) return false;
        const today = new Date();
        const deadlineDate = new Date(deadline);
        const diffDays = Math.ceil((deadlineDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));
        return diffDays <= 2 && diffDays >= 0; // Due within 2 days
    }
}
