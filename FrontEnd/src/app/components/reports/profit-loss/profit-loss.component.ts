import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AccountService } from '../../../services/account.service';
import { Account } from '../../../models/account.model';
import { NotificationService } from '../../../services/notification.service';

interface PLSection {
    accounts: { code: string; name: string; balance: number }[];
    total: number;
}

@Component({
    selector: 'app-profit-loss',
    templateUrl: './profit-loss.component.html',
    styleUrls: ['./profit-loss.component.css'],
    standalone: false,
})
export class ProfitLossComponent implements OnInit {
    filterForm: FormGroup;
    loading = false;
    
    revenue: PLSection = { accounts: [], total: 0 };
    expenses: PLSection = { accounts: [], total: 0 };
    
    netProfit = 0;
    isProfit = false;
    
    Math = Math; // Make Math available in template

    constructor(
        private fb: FormBuilder,
        private accountService: AccountService,
        private notificationService: NotificationService
    ) {
        this.filterForm = this.fb.group({
            fromDate: [new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0]],
            toDate: [new Date().toISOString().split('T')[0]],
        });
    }

    ngOnInit(): void {
        this.generateReport();
    }

    generateReport(): void {
        this.loading = true;
        this.accountService.getAccounts().subscribe({
            next: (accounts) => {
                this.processProfitLoss(accounts);
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading accounts:', error);
                this.loading = false;
                this.notificationService.showError('Error generating report');
            },
        });
    }

    private processProfitLoss(accounts: Account[]): void {
        this.revenue = { accounts: [], total: 0 };
        this.expenses = { accounts: [], total: 0 };

        accounts.forEach(account => {
            const balance = account.currentBalance;
            
            if (account.accountType === 'Revenue' && balance !== 0) {
                this.revenue.accounts.push({
                    code: account.accountCode,
                    name: account.accountName,
                    balance: balance,
                });
                this.revenue.total += balance;
            } else if (account.accountType === 'Expense' && balance !== 0) {
                this.expenses.accounts.push({
                    code: account.accountCode,
                    name: account.accountName,
                    balance: balance,
                });
                this.expenses.total += balance;
            }
        });

        this.netProfit = this.revenue.total - this.expenses.total;
        this.isProfit = this.netProfit >= 0;
    }

    exportToExcel(): void {
        const data: any[] = [];
        
        // Revenue
        data.push({ 'Section': 'REVENUE', 'Account Code': '', 'Account Name': '', 'Amount': '' });
        this.revenue.accounts.forEach(acc => {
            data.push({ 'Section': '', 'Account Code': acc.code, 'Account Name': acc.name, 'Amount': acc.balance.toFixed(2) });
        });
        data.push({ 'Section': '', 'Account Code': '', 'Account Name': 'Total Revenue', 'Amount': this.revenue.total.toFixed(2) });
        
        // Expenses
        data.push({ 'Section': 'EXPENSES', 'Account Code': '', 'Account Name': '', 'Amount': '' });
        this.expenses.accounts.forEach(acc => {
            data.push({ 'Section': '', 'Account Code': acc.code, 'Account Name': acc.name, 'Amount': acc.balance.toFixed(2) });
        });
        data.push({ 'Section': '', 'Account Code': '', 'Account Name': 'Total Expenses', 'Amount': this.expenses.total.toFixed(2) });
        
        // Net Profit/Loss
        data.push({ 'Section': '', 'Account Code': '', 'Account Name': this.isProfit ? 'Net Profit' : 'Net Loss', 'Amount': Math.abs(this.netProfit).toFixed(2) });

        this.downloadCSV(data, `profit-loss-${new Date().toISOString().split('T')[0]}.csv`);
        this.notificationService.showSuccess('Report exported successfully');
    }

    exportToPDF(): void {
        window.print();
        this.notificationService.showInfo('Use Print dialog to save as PDF');
    }

    private downloadCSV(data: any[], filename: string): void {
        if (data.length === 0) return;

        const headers = Object.keys(data[0]);
        const csvContent = [
            headers.join(','),
            ...data.map(row => headers.map(header => `"${row[header]}"`).join(','))
        ].join('\n');

        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        const url = URL.createObjectURL(blob);
        
        link.setAttribute('href', url);
        link.setAttribute('download', filename);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
}
