import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  PerformanceCriteria,
  CreatePerformanceCriteriaDto,
  UpdatePerformanceCriteriaDto,
} from '../../../models/performance-criteria.model';
import { UserRole } from '../../../models/user.model';
import { PerformanceCriteriaService } from '../../../services/performance-criteria.service';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-performance-criteria-management',
  templateUrl: './performance-criteria-management.component.html',
  styleUrls: ['./performance-criteria-management.component.css'],
  standalone: false,
})
export class PerformanceCriteriaManagementComponent implements OnInit {
  criteriaForm: FormGroup;
  criteria: PerformanceCriteria[] = [];
  filteredCriteria: PerformanceCriteria[] = [];
  selectedCriteria: PerformanceCriteria | null = null;
  isEditing = false;
  isLoading = false;
  selectedRole: UserRole | null = null;

  UserRole = UserRole;

  readonly roleOptions: { value: UserRole; label: string }[] = [
    { value: UserRole.User, label: 'User' },
    { value: UserRole.Manager, label: 'Manager' },
    { value: UserRole.CompanyAdmin, label: 'Company Admin' },
    { value: UserRole.Writer, label: 'Writer' },
  ];

  constructor(
    private fb: FormBuilder,
    private criteriaService: PerformanceCriteriaService,
    private notificationService: NotificationService
  ) {
    this.criteriaForm = this.createForm();
  }

  ngOnInit(): void {
    this.loadCriteria();
  }

  createForm(): FormGroup {
    return this.fb.group({
      userRole: [UserRole.User, [Validators.required]],
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      category: [''],
      weightPercentage: [100, [Validators.required, Validators.min(0), Validators.max(100)]],
      measurementMethod: [''],
      scoringMethod: [''],
      targetValue: [null],
      minimumThreshold: [null],
      maximumThreshold: [null],
      isActive: [true],
      createdByAI: [false],
    });
  }

  loadCriteria(): void {
    this.isLoading = true;
    this.criteriaService.getAll().subscribe({
      next: (criteria) => {
        this.criteria = criteria;
        this.filterCriteria();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading criteria:', error);
        this.notificationService.showError('Error loading performance criteria');
        this.isLoading = false;
      },
    });
  }

  filterByRole(role: UserRole | null): void {
    this.selectedRole = role;
    this.filterCriteria();
  }

  filterCriteria(): void {
    if (this.selectedRole) {
      this.filteredCriteria = this.criteria.filter((c) => c.userRole === this.selectedRole);
    } else {
      this.filteredCriteria = this.criteria;
    }
  }

  onSubmit(): void {
    if (this.criteriaForm.valid) {
      const formValue = this.criteriaForm.value;

      if (this.isEditing && this.selectedCriteria) {
        const updateDto: UpdatePerformanceCriteriaDto = {
          ...formValue,
          id: this.selectedCriteria.id!,
        };
        this.updateCriteria(updateDto);
      } else {
        const createDto: CreatePerformanceCriteriaDto = formValue;
        this.createCriteria(createDto);
      }
    } else {
      this.markFormGroupTouched();
    }
  }

  createCriteria(dto: CreatePerformanceCriteriaDto): void {
    this.isLoading = true;
    this.criteriaService.create(dto).subscribe({
      next: (newCriteria) => {
        this.criteria.push(newCriteria);
        this.filterCriteria();
        this.resetForm();
        this.isLoading = false;
        this.notificationService.showSuccess('Performance criteria created successfully!');
      },
      error: (error) => {
        console.error('Error creating criteria:', error);
        this.isLoading = false;
        this.notificationService.showError('Error creating criteria: ' + (error.error?.message || error.message));
      },
    });
  }

  updateCriteria(dto: UpdatePerformanceCriteriaDto): void {
    this.isLoading = true;
    this.criteriaService.update(dto.id, dto).subscribe({
      next: (updatedCriteria) => {
        const index = this.criteria.findIndex((c) => c.id === updatedCriteria.id);
        if (index !== -1) {
          this.criteria[index] = updatedCriteria;
        }
        this.filterCriteria();
        this.resetForm();
        this.isLoading = false;
        this.notificationService.showSuccess('Performance criteria updated successfully!');
      },
      error: (error) => {
        console.error('Error updating criteria:', error);
        this.isLoading = false;
        this.notificationService.showError('Error updating criteria: ' + (error.error?.message || error.message));
      },
    });
  }

  editCriteria(criteria: PerformanceCriteria): void {
    this.selectedCriteria = criteria;
    this.isEditing = true;
    this.criteriaForm.patchValue({
      userRole: criteria.userRole,
      name: criteria.name,
      description: criteria.description,
      category: criteria.category,
      weightPercentage: criteria.weightPercentage,
      measurementMethod: criteria.measurementMethod,
      scoringMethod: criteria.scoringMethod,
      targetValue: criteria.targetValue,
      minimumThreshold: criteria.minimumThreshold,
      maximumThreshold: criteria.maximumThreshold,
      isActive: criteria.isActive,
      createdByAI: criteria.createdByAI,
    });
  }

  deleteCriteria(criteria: PerformanceCriteria): void {
    this.notificationService
      .showConfirm(`Are you sure you want to delete "${criteria.name}"?`, 'Delete Criteria', 'Yes, delete it!', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.criteriaService.delete(criteria.id!).subscribe({
            next: () => {
              this.criteria = this.criteria.filter((c) => c.id !== criteria.id);
              this.filterCriteria();
              this.notificationService.showSuccess('Criteria deleted successfully!');
            },
            error: (error) => {
              console.error('Error deleting criteria:', error);
              this.notificationService.showError('Error deleting criteria: ' + (error.error?.message || error.message));
            },
          });
        }
      });
  }

  resetForm(): void {
    this.criteriaForm.reset();
    this.criteriaForm.patchValue({
      userRole: UserRole.User,
      weightPercentage: 100,
      isActive: true,
      createdByAI: false,
    });
    this.selectedCriteria = null;
    this.isEditing = false;
  }

  private markFormGroupTouched(): void {
    Object.keys(this.criteriaForm.controls).forEach((key) => {
      const control = this.criteriaForm.get(key);
      control?.markAsTouched();
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.criteriaForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getRoleLabel(role: UserRole): string {
    const roleOption = this.roleOptions.find((r) => r.value === role);
    return roleOption ? roleOption.label : role;
  }
}
