import { Component, OnInit } from '@angular/core';
import { TeamLeadService, TeamLeadOrderDto, WriterDto, AssignOrderDto } from '../../services/team-lead.service';
import { NotificationService } from '../../services/notification.service';
import { OrderInstructionDto, CreateOrderInstructionDto, UpdateOrderDetailsDto } from '../../models/sale-order.model';

@Component({
    selector: 'app-team-lead-orders',
    templateUrl: './team-lead-orders.component.html',
    styleUrls: ['./team-lead-orders.component.css'],
    standalone: false,
})
export class TeamLeadOrdersComponent implements OnInit {
    orders: TeamLeadOrderDto[] = [];
    writers: WriterDto[] = [];
    isLoading = false;
    isAssignModalOpen = false;
    selectedOrder: TeamLeadOrderDto | null = null;
    selectedWriterId: number | null = null;

    // Order details editing
    isEditingDetails = false;
    editingOrderId: number | null = null;
    editedOrderDetails = '';

    // Instructions
    isInstructionModalOpen = false;
    instructions: OrderInstructionDto[] = [];
    newInstructionMessage = '';
    isLoadingInstructions = false;

    // File upload for instructions
    selectedInstructionFiles: File[] = [];

    constructor(
        private teamLeadService: TeamLeadService,
        private notificationService: NotificationService
    ) {}

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
            },
        });
    }

    loadWriters(): void {
        this.teamLeadService.getWriters().subscribe({
            next: (writers) => {
                this.writers = writers.filter((w) => w.isActive);
            },
            error: (error) => {
                console.error('Error loading writers:', error);
                this.notificationService.showError('Error loading writers.');
            },
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
            writerId: this.selectedWriterId,
        };

        // For legacy orders where assignedToUserId might be null but assignedTo has a value,
        // we should treat it as a new assignment rather than an update
        const hasValidCurrentAssignment =
            this.selectedOrder.assignedToUserId && this.selectedOrder.assignedToUserId > 0;
        const isUpdate = hasValidCurrentAssignment;

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

                // If update fails for legacy data, try as new assignment
                if (isUpdate && error.status === 400) {
                    console.log('Update failed, trying as new assignment for legacy data...');
                    this.teamLeadService.assignOrder(assignment).subscribe({
                        next: (response) => {
                            this.notificationService.showSuccess('Order assigned successfully!');
                            this.closeAssignModal();
                            this.loadOrders();
                        },
                        error: (assignError) => {
                            console.error('Error in fallback assignment:', assignError);
                            this.notificationService.showError('Error assigning order. Please try again.');
                        },
                    });
                } else {
                    this.notificationService.showError('Error assigning order. Please try again.');
                }
            },
        });
    }

    formatDate(date: Date | undefined): string {
        if (!date) return 'N/A';
        return new Date(date).toLocaleDateString();
    }

    getStatusClass(status: string | undefined): string {
        switch (status) {
            case 'Completed':
                return 'badge badge-success';
            case 'workSubmittedByWriter':
                return 'badge badge-info';
            case 'InProgress':
                return 'badge badge-warning';
            case 'Need clarity':
                return 'badge badge-danger';
            default:
                return 'badge badge-secondary';
        }
    }

    getSubmissionStatusClass(status: string | undefined): string {
        switch (status) {
            case 'Completed':
                return 'badge badge-success';
            case 'Need clarity':
                return 'badge badge-warning';
            default:
                return 'badge badge-light';
        }
    }

    canReassign(order: TeamLeadOrderDto): boolean {
        // Can reassign if order is not completed or if writer submitted work but it's not marked complete
        return order.status !== 'Completed' || order.submissionStatus !== 'Completed';
    }

    getAttachmentsList(attachmentFilenames: string | undefined): string[] {
        if (!attachmentFilenames) return [];
        return attachmentFilenames.split(',').filter((name) => name.trim());
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
        const writer = this.writers.find((w) => w.id === writerId);
        return writer ? writer.name : 'Unknown Writer';
    }

    // Order details editing methods
    startEditingDetails(order: TeamLeadOrderDto): void {
        this.isEditingDetails = true;
        this.editingOrderId = order.id;
        this.editedOrderDetails = order.orderDetails || '';
    }

    cancelEditingDetails(): void {
        this.isEditingDetails = false;
        this.editingOrderId = null;
        this.editedOrderDetails = '';
    }

    saveOrderDetails(): void {
        if (!this.editingOrderId) return;

        const updateDto: UpdateOrderDetailsDto = {
            id: this.editingOrderId,
            orderDetails: this.editedOrderDetails,
        };

        this.teamLeadService.updateOrderDetails(this.editingOrderId, updateDto).subscribe({
            next: (response) => {
                this.notificationService.showSuccess('Order details updated successfully!');
                this.cancelEditingDetails();
                this.loadOrders(); // Reload to see updated details
            },
            error: (error) => {
                console.error('Error updating order details:', error);
                this.notificationService.showError('Error updating order details. Please try again.');
            },
        });
    }

    // Instructions methods
    openInstructionsModal(order: TeamLeadOrderDto): void {
        this.selectedOrder = order;
        this.isInstructionModalOpen = true;
        this.loadInstructions(order.id);
    }

    closeInstructionsModal(): void {
        this.isInstructionModalOpen = false;
        this.selectedOrder = null;
        this.instructions = [];
        this.newInstructionMessage = '';
        this.selectedInstructionFiles = [];
    }

    loadInstructions(saleOrderId: number): void {
        this.isLoadingInstructions = true;
        this.teamLeadService.getOrderInstructions(saleOrderId).subscribe({
            next: (instructions) => {
                this.instructions = instructions;
                this.isLoadingInstructions = false;
            },
            error: (error) => {
                console.error('Error loading instructions:', error);
                this.notificationService.showError('Error loading instructions.');
                this.isLoadingInstructions = false;
            },
        });
    }

    addInstruction(): void {
        if (!this.selectedOrder || !this.newInstructionMessage.trim()) return;

        // Upload files first if any are selected
        if (this.selectedInstructionFiles.length > 0) {
            this.uploadInstructionFiles()
                .then((filenames) => {
                    this.sendInstruction(filenames.join(','));
                })
                .catch((error) => {
                    console.error('Error uploading files:', error);
                    this.notificationService.showError('Error uploading files. Please try again.');
                });
        } else {
            this.sendInstruction();
        }
    }

    private sendInstruction(attachmentFilenames?: string): void {
        if (!this.selectedOrder) return;

        const instruction: CreateOrderInstructionDto = {
            saleOrderId: this.selectedOrder.id,
            message: this.newInstructionMessage.trim(),
            attachmentFilenames: attachmentFilenames,
        };

        this.teamLeadService.addOrderInstruction(this.selectedOrder.id, instruction).subscribe({
            next: (newInstruction) => {
                this.instructions.push(newInstruction);
                this.newInstructionMessage = '';
                this.selectedInstructionFiles = [];
                this.notificationService.showSuccess('Instruction added successfully!');
            },
            error: (error) => {
                console.error('Error adding instruction:', error);
                this.notificationService.showError('Error adding instruction. Please try again.');
            },
        });
    }

    private uploadInstructionFiles(): Promise<string[]> {
        return new Promise((resolve, reject) => {
            if (!this.selectedOrder) {
                reject('No order selected');
                return;
            }

            this.teamLeadService
                .uploadInstructionFiles(this.selectedOrder.id, this.selectedInstructionFiles)
                .subscribe({
                    next: (response) => {
                        resolve(response.filenames || []);
                    },
                    error: (error) => {
                        reject(error);
                    },
                });
        });
    }

    onInstructionFilesSelected(event: any): void {
        const files = event.target.files;
        if (files) {
            this.selectedInstructionFiles = Array.from(files);
        }
    }

    formatInstructionDate(date: Date): string {
        return new Date(date).toLocaleString();
    }

    getInstructionAttachments(attachmentFilenames?: string): string[] {
        if (!attachmentFilenames) return [];
        return attachmentFilenames.split(',').filter((name) => name.trim());
    }
}
