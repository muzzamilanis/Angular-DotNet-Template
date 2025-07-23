import { Component, OnInit } from '@angular/core';
import { ProjectService } from '../../services/project.service';
import { ProjectListItem, ProjectStatus, getProjectStatusText, getProjectStatusClass } from '../../models/project.model';
import { PaginatedResponse } from '../../models/common.model';

@Component({
  selector: 'app-project-list',
  templateUrl: './project-list.component.html',
  styleUrls: ['./project-list.component.css'],
  standalone: false
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

  constructor(private projectService: ProjectService) { }

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
      sorting: this.getSortingString()
    };

    this.projectService.getAll(params).subscribe({
      next: (response: PaginatedResponse<ProjectListItem>) => {
        this.projects = response.items;
        this.totalRecords = response.totalCount;
        this.totalPages = Math.ceil(this.totalRecords / this.itemsPerPage);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading projects:', error);
        this.showAlertMessage('Failed to load projects', 'danger');
        this.loading = false;
      }
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
    if (confirm(`Are you sure you want to delete the project "${project.projectName}"?`)) {
      this.deleteProject(project.id);
    }
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
      }
    });
  }

  getStatusText(status: ProjectStatus): string {
    return getProjectStatusText(status);
  }

  getStatusClass(status: ProjectStatus): string {
    return getProjectStatusClass(status);
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