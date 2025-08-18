import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WriterDto, WriterAssignmentDto, WriterSubmissionDto } from '../models/sale-order.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class WriterService {
  private apiUrl = `${environment.apiUrl}/Writers`;

  constructor(private http: HttpClient) { }

  getWriters(): Observable<WriterDto[]> {
    return this.http.get<WriterDto[]>(`${this.apiUrl}/list`);
  }

  getAssignments(): Observable<WriterAssignmentDto[]> {
    return this.http.get<WriterAssignmentDto[]>(`${this.apiUrl}/assignments`);
  }

  submitWork(submission: WriterSubmissionDto, file?: File): Observable<any> {
    const formData = new FormData();
    formData.append('saleOrderId', submission.saleOrderId.toString());
    formData.append('submissionStatus', submission.submissionStatus);
    
    if (submission.submittedFilename) {
      formData.append('submittedFilename', submission.submittedFilename);
    }
    
    if (submission.notes) {
      formData.append('notes', submission.notes);
    }
    
    if (file) {
      formData.append('submittedFile', file);
    }

    return this.http.post(`${this.apiUrl}/submit`, formData);
  }
}