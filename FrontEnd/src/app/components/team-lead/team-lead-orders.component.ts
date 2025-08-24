import { Component, OnInit } from '@angular/core';
import { TeamLeadService, TeamLeadOrderDto, WriterDto, AssignOrderDto } from '../../services/team-lead.service';
import { NotificationService } from '../../services/notification.service';

@Component({
    selector: 'app-team-lead-orders',
    templateUrl: './team-lead-orders.component.html',
    styleUrls: ['./team-lead-orders.component.css'],
    standalone: false
})
export class TeamLeadOrdersComponent implements OnInit {
    orders: TeamLeadOrderDto[] = [];
    writers: WriterDto[] = [];
    isLoading = false;
    isAssignModalOpen = false;
    selectedOrder: TeamLeadOrderDto | null = null;
    selectedWriterId: number | null = null;

    constructor(
        private teamLeadService: TeamLeadService,
        private notificationService: NotificationService
    ) { }

    ngOnInit(): void {
        this.loadOrders();
        this.loadWriters();
    }

    loadOrders(): void {
        this.isLoading = true;
        this.teamLeadService.getOrders().subscribe({
            next: (orders) => {
                this.orders = orders;
                this.isLoading = false;
            },
            error: (error) => {
                console.error('Error loading orders:', error);
                this.notificationService.showError('Error loading orders. Please try again.');
                this.isLoading = false;
            }
        });
    }

    loadWriters(): void {
        this.teamLeadService.getWriters().subscribe({
            next: (writers) => {
                this.writers = writers.filter(w => w.isActive);
            },
            error: (error) => {
                console.error('Error loading writers:', error);
                this.notificationService.showError('Error loading writers.');
            }
        });
    }

    openAssignModal(order: TeamLeadOrderDto): void {
        this.selectedOrder = order;
        this.selectedWriterId = order.assignedToUserId || null;
        this.isAssignModalOpen = true;
    }

    closeAssignModal(): void {
        this.isAssignModalOpen = false;
        this.selectedOrder = null;
        this.selectedWriterId = null;
    }

    assignOrder(): void {
        if (!this.selectedOrder || !this.selectedWriterId) {
            this.notificationService.showError('Please select a writer.');
            return;
        }

        const assignment: AssignOrderDto = {
            saleOrderId: this.selectedOrder.id,
            writerId: this.selectedWriterId
        };

        const isUpdate = !!this.selectedOrder.assignedToUserId;
        const operation = isUpdate 
            ? this.teamLeadService.updateAssignment(assignment)
            : this.teamLeadService.assignOrder(assignment);

        operation.subscribe({
            next: (response) => {
                this.notificationService.showSuccess(
                    isUpdate ? 'Assignment updated successfully!' : 'Order assigned successfully!'
                );
                this.closeAssignModal();
                this.loadOrders(); // Reload to see updated assignments
            },
            error: (error) => {
                console.error('Error assigning order:', error);
                this.notificationService.showError('Error assigning order. Please try again.');
            }
        });
    }

    formatDate(date: Date | undefined): string {
        if (!date) return 'N/A';
        return new Date(date).toLocaleDateString();
    }

    getStatusClass(status: string | undefined): string {
        switch (status) {
            case 'Completed': return 'badge badge-success';
            case 'workSubmittedByWriter': return 'badge badge-info';
            case 'InProgress': return 'badge badge-warning';
            case 'Need clarity': return 'badge badge-danger';
            default: return 'badge badge-secondary';
        }
    }

    getSubmissionStatusClass(status: string | undefined): string {
        switch (status) {
            case 'Completed': return 'badge badge-success';
            case 'Need clarity': return 'badge badge-warning';
            default: return 'badge badge-light';
        }
    }

    canReassign(order: TeamLeadOrderDto): boolean {
        // Can reassign if order is not completed or if writer submitted work but it's not marked complete
        return order.status !== 'Completed' || order.submissionStatus !== 'Completed';
    }

    getAttachmentsList(attachmentFilenames: string | undefined): string[] {
        if (!attachmentFilenames) return [];
        return attachmentFilenames.split(',').filter(name => name.trim());
    }

    isDeadlineUrgent(deadline: Date | undefined): boolean {
        if (!deadline) return false;
        const now = new Date();
        const deadlineDate = new Date(deadline);
        const diffTime = deadlineDate.getTime() - now.getTime();
        const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        return diffDays <= 2 && diffDays >= 0; // Urgent if 2 days or less
    }

    getWriterName(writerId?: number): string {
        if (!writerId) return 'Unassigned';
        const writer = this.writers.find(w => w.id === writerId);
        return writer ? writer.name : 'Unknown Writer';
    }
}