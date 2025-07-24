import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BusinessNature, IndustryConfig } from '../models/multi-tenant.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BusinessNatureService {
  private readonly apiUrl = `${environment.apiUrl}/api/BusinessNatures`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<BusinessNature[]> {
    return this.http.get<BusinessNature[]>(this.apiUrl);
  }

  getById(id: number): Observable<BusinessNature> {
    return this.http.get<BusinessNature>(`${this.apiUrl}/${id}`);
  }

  getIndustryConfig(businessNatureId: number): Observable<IndustryConfig> {
    return this.http.get<IndustryConfig>(`${this.apiUrl}/${businessNatureId}/config`);
  }
}