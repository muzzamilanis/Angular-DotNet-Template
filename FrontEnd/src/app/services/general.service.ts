import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CountryOption } from '../models/general.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class GeneralService {
    private apiUrl = `${environment.apiUrl}/General`;

    constructor(private http: HttpClient) {}

    getCountries(): Observable<CountryOption[]> {
        return this.http.get<CountryOption[]>(`${this.apiUrl}/countries`);
    }
}
