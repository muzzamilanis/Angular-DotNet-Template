import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
    PerformanceCriteria,
    CreatePerformanceCriteriaDto,
    UpdatePerformanceCriteriaDto,
} from '../models/performance-criteria.model';
import { UserRole } from '../models/user.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class PerformanceCriteriaService {
    private apiUrl = `${environment.apiUrl}/PerformanceCriteria`;

    constructor(private http: HttpClient) { }

    getAll(): Observable<PerformanceCriteria[]> {
        return this.http.get<PerformanceCriteria[]>(this.apiUrl);
    }

    getByUserRole(role: UserRole): Observable<PerformanceCriteria[]> {
        return this.http.get<PerformanceCriteria[]>(`${this.apiUrl}/by-role/${role}`);
    }

    getById(id: number): Observable<PerformanceCriteria> {
        return this.http.get<PerformanceCriteria>(`${this.apiUrl}/${id}`);
    }

    create(dto: CreatePerformanceCriteriaDto): Observable<PerformanceCriteria> {
        return this.http.post<PerformanceCriteria>(this.apiUrl, dto);
    }

    update(id: number, dto: UpdatePerformanceCriteriaDto): Observable<PerformanceCriteria> {
        return this.http.put<PerformanceCriteria>(`${this.apiUrl}/${id}`, dto);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}
