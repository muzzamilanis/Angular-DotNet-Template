import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProjectService } from '../../services/project.service';
import { Project, getProjectStatusText, getProjectStatusClass } from '../../models/project.model';

@Component({
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.css'],
  standalone: false
})
export class ProjectDetailComponent implements OnInit {
  project?: Project;
  loading = true;
  projectId?: number;

  // Alert state
  alertMessage = '';
  alertType = '';
  showAlert = false;

  constructor(
    private projectService: ProjectService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.projectId = +params['id'];
        this.loadProject();
      }
    });
  }

  private loadProject(): void {
    if (!this.projectId) return;

    this.loading = true;
    this.projectService.getById(this.projectId).subscribe({
      next: (project: Project) => {
        this.project = project;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading project:', error);
        this.showAlertMessage('Failed to load project details', 'danger');
        this.loading = false;
        this.router.navigate(['/masters/projects']);
      }
    });
  }

  editProject(): void {
    if (this.projectId) {
      this.router.navigate(['/masters/projects/edit', this.projectId]);
    }
  }

  backToList(): void {
    this.router.navigate(['/masters/projects']);
  }

  getStatusText(status: number): string {
    return getProjectStatusText(status);
  }

  getStatusClass(status: number): string {
    return getProjectStatusClass(status);
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

  closeAlert(): void {
    this.showAlert = false;
  }
}