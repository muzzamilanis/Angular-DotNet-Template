import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SaleOrder, CreateSaleOrderDto, UpdateSaleOrderDto } from '../models/sale-order.model';

@Injectable({
  providedIn: 'root'
})
export class SaleOrderService {
  private apiUrl = '/api/saleorders';

  constructor(private http: HttpClient) { }

  getSaleOrders(): Observable<SaleOrder[]> {
    return this.http.get<SaleOrder[]>(this.apiUrl);
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
}