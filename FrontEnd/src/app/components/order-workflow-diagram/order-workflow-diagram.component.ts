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

    /** Narration lines the bot types out when a new stage arrives live */
    liveFlash: string | null = null;
    private liveFlashTimeout: any = null;

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
        if (this.liveFlashTimeout) clearTimeout(this.liveFlashTimeout);
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
        this.orderWorkflowService.onWorkflowUpdated((data: any) => {
            this.currentStage = data.stage;
            this.workflowHistory.push(data);
            this.flashLive(`Stage updated → ${this.stageLabel(data.stage)}`);
        });
    }

    private flashLive(msg: string): void {
        this.liveFlash = msg;
        if (this.liveFlashTimeout) clearTimeout(this.liveFlashTimeout);
        this.liveFlashTimeout = setTimeout(() => { this.liveFlash = null; }, 4000);
    }

    getStageStatus(stageKey: string): 'completed' | 'current' | 'pending' {
        const stageIndex  = this.stages.findIndex(s => s.key === stageKey);
        const currentIndex = this.stages.findIndex(s => s.key === this.currentStage);
        if (stageIndex < currentIndex)  return 'completed';
        if (stageIndex === currentIndex) return 'current';
        return 'pending';
    }

    getStageEntry(stageKey: string): any {
        return this.workflowHistory.slice().reverse().find(h => h.stage === stageKey);
    }

    selectStage(stageKey: string): void {
        const entry = this.getStageEntry(stageKey);
        if (entry) this.selectedStage = entry;
    }

    closePopup(): void { this.selectedStage = null; }

    goBack(): void { this.router.navigate(['/transactions/sale-orders']); }

    stageLabel(key: string): string {
        return this.stages.find(s => s.key === key)?.label ?? key;
    }

    stageIcon(key: string): string {
        return this.stages.find(s => s.key === key)?.icon ?? 'fas fa-circle';
    }

    /** Human-readable bot message for each history entry */
    botMessage(entry: any): string {
        const by = entry.updatedBy ?? 'System';
        const label = this.stageLabel(entry.stage);
        const messages: Record<string, string> = {
            OrderLogged:     `✅ Order has been received and logged by ${by}. The AI has captured all details.`,
            WriterAssigned:  `✍️ A writer has been assigned to this order by ${by}. Work will begin shortly.`,
            InProgress:      `🔄 The writer is actively working on this order. Updated by ${by}.`,
            UnderReview:     `🔍 Work submitted. ${by} is reviewing the draft for quality assurance.`,
            Delivered:       `🎉 Order delivered successfully by ${by}. Mission complete!`,
        };
        return messages[entry.stage] ?? `📌 Stage "${label}" was updated by ${by}.`;
    }

    timeAgo(dateStr: string): string {
        const diff = Date.now() - new Date(dateStr).getTime();
        const mins = Math.floor(diff / 60000);
        if (mins < 1)  return 'just now';
        if (mins < 60) return `${mins}m ago`;
        const hrs = Math.floor(mins / 60);
        if (hrs < 24)  return `${hrs}h ago`;
        const days = Math.floor(hrs / 24);
        return `${days}d ago`;
    }

    get progressPercent(): number {
        if (!this.currentStage) return 0;
        const idx = this.stages.findIndex(s => s.key === this.currentStage);
        return Math.round(((idx + 1) / this.stages.length) * 100);
    }
}

