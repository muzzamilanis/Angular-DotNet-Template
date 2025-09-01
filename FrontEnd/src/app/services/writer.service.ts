import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WriterAssignmentDto, WriterSubmissionDto, OrderInstructionDto, CreateOrderInstructionDto } from '../models/sale-order.model';
import { User } from '../models/user.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class WriterService {
  private apiUrl = `${environment.apiUrl}/Writers`;

  constructor(private http: HttpClient) { }

  getWriters(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/list`);
  }

  getAssignments(): Observable<WriterAssignmentDto[]> {
    return this.http.get<WriterAssignmentDto[]>(`${this.apiUrl}/assignments`);
  }

  submitWork(submission: WriterSubmissionDto, file?: File): Observable<any> {
    const formData = new FormData();
    
    // Match your DTO property names exactly
    formData.append('saleOrderId', String(submission.saleOrderId));
    formData.append('submissionStatus', String(submission.submissionStatus));
    
    if (submission.submittedFilename) {
      formData.append('submittedFilename', submission.submittedFilename);
    }
    if (submission.notes) {
      formData.append('notes', submission.notes);
    }
    if (file) {
      formData.append('submittedFile', file, file.name); // Must match IFormFile parameter name
    }
    
    // Do NOT pass any headers - let browser set multipart/form-data
    return this.http.post(`${this.apiUrl}/submit`, formData);
  }

  // Order Instructions methods for writers
  getOrderInstructions(saleOrderId: number): Observable<OrderInstructionDto[]> {
    return this.http.get<OrderInstructionDto[]>(`${this.apiUrl}/assignments/${saleOrderId}/instructions`);
  }

  addWriterResponse(saleOrderId: number, instruction: CreateOrderInstructionDto): Observable<OrderInstructionDto> {
    return this.http.post<OrderInstructionDto>(`${this.apiUrl}/assignments/${saleOrderId}/instructions`, instruction);
  }
}