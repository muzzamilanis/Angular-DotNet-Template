import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AccountService } from '../../../services/account.service';
import { Account } from '../../../models/account.model';
import { NotificationService } from '../../../services/notification.service';

interface BalanceSheetSection {
    accounts: { code: string; name: string; balance: number }[];
    total: number;
}

@Component({
    selector: 'app-balance-sheet',
    templateUrl: './balance-sheet.component.html',
    styleUrls: ['./balance-sheet.component.css'],
    standalone: false,
})
export class BalanceSheetComponent implements OnInit {
    filterForm: FormGroup;
    loading = false;
    
    assets: BalanceSheetSection = { accounts: [], total: 0 };
    liabilities: BalanceSheetSection = { accounts: [], total: 0 };
    equity: BalanceSheetSection = { accounts: [], total: 0 };
    
    totalLiabilitiesAndEquity = 0;
    isBalanced = false;

    constructor(
        private fb: FormBuilder,
        private accountService: AccountService,
        private notificationService: NotificationService
    ) {
        this.filterForm = this.fb.group({
            asOfDate: [new Date().toISOString().split('T')[0]],
        });
    }

    ngOnInit(): void {
        this.generateReport();
    }

    generateReport(): void {
        this.loading = true;
        this.accountService.getAccounts().subscribe({
            next: (accounts) => {
                this.processBalanceSheet(accounts);
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading accounts:', error);
                this.loading = false;
                this.notificationService.showError('Error generating report');
            },
        });
    }

    private processBalanceSheet(accounts: Account[]): void {
        this.assets = { accounts: [], total: 0 };
        this.liabilities = { accounts: [], total: 0 };
        this.equity = { accounts: [], total: 0 };

        accounts.forEach(account => {
            const balance = account.currentBalance;
            
            if (account.accountType === 'Asset' && balance !== 0) {
                this.assets.accounts.push({
                    code: account.accountCode,
                    name: account.accountName,
                    balance: balance,
                });
                this.assets.total += balance;
            } else if (account.accountType === 'Liability' && balance !== 0) {
                this.liabilities.accounts.push({
                    code: account.accountCode,
                    name: account.accountName,
                    balance: balance,
                });
                this.liabilities.total += balance;
            } else if (account.accountType === 'Equity' && balance !== 0) {
                this.equity.accounts.push({
                    code: account.accountCode,
                    name: account.accountName,
                    balance: balance,
                });
                this.equity.total += balance;
            }
        });

        this.totalLiabilitiesAndEquity = this.liabilities.total + this.equity.total;
        this.isBalanced = Math.abs(this.assets.total - this.totalLiabilitiesAndEquity) < 0.01;
    }

    exportToExcel(): void {
        const data: any[] = [];
        
        // Assets
        data.push({ 'Section': 'ASSETS', 'Account Code': '', 'Account Name': '', 'Balance': '' });
        this.assets.accounts.forEach(acc => {
            data.push({ 'Section': '', 'Account Code': acc.code, 'Account Name': acc.name, 'Balance': acc.balance.toFixed(2) });
        });
        data.push({ 'Section': '', 'Account Code': '', 'Account Name': 'Total Assets', 'Balance': this.assets.total.toFixed(2) });
        
        // Liabilities
        data.push({ 'Section': 'LIABILITIES', 'Account Code': '', 'Account Name': '', 'Balance': '' });
        this.liabilities.accounts.forEach(acc => {
            data.push({ 'Section': '', 'Account Code': acc.code, 'Account Name': acc.name, 'Balance': acc.balance.toFixed(2) });
        });
        data.push({ 'Section': '', 'Account Code': '', 'Account Name': 'Total Liabilities', 'Balance': this.liabilities.total.toFixed(2) });
        
        // Equity
        data.push({ 'Section': 'EQUITY', 'Account Code': '', 'Account Name': '', 'Balance': '' });
        this.equity.accounts.forEach(acc => {
            data.push({ 'Section': '', 'Account Code': acc.code, 'Account Name': acc.name, 'Balance': acc.balance.toFixed(2) });
        });
        data.push({ 'Section': '', 'Account Code': '', 'Account Name': 'Total Equity', 'Balance': this.equity.total.toFixed(2) });

        this.downloadCSV(data, `balance-sheet-${new Date().toISOString().split('T')[0]}.csv`);
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
