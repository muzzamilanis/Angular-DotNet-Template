import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../../../services/account.service';
import { Account, CreateAccountDto } from '../../../models/account.model';
import { NotificationService } from '../../../services/notification.service';

@Component({
    selector: 'app-chart-of-accounts',
    templateUrl: './chart-of-accounts.component.html',
    styleUrls: ['./chart-of-accounts.component.css'],
    standalone: false,
})
export class ChartOfAccountsComponent implements OnInit {
    accountForm: FormGroup;
    accounts: Account[] = [];
    editingAccount: Account | null = null;
    loading = false;

    accountTypes = ['Asset', 'Liability', 'Equity', 'Revenue', 'Expense'];
    accountCategories = [
        'Main', 'Current Asset', 'Fixed Asset', 'Current Liability', 'Long-term Liability',
        "Owner's Equity", 'Operating Revenue', 'Other Revenue', 'Operating Expense', 'Other Expense'
    ];

    constructor(
        private fb: FormBuilder,
        private accountService: AccountService,
        private notificationService: NotificationService
    ) {
        this.initializeForm();
    }

    ngOnInit(): void {
        this.loadAccounts();
    }

    private initializeForm(): void {
        this.accountForm = this.fb.group({
            accountCode: ['', [Validators.required, Validators.maxLength(20)]],
            accountName: ['', [Validators.required, Validators.maxLength(200)]],
            accountType: ['Asset', [Validators.required]],
            accountCategory: [''],
            openingBalance: [0, [Validators.required]],
            description: [''],
        });
    }

    onSubmit(): void {
        if (this.accountForm.valid) {
            this.loading = true;
            const accountDto: CreateAccountDto = this.accountForm.value;

            if (this.editingAccount) {
                this.accountService.updateAccount(this.editingAccount.id, accountDto).subscribe({
                    next: () => {
                        this.loadAccounts();
                        this.resetForm();
                        this.loading = false;
                        this.notificationService.showSuccess('Account updated successfully!');
                    },
                    error: (error) => {
                        console.error('Error updating account:', error);
                        this.loading = false;
                        this.notificationService.showError(error.error?.message || 'Error updating account. Please try again.');
                    },
                });
            } else {
                this.accountService.createAccount(accountDto).subscribe({
                    next: () => {
                        this.loadAccounts();
                        this.resetForm();
                        this.loading = false;
                        this.notificationService.showSuccess('Account created successfully!');
                    },
                    error: (error) => {
                        console.error('Error creating account:', error);
                        this.loading = false;
                        this.notificationService.showError(error.error?.message || 'Error creating account. Please try again.');
                    },
                });
            }
        }
    }

    editAccount(account: Account): void {
        this.editingAccount = account;
        this.accountForm.patchValue(account);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    deleteAccount(account: Account): void {
        this.notificationService
            .showConfirm(
                `Are you sure you want to delete account "${account.accountName}"?`,
                'Delete Account',
                'Yes, delete it!',
                'Cancel'
            )
            .then((confirmed) => {
                if (confirmed) {
                    this.accountService.deleteAccount(account.id).subscribe({
                        next: () => {
                            this.loadAccounts();
                            this.notificationService.showSuccess('Account deleted successfully!');
                        },
                        error: (error) => {
                            console.error('Error deleting account:', error);
                            this.notificationService.showError(error.error?.message || 'Error deleting account. Please try again.');
                        },
                    });
                }
            });
    }

    initializeDefaultAccounts(): void {
        this.notificationService
            .showConfirm(
                'This will create default accounts for Real Estate business. Continue?',
                'Initialize Accounts',
                'Yes, initialize',
                'Cancel'
            )
            .then((confirmed) => {
                if (confirmed) {
                    this.loading = true;
                    this.accountService.initializeDefaultAccounts().subscribe({
                        next: () => {
                            this.loadAccounts();
                            this.loading = false;
                            this.notificationService.showSuccess('Default accounts initialized successfully!');
                        },
                        error: (error) => {
                            console.error('Error initializing accounts:', error);
                            this.loading = false;
                            this.notificationService.showError('Error initializing accounts. They may already exist.');
                        },
                    });
                }
            });
    }

    resetForm(): void {
        this.editingAccount = null;
        this.accountForm.reset({
            accountType: 'Asset',
            openingBalance: 0,
        });
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

    getAccountsByType(type: string): Account[] {
        return this.accounts.filter(a => a.accountType === type);
    }
}
