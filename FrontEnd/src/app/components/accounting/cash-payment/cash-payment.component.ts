import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { AccountService, VoucherService } from '../../../services/account.service';
import { Account } from '../../../models/account.model';
import { NotificationService } from '../../../services/notification.service';

@Component({
    selector: 'app-cash-payment',
    templateUrl: './cash-payment.component.html',
    styleUrls: ['./cash-payment.component.css'],
    standalone: false,
})
export class CashPaymentComponent implements OnInit {
    voucherForm: FormGroup;
    accounts: Account[] = [];
    cashAccounts: Account[] = [];
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
            cashAccountId: [null, Validators.required],
            paymentToAccountId: [null, Validators.required],
            amount: [0, [Validators.required, Validators.min(0.01)]],
            narration: ['']
        });
    }

    ngOnInit(): void {
        this.loadAccounts();
    }

    loadAccounts(): void {
        this.accountService.getAccounts().subscribe({
            next: (accounts) => {
                this.accounts = accounts;
                this.cashAccounts = accounts.filter(a => 
                    a.accountName.toLowerCase().includes('cash') || 
                    a.accountType === 'Asset'
                );
            },
            error: (error) => {
                console.error('Error loading accounts:', error);
                this.notificationService.showError('Error loading accounts');
            }
        });
    }

    onSubmit(): void {
        if (this.voucherForm.invalid) {
            this.notificationService.showError('Please fill all required fields');
            return;
        }

        this.loading = true;
        const formValue = this.voucherForm.value;
        
        const voucherData = {
            voucherNumber: formValue.voucherNumber,
            voucherType: 'Cash',
            voucherDate: formValue.voucherDate,
            referenceNumber: formValue.referenceNumber,
            description: formValue.description || 'Cash Payment',
            journalEntries: [
                {
                    accountId: formValue.paymentToAccountId,
                    entryType: 'Debit',
                    amount: parseFloat(formValue.amount),
                    description: formValue.narration || 'Payment'
                },
                {
                    accountId: formValue.cashAccountId,
                    entryType: 'Credit',
                    amount: parseFloat(formValue.amount),
                    description: formValue.narration || 'Cash payment made'
                }
            ]
        };

        this.voucherService.createVoucher(voucherData).subscribe({
            next: (response) => {
                this.notificationService.showSuccess('Cash payment voucher created successfully!');
                this.resetForm();
                this.loading = false;
            },
            error: (error) => {
                console.error('Error creating voucher:', error);
                this.notificationService.showError('Error creating cash payment voucher. Please try again.');
                this.loading = false;
            }
        });
    }

    resetForm(): void {
        this.voucherForm.reset({
            voucherDate: new Date().toISOString().split('T')[0],
            amount: 0
        });
    }
}
