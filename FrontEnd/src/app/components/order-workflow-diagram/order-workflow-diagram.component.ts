import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { Subscription } from 'rxjs';
import { OrderWorkflowService, WorkflowStage, WORKFLOW_STAGES } from '../../services/order-workflow.service';

@Component({
    selector: 'app-order-workflow-diagram',
    templateUrl: './order-workflow-diagram.component.html',
    styleUrls: ['./order-workflow-diagram.component.css'],
    standalone: false,
})
export class OrderWorkflowDiagramComponent implements OnInit, OnDestroy {
    orderId!: number;
    stages = WORKFLOW_STAGES;
    workflowHistory: any[] = [];
    currentStage = '';
    selectedStage: any = null;
    isLoading = true;
    private subs: Subscription[] = [];

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private location: Location,
        private orderWorkflowService: OrderWorkflowService
    ) { }

    ngOnInit(): void {
        this.orderId = Number(this.route.snapshot.paramMap.get('id'));
        this.loadWorkflow();
        this.connectRealTime();
    }

    ngOnDestroy(): void {
        this.subs.forEach(s => s.unsubscribe());
        this.orderWorkflowService.stopConnection(this.orderId.toString());
    }

    loadWorkflow(): void {
        this.isLoading = true;
        this.orderWorkflowService.getOrderWorkflow(this.orderId).subscribe({
            next: (history) => {
                this.workflowHistory = history;
                if (history.length > 0) {
                    this.currentStage = history[history.length - 1].stage;
                }
                this.isLoading = false;
            },
            error: (err) => {
                console.error('Failed to load workflow history:', err);
                this.isLoading = false;
            }
        });
    }

    connectRealTime(): void {
        this.orderWorkflowService.startConnection(this.orderId.toString());
        this.orderWorkflowService.onWorkflowUpdated((data) => {
            this.currentStage = data.stage;
            this.workflowHistory.push(data);
        });
    }

    getStageStatus(stageKey: string): 'completed' | 'current' | 'pending' | 'delayed' {
        const stageIndex = this.stages.findIndex(s => s.key === stageKey);
        const currentIndex = this.stages.findIndex(s => s.key === this.currentStage);

        if (stageIndex < currentIndex) return 'completed';
        if (stageIndex === currentIndex) return 'current';
        return 'pending';
    }

    getStageEntry(stageKey: string): any {
        return this.workflowHistory.slice().reverse().find(h => h.stage === stageKey);
    }

    selectStage(stageKey: string): void {
        const entry = this.getStageEntry(stageKey);
        if (entry) {
            this.selectedStage = entry;
        }
    }

    closePopup(): void {
        this.selectedStage = null;
    }

    goBack(): void {
        this.router.navigate(['/transactions/sale-orders']);
    }
}
