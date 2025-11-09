import { Component, OnInit } from '@angular/core';
import { PropertyService, PropertyPurchaseService, PropertySaleService } from '../../../services/property.service';
import { AccountService } from '../../../services/account.service';

@Component({
    selector: 'app-realestate-dashboard',
    templateUrl: './realestate-dashboard.component.html',
    styleUrls: ['./realestate-dashboard.component.css'],
    standalone: false,
})
export class RealEstateDashboardComponent implements OnInit {
    totalProperties = 0;
    totalPurchases = 0;
    totalSales = 0;
    totalValue = 0;
    loading = true;

    constructor(
        private propertyService: PropertyService,
        private purchaseService: PropertyPurchaseService,
        private saleService: PropertySaleService,
        private accountService: AccountService
    ) {}

    ngOnInit(): void {
        this.loadDashboardData();
    }

    private loadDashboardData(): void {
        this.loading = true;

        // Load properties
        this.propertyService.getProperties().subscribe({
            next: (response) => {
                this.totalProperties = response.totalCount || response.items?.length || 0;
                this.totalValue = response.items?.reduce((sum, p) => sum + p.currentValue, 0) || 0;
            },
            error: (error) => console.error('Error loading properties:', error),
        });

        // Load purchases
        this.purchaseService.getPurchases().subscribe({
            next: (response) => {
                this.totalPurchases = response.totalCount || response.items?.length || 0;
            },
            error: (error) => console.error('Error loading purchases:', error),
        });

        // Load sales
        this.saleService.getSales().subscribe({
            next: (response) => {
                this.totalSales = response.totalCount || response.items?.length || 0;
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading sales:', error);
                this.loading = false;
            },
        });
    }
}
