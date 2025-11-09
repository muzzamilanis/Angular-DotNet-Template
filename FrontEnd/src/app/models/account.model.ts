export interface Account {
    id: number;
    accountCode: string;
    accountName: string;
    accountType: string;
    accountCategory?: string;
    parentAccountId?: number;
    level: number;
    openingBalance: number;
    currentBalance: number;
    description?: string;
    isActive: boolean;
    companyId: number;
}

export interface CreateAccountDto {
    accountCode: string;
    accountName: string;
    accountType: string;
    accountCategory?: string;
    parentAccountId?: number;
    openingBalance: number;
    description?: string;
}

export interface Voucher {
    id: number;
    voucherNumber: string;
    voucherType: string;
    voucherDate: Date;
    referenceNumber?: string;
    description?: string;
    totalAmount: number;
    status?: string;
    isPosted: boolean;
    postedDate?: Date;
    postedByUserId?: number;
    companyId: number;
    journalEntries: JournalEntry[];
}

export interface CreateVoucherDto {
    voucherNumber: string;
    voucherType: string;
    voucherDate: Date;
    referenceNumber?: string;
    description?: string;
    journalEntries: CreateJournalEntryDto[];
}

export interface JournalEntry {
    id: number;
    voucherId: number;
    accountId: number;
    accountName: string;
    entryType: string;
    amount: number;
    description?: string;
    entryDate: Date;
}

export interface CreateJournalEntryDto {
    accountId: number;
    entryType: string;
    amount: number;
    description?: string;
}
