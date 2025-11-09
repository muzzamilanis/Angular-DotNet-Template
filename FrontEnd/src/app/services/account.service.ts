import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Account, CreateAccountDto, Voucher, CreateVoucherDto } from '../models/account.model';
import { PaginatedResponse } from '../models/common.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class AccountService {
    private apiUrl = environment.apiUrl + '/accounts';

    constructor(private http: HttpClient) {}

    getAccounts(): Observable<Account[]> {
        return this.http.get<Account[]>(this.apiUrl);
    }

    getAccountsByType(type: string): Observable<Account[]> {
        return this.http.get<Account[]>(`${this.apiUrl}/by-type/${type}`);
    }

    getAccount(id: number): Observable<Account> {
        return this.http.get<Account>(`${this.apiUrl}/${id}`);
    }

    createAccount(account: CreateAccountDto): Observable<Account> {
        return this.http.post<Account>(this.apiUrl, account);
    }

    updateAccount(id: number, account: CreateAccountDto): Observable<Account> {
        return this.http.put<Account>(`${this.apiUrl}/${id}`, account);
    }

    deleteAccount(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

    initializeDefaultAccounts(): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/initialize`, {});
    }
}

@Injectable({
    providedIn: 'root',
})
export class VoucherService {
    private apiUrl = environment.apiUrl + '/vouchers';

    constructor(private http: HttpClient) {}

    getVouchers(): Observable<PaginatedResponse<Voucher>> {
        return this.http.get<PaginatedResponse<Voucher>>(this.apiUrl);
    }

    getVouchersByType(type: string): Observable<Voucher[]> {
        return this.http.get<Voucher[]>(`${this.apiUrl}/by-type/${type}`);
    }

    getVoucher(id: number): Observable<Voucher> {
        return this.http.get<Voucher>(`${this.apiUrl}/${id}`);
    }

    createVoucher(voucher: CreateVoucherDto): Observable<Voucher> {
        return this.http.post<Voucher>(this.apiUrl, voucher);
    }

    deleteVoucher(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}
