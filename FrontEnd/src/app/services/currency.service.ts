import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Currency, CreateCurrencyDto, UpdateCurrencyDto, ExchangeRateResponse } from '../models/currency.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class CurrencyService {
    private apiUrl = `${environment.apiUrl}/Currency`;

    constructor(private http: HttpClient) {}

    getAll(): Observable<Currency[]> {
        return this.http.get<Currency[]>(this.apiUrl);
    }

    getById(id: number): Observable<Currency> {
        return this.http.get<Currency>(`${this.apiUrl}/${id}`);
    }

    create(currency: CreateCurrencyDto): Observable<Currency> {
        return this.http.post<Currency>(this.apiUrl, currency);
    }

    update(id: number, currency: UpdateCurrencyDto): Observable<Currency> {
        return this.http.put<Currency>(`${this.apiUrl}/${id}`, currency);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

    getExchangeRate(currencyCode: string): Observable<ExchangeRateResponse> {
        return this.http.get<ExchangeRateResponse>(`${this.apiUrl}/rate?code=${currencyCode}`);
    }
}