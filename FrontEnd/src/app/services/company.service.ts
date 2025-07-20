import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CompanyProfile, CreateCompanyProfileRequest } from '../models/multi-tenant.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private readonly apiUrl = `${environment.apiUrl}/api/Company`;

  constructor(private http: HttpClient) {}

  getProfile(): Observable<CompanyProfile> {
    return this.http.get<CompanyProfile>(`${this.apiUrl}/profile`);
  }

  createProfile(request: CreateCompanyProfileRequest): Observable<CompanyProfile> {
    return this.http.post<CompanyProfile>(`${this.apiUrl}/profile`, request);
  }

  updateProfile(request: CreateCompanyProfileRequest): Observable<CompanyProfile> {
    return this.http.put<CompanyProfile>(`${this.apiUrl}/profile`, request);
  }
}