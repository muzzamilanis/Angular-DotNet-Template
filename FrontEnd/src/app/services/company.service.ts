import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Company, CreateCompanyDto, UpdateCompanyDto } from '../models/company.model';
import { CompanyProfile, CreateCompanyProfileRequest, BusinessNature } from '../models/multi-tenant.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class CompanyService {
    private apiUrl = `${environment.apiUrl}/Companies`;

    constructor(private http: HttpClient) {}

    // Get current user's company
    getCurrentCompany(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/current`);
    }

    // Company Profile methods
    createProfile(request: CreateCompanyProfileRequest): Observable<any> {
        const createRequest = {
            name: request.companyName,
            address: request.address,
            phone: request.phone,
            email: request.email,
            contactPerson: request.contactPerson,
            businessNatureId: request.businessNatureId,
        };
        return this.http.post<any>(this.apiUrl, createRequest);
    }

    update(id: number, updateRequest: any): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/${id}`, updateRequest);
    }

    getAll(): Observable<Company[]> {
        return this.http.get<Company[]>(this.apiUrl).pipe(catchError(() => of(this.getMockCompanies())));
    }

    getById(id: number): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/${id}`);
    }

    create(company: CreateCompanyDto): Observable<Company> {
        return this.http.post<Company>(this.apiUrl, company);
    }

    uploadLogo(file: File): Observable<any> {
        const formData = new FormData();
        formData.append('file', file);
        return this.http.post<any>(`${this.apiUrl}/upload-logo`, formData);
    }

    getLogo(fileName: string): Observable<Blob> {
        return this.http.get(`${this.apiUrl}/logos/${fileName}`, { responseType: 'blob' });
    }

    private getMockCompanies(): Company[] {
        // Mock data for demonstration - can be replaced with real API call
        const mockCompanies: Company[] = [
            {
                id: 1,
                name: 'Acme Corporation',
                code: 'ACME',
                email: 'contact@acme.com',
                phone: '+1-555-0123',
                address: '123 Business Ave',
                city: 'New York',
                state: 'NY',
                country: 'USA',
                zipCode: '10001',
                website: 'https://acme.com',
                taxNumber: 'TAX123456',
                registrationNumber: 'REG789012',
                isActive: true,
                createdDate: new Date('2024-01-01'),
            },
            {
                id: 2,
                name: 'Tech Solutions Ltd',
                code: 'TECH',
                email: 'info@techsolutions.com',
                phone: '+1-555-0456',
                address: '456 Innovation Blvd',
                city: 'San Francisco',
                state: 'CA',
                country: 'USA',
                zipCode: '94102',
                website: 'https://techsolutions.com',
                taxNumber: 'TAX654321',
                registrationNumber: 'REG210987',
                isActive: true,
                createdDate: new Date('2024-02-15'),
            },
        ];
        return mockCompanies;
    }
}
