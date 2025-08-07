import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Lead, CreateLeadDto, UpdateLeadDto } from '../../models/crm/crm.model';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class LeadService {
    private apiUrl = environment.apiUrl + '/leads';

    constructor(private http: HttpClient) {}

    getLeads(skipCount: number = 0, maxResultCount: number = 10, filter?: string): Observable<any> {
        let params = new HttpParams()
            .set('skipCount', skipCount.toString())
            .set('maxResultCount', maxResultCount.toString());
        
        if (filter) {
            params = params.set('filter', filter);
        }

        return this.http.get<any>(this.apiUrl, { params });
    }

    getActiveLeads(): Observable<Lead[]> {
        return this.http.get<Lead[]>(`${this.apiUrl}/active`);
    }

    getLead(id: number): Observable<Lead> {
        return this.http.get<Lead>(`${this.apiUrl}/${id}`);
    }

    createLead(lead: CreateLeadDto): Observable<Lead> {
        return this.http.post<Lead>(this.apiUrl, lead);
    }

    updateLead(id: number, lead: UpdateLeadDto): Observable<Lead> {
        return this.http.put<Lead>(`${this.apiUrl}/${id}`, lead);
    }

    deleteLead(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}