import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ConstructionClientService } from '../../../services/construction-services';
import { ConstructionProjectService } from '../../../services/construction-project.service';
import { ProjectClient, CreateProjectClient, UpdateProjectClient } from '../../../models/construction.model';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-construction-clients',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule],
    templateUrl: './construction-clients.component.html',
    styleUrls: ['./construction-clients.component.css'],
})
export class ConstructionClientsComponent implements OnInit {
    clientForm: FormGroup;
    clients: ProjectClient[] = [];
    projects: any[] = [];
    editingClient: ProjectClient | null = null;
    loading = false;
    showForm = false;
    selectedProjectId: number = 0;

    constructor(
        private fb: FormBuilder,
        private clientService: ConstructionClientService,
        private projectService: ConstructionProjectService
    ) {
        this.clientForm = this.fb.group({
            clientName: ['', [Validators.required, Validators.maxLength(200)]],
            address: ['', Validators.maxLength(500)],
            phone: ['', Validators.maxLength(50)],
            email: ['', [Validators.email, Validators.maxLength(200)]],
            contactPerson: ['', Validators.maxLength(100)],
            cnicOrNtn: ['', Validators.maxLength(100)],
            sharePercentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
            constructionProjectId: [0, [Validators.required, Validators.min(1)]],
        });
    }

    ngOnInit(): void {
        this.loadProjects();
    }

    loadProjects(): void {
        this.projectService.getAll({ skipCount: 0, maxResultCount: 100 }).subscribe({
            next: (response) => {
                this.projects = response.items;
                if (this.projects.length > 0 && !this.selectedProjectId) {
                    this.selectedProjectId = this.projects[0].id;
                    this.loadClients();
                }
            },
            error: (error) => {
                console.error('Error loading projects:', error);
                this.showError('Failed to load projects');
            },
        });
    }

    onProjectChange(): void {
        if (this.selectedProjectId) {
            this.loadClients();
        }
    }

    loadClients(): void {
        if (!this.selectedProjectId) return;
        
        this.loading = true;
        this.clientService.getByProject(this.selectedProjectId).subscribe({
            next: (data) => {
                this.clients = data;
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading clients:', error);
                this.showError('Failed to load clients');
                this.loading = false;
            },
        });
    }

    openForm(client?: ProjectClient): void {
        this.showForm = true;
        if (client) {
            this.editingClient = client;
            this.clientForm.patchValue(client);
        } else {
            this.editingClient = null;
            this.clientForm.reset({
                sharePercentage: 0,
                constructionProjectId: this.selectedProjectId || 0,
            });
        }
    }

    closeForm(): void {
        this.showForm = false;
        this.editingClient = null;
        this.clientForm.reset({ sharePercentage: 0 });
    }

    onSubmit(): void {
        if (this.clientForm.invalid) {
            this.showError('Please fill all required fields correctly');
            return;
        }

        this.loading = true;
        const formValue = this.clientForm.value;

        if (this.editingClient) {
            const updateData: UpdateProjectClient = {
                id: this.editingClient.id,
                ...formValue,
                isActive: this.editingClient.isActive,
            };
            
            this.clientService.update(this.editingClient.id, updateData).subscribe({
                next: () => {
                    this.showSuccess('Client updated successfully!');
                    this.loadClients();
                    this.closeForm();
                    this.loading = false;
                },
                error: (error) => {
                    console.error('Error updating client:', error);
                    this.showError('Failed to update client');
                    this.loading = false;
                },
            });
        } else {
            const createData: CreateProjectClient = formValue;
            
            this.clientService.create(createData).subscribe({
                next: () => {
                    this.showSuccess('Client created successfully!');
                    this.loadClients();
                    this.closeForm();
                    this.loading = false;
                },
                error: (error) => {
                    console.error('Error creating client:', error);
                    this.showError('Failed to create client');
                    this.loading = false;
                },
            });
        }
    }

    deleteClient(client: ProjectClient): void {
        Swal.fire({
            title: 'Delete Client?',
            text: `Are you sure you want to delete "${client.clientName}"?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, delete it!',
        }).then((result) => {
            if (result.isConfirmed) {
                this.clientService.delete(client.id).subscribe({
                    next: () => {
                        this.showSuccess('Client deleted successfully!');
                        this.loadClients();
                    },
                    error: (error) => {
                        console.error('Error deleting client:', error);
                        this.showError('Failed to delete client');
                    },
                });
            }
        });
    }

    formatCurrency(value: number | undefined): string {
        if (!value) return '0.00';
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'PKR',
        }).format(value);
    }

    private showSuccess(message: string): void {
        Swal.fire({
            icon: 'success',
            title: 'Success!',
            text: message,
            timer: 2000,
            showConfirmButton: false,
        });
    }

    private showError(message: string): void {
        Swal.fire({
            icon: 'error',
            title: 'Error!',
            text: message,
        });
    }
}
