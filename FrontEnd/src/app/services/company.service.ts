import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { Company, CreateCompanyDto, UpdateCompanyDto } from '../models/company.model';
import { CompanyProfile, CreateCompanyProfileRequest, BusinessNature } from '../models/multi-tenant.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private apiUrl = `${environment.apiUrl}/services/app/Company`;
  private readonly profileApiUrl = `${environment.apiUrl}/Company`;
  private readonly companiesApiUrl = `${environment.apiUrl}/Companies`;

  constructor(private http: HttpClient) { }

  // Get current user's company
  getCurrentCompany(): Observable<Company> {
    return this.http.get<Company>(`${this.companiesApiUrl}/current`);
  }

  // Company Profile methods (from saas-implementation)
  getProfile(): Observable<CompanyProfile> {
    // Mock implementation for demo
    const mockProfile: CompanyProfile = {
      id: 1,
      tenantId: 1,
      businessNatureId: 1,
      companyName: 'Demo Company',
      address: '123 Demo Street',
      phone: '+1-555-0123',
      email: 'demo@company.com',
      isActive: true,
      createdDate: new Date(),
      businessNature: {
        id: 1,
        name: 'retail',
        displayName: 'Retail & E-commerce',
        description: 'Traditional retail and online commerce operations',
        isActive: true,
        createdDate: new Date()
      }
    };

    return of(mockProfile);
    // Original implementation:
    // return this.http.get<CompanyProfile>(`${this.profileApiUrl}/profile`);
  }

  createProfile(request: CreateCompanyProfileRequest): Observable<CompanyProfile> {
    // Mock implementation for demo
    const newProfile: CompanyProfile = {
      id: Date.now(),
      tenantId: 1,
      businessNatureId: request.businessNatureId,
      companyName: request.companyName,
      address: request.address,
      phone: request.phone,
      email: request.email,
      customFields: request.customFields,
      isActive: true,
      createdDate: new Date()
    };

    return of(newProfile);
    // Original implementation:
    // return this.http.post<CompanyProfile>(`${this.profileApiUrl}/profile`, request);
  }

  updateProfile(request: CreateCompanyProfileRequest): Observable<CompanyProfile> {
    return this.http.put<CompanyProfile>(`${this.profileApiUrl}/profile`, request);
  }

  getAll(): Observable<Company[]> {
    // Mock data for demonstration
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
        createdDate: new Date('2024-01-01')
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
        createdDate: new Date('2024-02-15')
      }
    ];
    
    return new Observable(observer => {
      observer.next(mockCompanies);
      observer.complete();
    });
    
    // Original implementation (commented for demo):
    // return this.http.get<Company[]>(`${this.apiUrl}/GetAll`);
  }

  getById(id: number): Observable<Company> {
    return this.http.get<Company>(`${this.apiUrl}/Get?id=${id}`);
  }

  create(company: CreateCompanyDto): Observable<Company> {
    // Mock implementation for demonstration
    const newCompany: Company = {
      id: Date.now(), // Mock ID
      ...company,
      isActive: true,
      createdDate: new Date()
    };
    
    return new Observable(observer => {
      // Simulate API delay
      setTimeout(() => {
        observer.next(newCompany);
        observer.complete();
      }, 500);
    });
    
    // Original implementation (commented for demo):
    // return this.http.post<Company>(`${this.apiUrl}/Create`, company);
  }

  update(company: UpdateCompanyDto): Observable<Company> {
    return this.http.put<Company>(`${this.apiUrl}/Update`, company);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Delete?id=${id}`);
  }

  activate(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/Activate`, { id });
  }

  deactivate(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/Deactivate`, { id });
  }
}