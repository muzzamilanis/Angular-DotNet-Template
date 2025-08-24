import { Component, OnInit } from '@angular/core';
import { TeamLeadService, TeamLeadOrderDto } from '../../services/team-lead.service';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-team-lead-dashboard',
    templateUrl: './team-lead-dashboard.component.html',
    styleUrls: ['./team-lead-dashboard.component.css'],
    standalone: false
})
export class TeamLeadDashboardComponent implements OnInit {
    currentUser: any = null;
    isLoading = true;
    orders: TeamLeadOrderDto[] = [];
    orderStats = {
        total: 0,
        assigned: 0,
        unassigned: 0,
        completed: 0,
        overdue: 0,
        needClarity: 0
    };

    constructor(
        private teamLeadService: TeamLeadService,
        private authService: AuthService
    ) { }

    ngOnInit(): void {
        this.loadCurrentUser();
        this.loadOrders();
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
            }
        });
    }

    loadOrders(): void {
        this.isLoading = true;
        this.teamLeadService.getOrders().subscribe({
            next: (orders) => {
                this.orders = orders;
                this.calculateOrderStats();
                this.isLoading = false;
            },
            error: (error) => {
                console.error('Error loading orders:', error);
                this.isLoading = false;
            }
        });
    }

    calculateOrderStats(): void {
        this.orderStats.total = this.orders.length;
        this.orderStats.assigned = this.orders.filter(o => o.assignedToUserId).length;
        this.orderStats.unassigned = this.orders.filter(o => !o.assignedToUserId).length;
        this.orderStats.completed = this.orders.filter(o => o.status === 'Completed').length;
        this.orderStats.needClarity = this.orders.filter(o => o.status === 'Need clarity').length;
        
        // Calculate overdue orders
        const today = new Date();
        this.orderStats.overdue = this.orders.filter(o => {
            if (!o.deadline) return false;
            const deadline = new Date(o.deadline);
            return deadline < today && o.status !== 'Completed';
        }).length;
    }

    getUserDisplayName(): string {
        if (this.currentUser) {
            return this.currentUser.name || this.currentUser.userName || 'Team Lead';
        }
        return 'Team Lead';
    }

    getRecentOrders(): TeamLeadOrderDto[] {
        return this.orders
            .sort((a, b) => new Date(b.orderDate || 0).getTime() - new Date(a.orderDate || 0).getTime())
            .slice(0, 5);
    }

    formatDate(date: Date | undefined): string {
        if (!date) return 'N/A';
        return new Date(date).toLocaleDateString();
    }

    getStatusClass(status: string | undefined): string {
        switch (status) {
            case 'Completed': return 'badge-success';
            case 'workSubmittedByWriter': return 'badge-info';
            case 'InProgress': return 'badge-warning';
            case 'Need clarity': return 'badge-danger';
            default: return 'badge-secondary';
        }
    }

    isDeadlineUrgent(deadline: Date | undefined): boolean {
        if (!deadline) return false;
        const today = new Date();
        const deadlineDate = new Date(deadline);
        const diffDays = Math.ceil((deadlineDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));
        return diffDays <= 2 && diffDays >= 0; // Due within 2 days
    }

    getAssignmentStatusText(order: TeamLeadOrderDto): string {
        return order.assignedToUserId ? 'Assigned' : 'Unassigned';
    }

    getAssignmentStatusClass(order: TeamLeadOrderDto): string {
        return order.assignedToUserId ? 'badge-success' : 'badge-warning';
    }
}