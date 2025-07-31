import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { BusinessNature, IndustryConfig } from '../models/multi-tenant.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class BusinessNatureService {
    private readonly apiUrl = `${environment.apiUrl}/api/BusinessNatures`;

    constructor(private http: HttpClient) {}

    getAll(): Observable<BusinessNature[]> {
        // Try API first, fallback to mock data
        return this.http.get<BusinessNature[]>(this.apiUrl).pipe(catchError(() => this.getMockBusinessNatures()));
    }

    getById(id: number): Observable<BusinessNature> {
        return this.http.get<BusinessNature>(`${this.apiUrl}/${id}`).pipe(
            catchError(() => {
                const mockData = this.getMockBusinessNaturesData();
                const businessNature = mockData.find((bn) => bn.id === id);
                return of(businessNature!);
            })
        );
    }

    getIndustryConfig(businessNatureId: number): Observable<IndustryConfig> {
        return this.http.get<IndustryConfig>(`${this.apiUrl}/${businessNatureId}/config`);
    }

    private getMockBusinessNatures(): Observable<BusinessNature[]> {
        return of(this.getMockBusinessNaturesData());
    }

    private getMockBusinessNaturesData(): BusinessNature[] {
        return [
            {
                id: 1,
                name: 'digital_marketing',
                displayName: 'Digital Marketing',
                description:
                    'Digital marketing agencies, online advertising, social media management, and digital content creation services',
                isActive: true,
                createdDate: new Date('2024-01-01'),
            },
            {
                id: 2,
                name: 'automobile',
                displayName: 'Automobile Industry',
                description:
                    'Car dealerships, automotive services, parts manufacturing, and vehicle maintenance services',
                isActive: true,
                createdDate: new Date('2024-01-01'),
            },
            {
                id: 3,
                name: 'general',
                displayName: 'General Business',
                description:
                    'General business operations, consulting, professional services, and other commercial activities',
                isActive: true,
                createdDate: new Date('2024-01-01'),
            },
            {
                id: 4,
                name: 'retail',
                displayName: 'Retail & E-commerce',
                description:
                    'Traditional retail stores, online marketplaces, e-commerce platforms, and consumer goods sales',
                isActive: true,
                createdDate: new Date('2024-01-01'),
            },
            {
                id: 5,
                name: 'manufacturing',
                displayName: 'Manufacturing',
                description:
                    'Production facilities, industrial manufacturing, product assembly, and supply chain operations',
                isActive: true,
                createdDate: new Date('2024-01-01'),
            },
            {
                id: 6,
                name: 'services',
                displayName: 'Service Industry',
                description:
                    'Professional services, consulting, healthcare, education, and other service-based businesses',
                isActive: true,
                createdDate: new Date('2024-01-01'),
            },
            {
                id: 7,
                name: 'hospitality',
                displayName: 'Hospitality & Tourism',
                description: 'Hotels, restaurants, travel agencies, tourism services, and hospitality management',
                isActive: true,
                createdDate: new Date('2024-01-01'),
            },
        ];
    }
}
