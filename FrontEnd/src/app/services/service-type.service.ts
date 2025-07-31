import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ServiceType, CreateServiceTypeDto } from '../models/service-type.model';
import { PaginatedResponse } from '../models/common.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class ServiceTypeService {
    private apiUrl = `${environment.apiUrl}/servicetypes`;

    constructor(private http: HttpClient) {}

    getServiceTypes(): Observable<PaginatedResponse<ServiceType>> {
        return this.http.get<PaginatedResponse<ServiceType>>(this.apiUrl);
    }

    getServiceTypesByCompany(companyId: number): Observable<ServiceType[]> {
        return this.http.get<ServiceType[]>(`${this.apiUrl}/company/${companyId}`);
    }

    getServiceType(id: number): Observable<ServiceType> {
        return this.http.get<ServiceType>(`${this.apiUrl}/${id}`);
    }

    createServiceType(serviceType: CreateServiceTypeDto): Observable<ServiceType> {
        return this.http.post<ServiceType>(this.apiUrl, serviceType);
    }

    updateServiceType(id: number, serviceType: CreateServiceTypeDto): Observable<ServiceType> {
        return this.http.put<ServiceType>(`${this.apiUrl}/${id}`, serviceType);
    }

    deleteServiceType(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}
