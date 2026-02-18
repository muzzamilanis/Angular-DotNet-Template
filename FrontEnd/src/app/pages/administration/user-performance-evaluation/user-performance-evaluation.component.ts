import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BackendUser } from '../../../models/user.model';
import { PerformanceCriteria } from '../../../models/performance-criteria.model';
import {
  UserPerformanceScore,
  CreateUserPerformanceScoreDto,
} from '../../../models/user-performance-score.model';
import { UserService } from '../../../services/user.service';
import { PerformanceCriteriaService } from '../../../services/performance-criteria.service';
import { UserPerformanceScoreService } from '../../../services/user-performance-score.service';
import { NotificationService } from '../../../services/notification.service';

@Component({
  selector: 'app-user-performance-evaluation',
  templateUrl: './user-performance-evaluation.component.html',
  styleUrls: ['./user-performance-evaluation.component.css'],
  standalone: false,
})
export class UserPerformanceEvaluationComponent implements OnInit {
  evaluationForm: FormGroup;
  users: BackendUser[] = [];
  criteria: PerformanceCriteria[] = [];
  scores: UserPerformanceScore[] = [];
  selectedUser: BackendUser | null = null;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private criteriaService: PerformanceCriteriaService,
    private scoreService: UserPerformanceScoreService,
    private notificationService: NotificationService
  ) {
    this.evaluationForm = this.createForm();
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  createForm(): FormGroup {
    return this.fb.group({
      userId: ['', [Validators.required]],
      performanceCriteriaId: ['', [Validators.required]],
      score: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
      achievedValue: [null],
      evaluationPeriod: [''],
      comments: [''],
    });
  }

  loadUsers(): void {
    this.isLoading = true;
    this.userService.getAll().subscribe({
      next: (users) => {
        this.users = users.filter((u) => u.isActive);
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.notificationService.showError('Error loading users');
        this.isLoading = false;
      },
    });
  }

  onUserChange(userId: string): void {
    if (!userId) {
      this.selectedUser = null;
      this.criteria = [];
      this.scores = [];
      return;
    }

    const user = this.users.find((u) => u.id === parseInt(userId));
    if (user) {
      this.selectedUser = user;
      this.loadCriteriaForUser(user);
      this.loadUserScores(user.id!);
    }
  }

  loadCriteriaForUser(user: BackendUser): void {
    this.isLoading = true;
    this.criteriaService.getByUserRole(user.role).subscribe({
      next: (criteria) => {
        this.criteria = criteria.filter((c) => c.isActive);
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading criteria:', error);
        this.notificationService.showError('Error loading performance criteria');
        this.isLoading = false;
      },
    });
  }

  loadUserScores(userId: number): void {
    this.scoreService.getByUserId(userId).subscribe({
      next: (scores) => {
        this.scores = scores;
      },
      error: (error) => {
        console.error('Error loading scores:', error);
      },
    });
  }

  onSubmit(): void {
    if (this.evaluationForm.valid) {
      const formValue = this.evaluationForm.value;
      const dto: CreateUserPerformanceScoreDto = {
        userId: parseInt(formValue.userId),
        performanceCriteriaId: parseInt(formValue.performanceCriteriaId),
        score: formValue.score,
        achievedValue: formValue.achievedValue,
        evaluationPeriod: formValue.evaluationPeriod,
        comments: formValue.comments,
      };

      this.createEvaluation(dto);
    }
  }

  createEvaluation(dto: CreateUserPerformanceScoreDto): void {
    this.isLoading = true;
    this.scoreService.create(dto).subscribe({
      next: (newScore) => {
        this.scores.unshift(newScore);
        this.resetForm();
        this.isLoading = false;
        this.notificationService.showSuccess('Performance evaluation created successfully!');
      },
      error: (error) => {
        console.error('Error creating evaluation:', error);
        this.isLoading = false;
        this.notificationService.showError('Error creating evaluation: ' + (error.error?.message || error.message));
      },
    });
  }

  resetForm(): void {
    const currentUserId = this.evaluationForm.get('userId')?.value;
    this.evaluationForm.reset();
    this.evaluationForm.patchValue({
      userId: currentUserId,
      score: 0,
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.evaluationForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getCriteriaName(criteriaId: number): string {
    const criteria = this.criteria.find((c) => c.id === criteriaId);
    return criteria ? criteria.name : 'Unknown';
  }
}
