import { Component, OnInit } from '@angular/core';
import { VoucherService } from '../../../services/account.service';
import { Voucher } from '../../../models/account.model';
import { NotificationService } from '../../../services/notification.service';

@Component({
    selector: 'app-voucher-management',
    templateUrl: './voucher-management.component.html',
    styleUrls: ['./voucher-management.component.css'],
    standalone: false,
})
export class VoucherManagementComponent implements OnInit {
    vouchers: Voucher[] = [];
    loading = false;
    selectedVoucher: Voucher | null = null;

    voucherTypes = ['Journal', 'Cash', 'Bank'];
    selectedType = '';

    constructor(
        private voucherService: VoucherService,
        private notificationService: NotificationService
    ) {}

    ngOnInit(): void {
        this.loadVouchers();
    }

    loadVouchers(): void {
        this.loading = true;
        if (this.selectedType) {
            this.voucherService.getVouchersByType(this.selectedType).subscribe({
                next: (vouchers) => {
                    this.vouchers = vouchers;
                    this.loading = false;
                },
                error: (error) => {
                    console.error('Error loading vouchers:', error);
                    this.loading = false;
                },
            });
        } else {
            this.voucherService.getVouchers().subscribe({
                next: (response) => {
                    this.vouchers = response.items || [];
                    this.loading = false;
                },
                error: (error) => {
                    console.error('Error loading vouchers:', error);
                    this.loading = false;
                },
            });
        }
    }

    viewVoucher(voucher: Voucher): void {
        this.voucherService.getVoucher(voucher.id).subscribe({
            next: (fullVoucher) => {
                this.selectedVoucher = fullVoucher;
            },
            error: (error) => {
                console.error('Error loading voucher details:', error);
                this.notificationService.showError('Error loading voucher details.');
            },
        });
    }

    closeVoucherDetails(): void {
        this.selectedVoucher = null;
    }

    deleteVoucher(voucher: Voucher): void {
        this.notificationService
            .showConfirm(
                `Are you sure you want to delete voucher ${voucher.voucherNumber}? This will reverse the accounting entries.`,
                'Delete Voucher',
                'Yes, delete it!',
                'Cancel'
            )
            .then((confirmed) => {
                if (confirmed) {
                    this.voucherService.deleteVoucher(voucher.id).subscribe({
                        next: () => {
                            this.loadVouchers();
                            this.notificationService.showSuccess('Voucher deleted and entries reversed successfully!');
                        },
                        error: (error) => {
                            console.error('Error deleting voucher:', error);
                            this.notificationService.showError('Error deleting voucher. Please try again.');
                        },
                    });
                }
            });
    }

    filterByType(): void {
        this.loadVouchers();
    }

    getTotalDebit(voucher: Voucher): number {
        if (!voucher.journalEntries) return 0;
        return voucher.journalEntries
            .filter(e => e.entryType === 'Debit')
            .reduce((sum, e) => sum + e.amount, 0);
    }

    getTotalCredit(voucher: Voucher): number {
        if (!voucher.journalEntries) return 0;
        return voucher.journalEntries
            .filter(e => e.entryType === 'Credit')
            .reduce((sum, e) => sum + e.amount, 0);
    }
}
