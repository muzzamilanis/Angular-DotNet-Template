import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Company, CreateCompanyDto, UpdateCompanyDto } from '../models/company.model';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private apiUrl = 'https://localhost:44311/api/services/app/Company';

  constructor(private http: HttpClient) { }

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