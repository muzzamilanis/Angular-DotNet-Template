import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Vendor, CreateVendorDto, UpdateVendorDto } from '../models/vendor.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class VendorService {
    private apiUrl = environment.apiUrl + '/vendors';

    constructor(private http: HttpClient) {}

    getVendors(): Observable<Vendor[]> {
        return this.http.get<Vendor[]>(this.apiUrl);
    }

    getActiveVendors(): Observable<Vendor[]> {
        return this.http.get<Vendor[]>(`${this.apiUrl}/active`);
    }

    getVendor(id: number): Observable<Vendor> {
        return this.http.get<Vendor>(`${this.apiUrl}/${id}`);
    }

    createVendor(vendor: CreateVendorDto): Observable<Vendor> {
        return this.http.post<Vendor>(this.apiUrl, vendor);
    }

    updateVendor(id: number, vendor: UpdateVendorDto): Observable<Vendor> {
        return this.http.put<Vendor>(`${this.apiUrl}/${id}`, vendor);
    }

    deleteVendor(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}
