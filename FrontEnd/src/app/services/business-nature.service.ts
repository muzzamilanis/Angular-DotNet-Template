import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { BusinessNature, IndustryConfig } from '../models/multi-tenant.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BusinessNatureService {
  private readonly apiUrl = `${environment.apiUrl}/api/BusinessNatures`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<BusinessNature[]> {
    // Mock data for demonstration
    const mockBusinessNatures: BusinessNature[] = [
      {
        id: 1,
        name: 'retail',
        displayName: 'Retail & E-commerce',
        description: 'Traditional retail and online commerce operations',
        isActive: true,
        createdDate: new Date()
      },
      {
        id: 2,
        name: 'manufacturing',
        displayName: 'Manufacturing',
        description: 'Manufacturing and production businesses',
        isActive: true,
        createdDate: new Date()
      },
      {
        id: 3,
        name: 'services',
        displayName: 'Service Industry',
        description: 'Professional and consulting services',
        isActive: true,
        createdDate: new Date()
      },
      {
        id: 4,
        name: 'hospitality',
        displayName: 'Hospitality & Tourism',
        description: 'Hotels, restaurants, and tourism services',
        isActive: true,
        createdDate: new Date()
      },
      {
        id: 5,
        name: 'digital-marketing',
        displayName: 'Digital Marketing',
        description: 'Digital marketing and content creation services',
        isActive: true,
        createdDate: new Date()
      }
    ];

    return of(mockBusinessNatures);
    // Original implementation:
    // return this.http.get<BusinessNature[]>(this.apiUrl);
  }

  getById(id: number): Observable<BusinessNature> {
    return this.http.get<BusinessNature>(`${this.apiUrl}/${id}`);
  }

  getIndustryConfig(businessNatureId: number): Observable<IndustryConfig> {
    return this.http.get<IndustryConfig>(`${this.apiUrl}/${businessNatureId}/config`);
  }
}