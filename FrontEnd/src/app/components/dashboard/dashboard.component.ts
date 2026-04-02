import { Component, OnInit } from '@angular/core';
import { DashboardService, DashboardStats, StatusBreakdown } from '../../services/dashboard.service';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.css'],
    standalone: false,
})
export class DashboardComponent implements OnInit {
    stats: DashboardStats | null = null;
    isLoading = true;
    loadError = false;

    // Palette for donut slices — deterministic by index
    private readonly palette = ['#3498db', '#2ecc71', '#e67e22', '#e74c3c', '#9b59b6', '#1abc9c'];

    constructor(private dashboardService: DashboardService) {}

    ngOnInit(): void {
        this.dashboardService.getStats().subscribe({
            next: (data) => {
                this.stats = data;
                this.isLoading = false;
            },
            error: () => {
                this.isLoading = false;
                this.loadError = true;
            }
        });
    }

    /** Format PKR amounts compactly e.g. 1,250,000 → 12.5L */
    formatRevenue(amount: number): string {
        if (amount >= 10_000_000) return (amount / 10_000_000).toFixed(1) + ' Cr';
        if (amount >= 100_000)    return (amount / 100_000).toFixed(1) + ' L';
        if (amount >= 1_000)      return (amount / 1_000).toFixed(0) + 'K';
        return amount.toFixed(0);
    }

    /** Build conic-gradient string for CSS donut chart */
    buildDonutGradient(byStatus: StatusBreakdown[]): string {
        const total = byStatus.reduce((s, b) => s + b.count, 0);
        if (total === 0) return 'conic-gradient(#dee2e6 0deg 360deg)';
        let deg = 0;
        const parts: string[] = [];
        byStatus.forEach((s, i) => {
            const slice = (s.count / total) * 360;
            parts.push(`${this.palette[i % this.palette.length]} ${deg}deg ${deg + slice}deg`);
            deg += slice;
        });
        return `conic-gradient(${parts.join(', ')})`;
    }

    colorFor(index: number): string {
        return this.palette[index % this.palette.length];
    }

    /** Time-ago string for recent orders */
    timeAgo(dateStr: string): string {
        const diff = Date.now() - new Date(dateStr).getTime();
        const mins = Math.floor(diff / 60000);
        if (mins < 1)   return 'just now';
        if (mins < 60)  return `${mins}m ago`;
        const hrs = Math.floor(mins / 60);
        if (hrs < 24)   return `${hrs}h ago`;
        const days = Math.floor(hrs / 24);
        return `${days}d ago`;
    }

    statusClass(status: string): string {
        switch ((status || '').toLowerCase()) {
            case 'new':         return 'badge-new';
            case 'inprogress':
            case 'in progress': return 'badge-inprogress';
            case 'delivered':   return 'badge-delivered';
            case 'pending':     return 'badge-pending';
            default:            return 'badge-default';
        }
    }
}

