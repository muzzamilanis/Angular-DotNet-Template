import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SaleOrder, CreateSaleOrderDto, UpdateSaleOrderDto } from '../models/sale-order.model';
import { PaginatedResponse } from '../models/common.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class SaleOrderService {
    private apiUrl = environment.apiUrl + '/saleorders';

    constructor(private http: HttpClient) {}

    getSaleOrders(): Observable<PaginatedResponse<SaleOrder>> {
        return this.http.get<PaginatedResponse<SaleOrder>>(this.apiUrl);
    }

    getSaleOrder(id: number): Observable<SaleOrder> {
        return this.http.get<SaleOrder>(`${this.apiUrl}/${id}`);
    }

    createSaleOrder(saleOrder: CreateSaleOrderDto): Observable<SaleOrder> {
        return this.http.post<SaleOrder>(this.apiUrl, saleOrder);
    }

    updateSaleOrder(id: number, saleOrder: UpdateSaleOrderDto): Observable<SaleOrder> {
        return this.http.put<SaleOrder>(`${this.apiUrl}/${id}`, saleOrder);
    }

    deleteSaleOrder(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

    uploadAttachments(saleOrderId: number, files: File[]): Observable<any> {
        const formData = new FormData();
        files.forEach((file) => {
            formData.append('files', file);
        });

        return this.http.post(`${this.apiUrl}/${saleOrderId}/upload-attachments`, formData);
    }

    downloadFile(filename: string): Observable<Blob> {
        return this.http.get(`${this.apiUrl}/download/${filename}`, { responseType: 'blob' });
    }
}
