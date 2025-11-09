import { Component, OnInit } from '@angular/core';
import { ProjectService } from '../../services/project.service';
import {
    ProjectListItem,
    ProjectStatus,
    getProjectStatusText,
    getProjectStatusClass,
} from '../../models/project.model';
import { PaginatedResponse } from '../../models/common.model';
import { NotificationService } from '../../services/notification.service';

@Component({
    selector: 'app-project-list',
    templateUrl: './project-list.component.html',
    styleUrls: ['./project-list.component.css'],
    standalone: false,
})
export class ProjectListComponent implements OnInit {
    projects: ProjectListItem[] = [];
    totalRecords = 0;
    loading = true;

    // Pagination
    currentPage = 1;
    itemsPerPage = 10;
    totalPages = 0;

    // Filtering
    globalFilter = '';

    // Sorting
    sortField = 'updatedDate';
    sortOrder = 'desc';

    // Project Status enum for template
    ProjectStatus = ProjectStatus;

    // Alert state
    alertMessage = '';
    alertType = '';
    showAlert = false;

    constructor(
        private projectService: ProjectService,
        private notificationService: NotificationService
    ) {}

    ngOnInit(): void {
        this.loadProjects();
    }

    loadProjects(): void {
        this.loading = true;

        const skipCount = (this.currentPage - 1) * this.itemsPerPage;
        const params = {
            skipCount: skipCount,
            maxResultCount: this.itemsPerPage,
            filter: this.globalFilter || undefined,
            sorting: this.getSortingString(),
        };

        this.projectService.getAll(params).subscribe({
            next: (response: PaginatedResponse<ProjectListItem>) => {
                console.log('items', response.items);
                // Normalize projectStatus coming from API (number 0..2, 1..3, or strings)
                this.projects = (response.items || []).map(
                    (p) =>
                        ({
                            ...p,
                            projectStatus: this.normalizeStatus(p.projectStatus),
                        }) as ProjectListItem
                );
                this.totalRecords = response.totalCount;
                this.totalPages = Math.ceil(this.totalRecords / this.itemsPerPage);
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading projects:', error);
                this.showAlertMessage('Failed to load projects', 'danger');
                this.loading = false;
            },
        });
    }

    onPageChange(page: number): void {
        this.currentPage = page;
        this.loadProjects();
    }

    onGlobalFilter(): void {
        this.currentPage = 1; // Reset to first page when filtering
        this.loadProjects();
    }

    onSort(field: string): void {
        if (this.sortField === field) {
            this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
        } else {
            this.sortField = field;
            this.sortOrder = 'asc';
        }
        this.loadProjects();
    }

    getSortingString(): string {
        if (!this.sortField) return 'updatedDate desc';

        const direction = this.sortOrder === 'desc' ? ' desc' : '';
        return `${this.sortField}${direction}`;
    }

    confirmDelete(project: ProjectListItem): void {
        this.notificationService
            .showConfirm(
                `Are you sure you want to delete the project "${project.projectName}"?`,
                'Delete Project',
                'Yes, delete it!',
                'Cancel'
            )
            .then((confirmed) => {
                if (confirmed) {
                    this.deleteProject(project.id);
                }
            });
    }

    deleteProject(id: number): void {
        this.projectService.delete(id).subscribe({
            next: () => {
                this.showAlertMessage('Project deleted successfully', 'success');
                this.loadProjects();
            },
            error: (error) => {
                console.error('Error deleting project:', error);
                this.showAlertMessage('Failed to delete project', 'danger');
            },
        });
    }

    getStatusText(status: ProjectStatus | number | string): string {
        return getProjectStatusText(this.normalizeStatus(status));
    }

    getStatusClass(status: ProjectStatus | number | string): string {
        return getProjectStatusClass(this.normalizeStatus(status));
    }

    private normalizeStatus(status: ProjectStatus | number | string | null | undefined): ProjectStatus {
        // Handle numeric values
        if (typeof status === 'number') {
            // If API returns 1..3 (matches enum)
            if (status >= 1 && status <= 3) return status as ProjectStatus;
            // If API returns 0..2 (shift to 1..3)
            if (status >= 0 && status <= 2) return (status + 1) as ProjectStatus;
        }

        // Handle string values ("Active"/"Inactive"/"Completed" or "1"/"2"/"3")
        if (typeof status === 'string') {
            const s = status.trim().toLowerCase();
            // numeric string
            if (/^\d+$/.test(s)) {
                const n = parseInt(s, 10);
                if (n >= 1 && n <= 3) return n as ProjectStatus;
                if (n >= 0 && n <= 2) return (n + 1) as ProjectStatus;
            }
            // named string
            if (s === 'active') return ProjectStatus.Active;
            if (s === 'inactive') return ProjectStatus.Inactive;
            if (s === 'completed' || s === 'complete') return ProjectStatus.Completed;
        }

        // Fallback (avoid "Unknown")
        return ProjectStatus.Active;
    }

    getSortIcon(field: string): string {
        if (this.sortField !== field) {
            return 'fas fa-sort text-muted';
        }
        return this.sortOrder === 'asc' ? 'fas fa-sort-up' : 'fas fa-sort-down';
    }

    refresh(): void {
        this.currentPage = 1;
        this.globalFilter = '';
        this.loadProjects();
    }

    getPageNumbers(): number[] {
        const pages: number[] = [];
        const maxPages = 5; // Show up to 5 page numbers

        let startPage = Math.max(1, this.currentPage - Math.floor(maxPages / 2));
        let endPage = Math.min(this.totalPages, startPage + maxPages - 1);

        if (endPage - startPage < maxPages - 1) {
            startPage = Math.max(1, endPage - maxPages + 1);
        }

        for (let i = startPage; i <= endPage; i++) {
            pages.push(i);
        }

        return pages;
    }

    private showAlertMessage(message: string, type: string): void {
        this.alertMessage = message;
        this.alertType = type;
        this.showAlert = true;

        // Auto-hide after 5 seconds
        setTimeout(() => {
            this.showAlert = false;
        }, 5000);
    }

    getEndRecord(): number {
        return Math.min(this.currentPage * this.itemsPerPage, this.totalRecords);
    }

    closeAlert(): void {
        this.showAlert = false;
    }
}
