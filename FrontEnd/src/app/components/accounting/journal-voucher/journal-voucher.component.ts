import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { AccountService, VoucherService } from '../../../services/account.service';
import { Account } from '../../../models/account.model';
import { NotificationService } from '../../../services/notification.service';

@Component({
    selector: 'app-journal-voucher',
    templateUrl: './journal-voucher.component.html',
    styleUrls: ['./journal-voucher.component.css'],
    standalone: false,
})
export class JournalVoucherComponent implements OnInit {
    voucherForm: FormGroup;
    accounts: Account[] = [];
    loading = false;

    constructor(
        private fb: FormBuilder,
        private accountService: AccountService,
        private voucherService: VoucherService,
        private notificationService: NotificationService
    ) {
        this.voucherForm = this.fb.group({
            voucherNumber: ['', Validators.required],
            voucherDate: [new Date().toISOString().split('T')[0], Validators.required],
            referenceNumber: [''],
            description: [''],
            entries: this.fb.array([])
        });
    }

    ngOnInit(): void {
        this.loadAccounts();
        this.addEntry(); // Add first entry
        this.addEntry(); // Add second entry
    }

    get entries(): FormArray {
        return this.voucherForm.get('entries') as FormArray;
    }

    loadAccounts(): void {
        this.accountService.getAccounts().subscribe({
            next: (accounts) => {
                this.accounts = accounts;
            },
            error: (error) => {
                console.error('Error loading accounts:', error);
                this.notificationService.showError('Error loading accounts');
            }
        });
    }

    createEntry(): FormGroup {
        return this.fb.group({
            accountId: [null, Validators.required],
            entryType: ['Debit', Validators.required],
            amount: [0, [Validators.required, Validators.min(0)]],
            description: ['']
        });
    }

    addEntry(): void {
        this.entries.push(this.createEntry());
    }

    removeEntry(index: number): void {
        if (this.entries.length > 2) {
            this.entries.removeAt(index);
        } else {
            this.notificationService.showWarning('At least two entries are required');
        }
    }

    getTotalDebit(): number {
        let total = 0;
        this.entries.controls.forEach(control => {
            const entryType = control.get('entryType')?.value;
            const amount = control.get('amount')?.value || 0;
            if (entryType === 'Debit') {
                total += parseFloat(amount);
            }
        });
        return total;
    }

    getTotalCredit(): number {
        let total = 0;
        this.entries.controls.forEach(control => {
            const entryType = control.get('entryType')?.value;
            const amount = control.get('amount')?.value || 0;
            if (entryType === 'Credit') {
                total += parseFloat(amount);
            }
        });
        return total;
    }

    isBalanced(): boolean {
        const debit = this.getTotalDebit();
        const credit = this.getTotalCredit();
        return Math.abs(debit - credit) < 0.01; // Allow for small floating point differences
    }

    onSubmit(): void {
        if (this.voucherForm.invalid) {
            this.notificationService.showError('Please fill all required fields');
            return;
        }

        if (!this.isBalanced()) {
            this.notificationService.showError('Voucher must be balanced (Total Debit = Total Credit)');
            return;
        }

        this.loading = true;
        const formValue = this.voucherForm.value;
        
        const voucherData = {
            voucherNumber: formValue.voucherNumber,
            voucherType: 'Journal',
            voucherDate: formValue.voucherDate,
            referenceNumber: formValue.referenceNumber,
            description: formValue.description,
            journalEntries: formValue.entries.map((entry: any) => ({
                accountId: entry.accountId,
                entryType: entry.entryType,
                amount: parseFloat(entry.amount),
                description: entry.description
            }))
        };

        this.voucherService.createVoucher(voucherData).subscribe({
            next: (response) => {
                this.notificationService.showSuccess('Journal voucher created successfully!');
                this.resetForm();
                this.loading = false;
            },
            error: (error) => {
                console.error('Error creating voucher:', error);
                this.notificationService.showError('Error creating journal voucher. Please try again.');
                this.loading = false;
            }
        });
    }

    resetForm(): void {
        this.voucherForm.reset({
            voucherDate: new Date().toISOString().split('T')[0]
        });
        this.entries.clear();
        this.addEntry();
        this.addEntry();
    }
}
