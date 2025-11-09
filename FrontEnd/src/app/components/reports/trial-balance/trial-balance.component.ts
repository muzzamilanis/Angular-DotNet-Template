import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AccountService } from '../../../services/account.service';
import { Account } from '../../../models/account.model';
import { NotificationService } from '../../../services/notification.service';

interface TrialBalanceEntry {
    accountCode: string;
    accountName: string;
    accountType: string;
    debit: number;
    credit: number;
}

@Component({
    selector: 'app-trial-balance',
    templateUrl: './trial-balance.component.html',
    styleUrls: ['./trial-balance.component.css'],
    standalone: false,
})
export class TrialBalanceComponent implements OnInit {
    filterForm: FormGroup;
    trialBalanceEntries: TrialBalanceEntry[] = [];
    loading = false;
    
    totalDebits = 0;
    totalCredits = 0;
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
                this.processTrialBalance(accounts);
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading accounts:', error);
                this.loading = false;
                this.notificationService.showError('Error generating report');
            },
        });
    }

    private processTrialBalance(accounts: Account[]): void {
        this.trialBalanceEntries = [];
        this.totalDebits = 0;
        this.totalCredits = 0;

        accounts.forEach(account => {
            const balance = account.currentBalance;
            
            let debit = 0;
            let credit = 0;

            // For debit balance accounts (Assets, Expenses)
            if (['Asset', 'Expense'].includes(account.accountType)) {
                if (balance >= 0) {
                    debit = balance;
                } else {
                    credit = Math.abs(balance);
                }
            } 
            // For credit balance accounts (Liabilities, Equity, Revenue)
            else {
                if (balance >= 0) {
                    credit = balance;
                } else {
                    debit = Math.abs(balance);
                }
            }

            if (debit !== 0 || credit !== 0) {
                this.trialBalanceEntries.push({
                    accountCode: account.accountCode,
                    accountName: account.accountName,
                    accountType: account.accountType,
                    debit: debit,
                    credit: credit,
                });

                this.totalDebits += debit;
                this.totalCredits += credit;
            }
        });

        this.isBalanced = Math.abs(this.totalDebits - this.totalCredits) < 0.01;
    }

    exportToExcel(): void {
        if (this.trialBalanceEntries.length === 0) {
            this.notificationService.showWarning('No data to export');
            return;
        }

        const data = this.trialBalanceEntries.map(entry => ({
            'Account Code': entry.accountCode,
            'Account Name': entry.accountName,
            'Account Type': entry.accountType,
            'Debit': entry.debit.toFixed(2),
            'Credit': entry.credit.toFixed(2),
        }));

        this.downloadCSV(data, `trial-balance-${new Date().toISOString().split('T')[0]}.csv`);
        this.notificationService.showSuccess('Report exported successfully');
    }

    exportToPDF(): void {
        if (this.trialBalanceEntries.length === 0) {
            this.notificationService.showWarning('No data to export');
            return;
        }

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
