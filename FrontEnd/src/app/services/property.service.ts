import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Property, CreatePropertyDto, PropertyPurchase, CreatePropertyPurchaseDto, PropertySale, CreatePropertySaleDto } from '../models/property.model';
import { PaginatedResponse } from '../models/common.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class PropertyService {
    private apiUrl = environment.apiUrl + '/properties';

    constructor(private http: HttpClient) {}

    getProperties(): Observable<PaginatedResponse<Property>> {
        return this.http.get<PaginatedResponse<Property>>(this.apiUrl);
    }

    getActiveProperties(): Observable<Property[]> {
        return this.http.get<Property[]>(`${this.apiUrl}/active`);
    }

    getProperty(id: number): Observable<Property> {
        return this.http.get<Property>(`${this.apiUrl}/${id}`);
    }

    createProperty(property: CreatePropertyDto): Observable<Property> {
        return this.http.post<Property>(this.apiUrl, property);
    }

    updateProperty(id: number, property: CreatePropertyDto): Observable<Property> {
        return this.http.put<Property>(`${this.apiUrl}/${id}`, property);
    }

    deleteProperty(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}

@Injectable({
    providedIn: 'root',
})
export class PropertyPurchaseService {
    private apiUrl = environment.apiUrl + '/propertypurchases';

    constructor(private http: HttpClient) {}

    getPurchases(): Observable<PaginatedResponse<PropertyPurchase>> {
        return this.http.get<PaginatedResponse<PropertyPurchase>>(this.apiUrl);
    }

    getPurchase(id: number): Observable<PropertyPurchase> {
        return this.http.get<PropertyPurchase>(`${this.apiUrl}/${id}`);
    }

    createPurchase(purchase: CreatePropertyPurchaseDto): Observable<PropertyPurchase> {
        return this.http.post<PropertyPurchase>(this.apiUrl, purchase);
    }

    updatePurchase(id: number, purchase: CreatePropertyPurchaseDto): Observable<PropertyPurchase> {
        return this.http.put<PropertyPurchase>(`${this.apiUrl}/${id}`, purchase);
    }

    deletePurchase(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}

@Injectable({
    providedIn: 'root',
})
export class PropertySaleService {
    private apiUrl = environment.apiUrl + '/propertysales';

    constructor(private http: HttpClient) {}

    getSales(): Observable<PaginatedResponse<PropertySale>> {
        return this.http.get<PaginatedResponse<PropertySale>>(this.apiUrl);
    }

    getSale(id: number): Observable<PropertySale> {
        return this.http.get<PropertySale>(`${this.apiUrl}/${id}`);
    }

    createSale(sale: CreatePropertySaleDto): Observable<PropertySale> {
        return this.http.post<PropertySale>(this.apiUrl, sale);
    }

    updateSale(id: number, sale: CreatePropertySaleDto): Observable<PropertySale> {
        return this.http.put<PropertySale>(`${this.apiUrl}/${id}`, sale);
    }

    deleteSale(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}
