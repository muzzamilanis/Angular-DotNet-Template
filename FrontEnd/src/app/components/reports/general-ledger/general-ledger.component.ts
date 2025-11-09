import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AccountService, VoucherService } from '../../../services/account.service';
import { Account, Voucher, JournalEntry } from '../../../models/account.model';
import { NotificationService } from '../../../services/notification.service';

interface LedgerEntry {
    date: Date;
    voucherNumber: string;
    description: string;
    debit: number;
    credit: number;
    balance: number;
}

@Component({
    selector: 'app-general-ledger',
    templateUrl: './general-ledger.component.html',
    styleUrls: ['./general-ledger.component.css'],
    standalone: false,
})
export class GeneralLedgerComponent implements OnInit {
    filterForm: FormGroup;
    accounts: Account[] = [];
    selectedAccount: Account | null = null;
    ledgerEntries: LedgerEntry[] = [];
    loading = false;
    
    totalDebits = 0;
    totalCredits = 0;
    openingBalance = 0;
    closingBalance = 0;

    constructor(
        private fb: FormBuilder,
        private accountService: AccountService,
        private voucherService: VoucherService,
        private notificationService: NotificationService
    ) {
        this.filterForm = this.fb.group({
            accountId: [null],
            fromDate: [new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0]],
            toDate: [new Date().toISOString().split('T')[0]],
        });
    }

    ngOnInit(): void {
        this.loadAccounts();
    }

    private loadAccounts(): void {
        this.accountService.getAccounts().subscribe({
            next: (accounts) => {
                this.accounts = accounts;
            },
            error: (error) => {
                console.error('Error loading accounts:', error);
            },
        });
    }

    generateReport(): void {
        const accountId = this.filterForm.get('accountId')?.value;
        if (!accountId) {
            this.notificationService.showWarning('Please select an account');
            return;
        }

        this.loading = true;
        this.selectedAccount = this.accounts.find(a => a.id === accountId) || null;

        // Get all vouchers
        this.voucherService.getVouchers().subscribe({
            next: (response) => {
                const vouchers = response.items || [];
                this.processLedgerEntries(vouchers, accountId);
                this.loading = false;
            },
            error: (error) => {
                console.error('Error loading vouchers:', error);
                this.loading = false;
                this.notificationService.showError('Error generating report');
            },
        });
    }

    private processLedgerEntries(vouchers: Voucher[], accountId: number): void {
        this.ledgerEntries = [];
        this.totalDebits = 0;
        this.totalCredits = 0;
        
        const fromDate = new Date(this.filterForm.get('fromDate')?.value);
        const toDate = new Date(this.filterForm.get('toDate')?.value);
        
        // Set opening balance
        this.openingBalance = this.selectedAccount?.openingBalance || 0;
        let runningBalance = this.openingBalance;

        // Filter and process vouchers
        const filteredVouchers = vouchers.filter(v => {
            const vDate = new Date(v.voucherDate);
            return vDate >= fromDate && vDate <= toDate;
        });

        // Sort by date
        filteredVouchers.sort((a, b) => new Date(a.voucherDate).getTime() - new Date(b.voucherDate).getTime());

        // Process each voucher's journal entries
        filteredVouchers.forEach(voucher => {
            const relevantEntries = voucher.journalEntries?.filter(je => je.accountId === accountId) || [];
            
            relevantEntries.forEach(entry => {
                const debit = entry.debitAmount || 0;
                const credit = entry.creditAmount || 0;
                
                runningBalance += debit - credit;
                
                this.ledgerEntries.push({
                    date: new Date(voucher.voucherDate),
                    voucherNumber: voucher.voucherNumber,
                    description: entry.description || voucher.reference || 'Transaction',
                    debit: debit,
                    credit: credit,
                    balance: runningBalance,
                });

                this.totalDebits += debit;
                this.totalCredits += credit;
            });
        });

        this.closingBalance = runningBalance;
    }

    exportToExcel(): void {
        if (!this.selectedAccount || this.ledgerEntries.length === 0) {
            this.notificationService.showWarning('No data to export');
            return;
        }

        const data = this.ledgerEntries.map(entry => ({
            'Date': entry.date.toLocaleDateString(),
            'Voucher': entry.voucherNumber,
            'Description': entry.description,
            'Debit': entry.debit.toFixed(2),
            'Credit': entry.credit.toFixed(2),
            'Balance': entry.balance.toFixed(2),
        }));

        this.downloadCSV(data, `general-ledger-${this.selectedAccount.accountName}-${new Date().toISOString().split('T')[0]}.csv`);
        this.notificationService.showSuccess('Report exported successfully');
    }

    exportToPDF(): void {
        if (!this.selectedAccount || this.ledgerEntries.length === 0) {
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

    clearReport(): void {
        this.selectedAccount = null;
        this.ledgerEntries = [];
        this.filterForm.reset({
            fromDate: new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0],
            toDate: new Date().toISOString().split('T')[0]},
        });
    }
}
