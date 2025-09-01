import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { OrderInstructionDto, CreateOrderInstructionDto, UpdateOrderDetailsDto } from '../models/sale-order.model';

export interface TeamLeadOrderDto {
    id: number;
    orderNumber: string;
    orderDate: Date;
    clientName?: string;
    customerName?: string;
    deadline?: Date;
    status?: string;
    assignedTo?: string;
    assignedToUserId?: number;
    orderDetails?: string;
    academicLevel?: string;
    country?: string;
    wordsOrPages?: string;
    submissionStatus?: string;
    submissionDate?: Date;
    attachmentFilenames?: string;
}

export interface AssignOrderDto {
    saleOrderId: number;
    writerId: number;
}

export interface WriterDto {
    id: number;
    name: string;
    email: string;
    isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class TeamLeadService {
  private apiUrl = `${environment.apiUrl}/TeamLead`;

  constructor(private http: HttpClient) { }

  getOrders(): Observable<TeamLeadOrderDto[]> {
    return this.http.get<TeamLeadOrderDto[]>(`${this.apiUrl}/orders`);
  }

  getWriters(): Observable<WriterDto[]> {
    return this.http.get<WriterDto[]>(`${this.apiUrl}/writers`);
  }

  assignOrder(assignment: AssignOrderDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/assign`, assignment);
  }

  updateAssignment(assignment: AssignOrderDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/assign`, assignment);
  }

  // Order Instructions methods
  getOrderInstructions(saleOrderId: number): Observable<OrderInstructionDto[]> {
    return this.http.get<OrderInstructionDto[]>(`${this.apiUrl}/orders/${saleOrderId}/instructions`);
  }

  addOrderInstruction(saleOrderId: number, instruction: CreateOrderInstructionDto): Observable<OrderInstructionDto> {
    return this.http.post<OrderInstructionDto>(`${this.apiUrl}/orders/${saleOrderId}/instructions`, instruction);
  }

  updateOrderDetails(saleOrderId: number, updateDto: UpdateOrderDetailsDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/orders/${saleOrderId}/details`, updateDto);
  }

  uploadInstructionFiles(saleOrderId: number, files: File[]): Observable<any> {
    const formData = new FormData();
    files.forEach(file => {
      formData.append('files', file, file.name);
    });
    return this.http.post(`${this.apiUrl}/orders/${saleOrderId}/upload-instructions`, formData);
  }
}