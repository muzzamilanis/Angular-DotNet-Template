import { Component, OnInit } from '@angular/core';
import { WriterService } from '../../services/writer.service';
import { SaleOrderService } from '../../services/sale-order.service';
import { WriterAssignmentDto, OrderInstructionDto, CreateOrderInstructionDto } from '../../models/sale-order.model';
import { NotificationService } from '../../services/notification.service';

@Component({
    selector: 'app-writer-assignments',
    templateUrl: './writer-assignments.component.html',
    styleUrls: ['./writer-assignments.component.css'],
    standalone: false,
})
export class WriterAssignmentsComponent implements OnInit {
    assignments: WriterAssignmentDto[] = [];
    isLoading = false;
    selectedAssignment: WriterAssignmentDto | null = null;
    isSubmissionModalOpen = false;
    submissionStatus = '';
    submissionFile: File | null = null;
    submissionNotes = '';

    // Instructions
    isInstructionModalOpen = false;
    instructions: OrderInstructionDto[] = [];
    newResponseMessage = '';
    isLoadingInstructions = false;
    instructionFile: File | null = null;

    constructor(
        private writerService: WriterService,
        private saleOrderService: SaleOrderService,
        private notificationService: NotificationService
    ) {}

    ngOnInit(): void {
        this.loadAssignments();
    }

    loadAssignments(): void {
        this.isLoading = true;
        this.writerService.getAssignments().subscribe({
            next: (assignments) => {
                this.assignments = assignments;
                this.isLoading = false;
            },
            error: (error) => {
                console.error('Error loading assignments:', error);
                this.isLoading = false;
            },
        });
    }

    openSubmissionModal(assignment: WriterAssignmentDto): void {
        this.selectedAssignment = assignment;
        this.isSubmissionModalOpen = true;
        this.submissionStatus = 'Completed';
        this.submissionFile = null;
        this.submissionNotes = '';
    }

    closeSubmissionModal(): void {
        this.isSubmissionModalOpen = false;
        this.selectedAssignment = null;
    }

    onFileSelected(event: any): void {
        const file = event.target.files[0];
        if (file) {
            this.submissionFile = file;
        }
    }

    onInstructionFileSelected(event: any): void {
        const file = event.target.files[0];
        if (file) {
            this.instructionFile = file;
        }
    }

    submitWork(): void {
        if (!this.selectedAssignment || !this.submissionStatus) {
            return;
        }

        const submission = {
            saleOrderId: this.selectedAssignment.id,
            submissionStatus: this.submissionStatus,
            notes: this.submissionNotes,
        };

        this.writerService.submitWork(submission, this.submissionFile || undefined).subscribe({
            next: (response) => {
                this.notificationService.showSuccess('Work submitted successfully!');
                this.closeSubmissionModal();
                this.loadAssignments(); // Reload to see updated status
            },
            error: (error) => {
                console.error('Error submitting work:', error);
                this.notificationService.showError('Error submitting work. Please try again.');
            },
        });
    }

    downloadAttachment(filename: string): void {
        console.log('Attempting to download file:', filename);
        this.saleOrderService.downloadFile(filename).subscribe({
            next: (blob) => {
                console.log('File download successful, blob size:', blob.size);
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = filename;
                document.body.appendChild(a);
                a.click();
                window.URL.revokeObjectURL(url);
                document.body.removeChild(a);
            },
            error: (error) => {
                console.error('Error downloading file:', error);
                console.error('Error status:', error.status);
                console.error('Error message:', error.message);

                if (error.status === 403) {
                    this.notificationService.showError(
                        'Access denied. You do not have permission to download this file.'
                    );
                } else if (error.status === 404) {
                    this.notificationService.showError('File not found. The attachment may have been deleted.');
                } else if (error.status === 401) {
                    this.notificationService.showError('Authentication required. Please log in again.');
                } else {
                    this.notificationService.showError(`Error downloading file: ${error.message || 'Unknown error'}`);
                }
            },
        });
    }

    getAttachmentsList(attachmentFilenames: string | undefined): string[] {
        if (!attachmentFilenames) {
            return [];
        }
        return attachmentFilenames.split(',').filter((name) => name.trim());
    }

    formatDate(date: Date | undefined): string {
        if (!date) {
            return 'N/A';
        }
        return new Date(date).toLocaleDateString();
    }

    getStatusClass(status: string | undefined): string {
        switch (status) {
            case 'Completed':
                return 'badge badge-success';
            case 'Need clarity':
                return 'badge badge-warning';
            case 'InProgress':
                return 'badge badge-info';
            default:
                return 'badge badge-secondary';
        }
    }

    getSubmissionStatusClass(submissionStatus: string | undefined): string {
        switch (submissionStatus) {
            case 'Completed':
                return 'badge badge-success';
            case 'Need clarity':
                return 'badge badge-warning';
            default:
                return 'badge badge-light';
        }
    }

    isDeadlineUrgent(deadline: Date | undefined): boolean {
        if (!deadline) {
            return false;
        }
        const deadlineDate = new Date(deadline);
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        return deadlineDate <= tomorrow;
    }

    // Instructions methods
    openInstructionsModal(assignment: WriterAssignmentDto): void {
        this.selectedAssignment = assignment;
        this.isInstructionModalOpen = true;
        this.loadInstructions(assignment.id);
    }

    closeInstructionsModal(): void {
        this.isInstructionModalOpen = false;
        this.selectedAssignment = null;
        this.instructions = [];
        this.newResponseMessage = '';
        this.instructionFile = null;
    }

    loadInstructions(saleOrderId: number): void {
        this.isLoadingInstructions = true;
        this.writerService.getOrderInstructions(saleOrderId).subscribe({
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

    addResponse(): void {
        if (!this.selectedAssignment || !this.newResponseMessage.trim()) {
            return;
        }

        const response: CreateOrderInstructionDto = {
            saleOrderId: this.selectedAssignment.id,
            message: this.newResponseMessage.trim(),
        };

        this.writerService
            .addWriterResponse(this.selectedAssignment.id, response, this.instructionFile || undefined)
            .subscribe({
                next: (newResponse) => {
                    this.instructions.push(newResponse);
                    this.newResponseMessage = '';
                    this.instructionFile = null;
                    // Clear the file input
                    const fileInput = document.getElementById('instructionFile') as HTMLInputElement;
                    if (fileInput) {
                        fileInput.value = '';
                    }
                    this.notificationService.showSuccess('Response sent successfully!');
                },
                error: (error) => {
                    console.error('Error sending response:', error);
                    this.notificationService.showError('Error sending response. Please try again.');
                },
            });
    }

    formatInstructionDate(date: Date): string {
        return new Date(date).toLocaleString();
    }

    getInstructionAttachments(attachmentFilenames?: string): string[] {
        if (!attachmentFilenames) {
            return [];
        }
        return attachmentFilenames.split(',').filter((name) => name.trim());
    }
}
