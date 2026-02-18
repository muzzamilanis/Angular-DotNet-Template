import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
    UserPerformanceScore,
    CreateUserPerformanceScoreDto,
    UpdateUserPerformanceScoreDto,
} from '../models/user-performance-score.model';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class UserPerformanceScoreService {
    private apiUrl = `${environment.apiUrl}/UserPerformanceScores`;

    constructor(private http: HttpClient) { }

    getAll(): Observable<UserPerformanceScore[]> {
        return this.http.get<UserPerformanceScore[]>(this.apiUrl);
    }

    getByUserId(userId: number): Observable<UserPerformanceScore[]> {
        return this.http.get<UserPerformanceScore[]>(`${this.apiUrl}/user/${userId}`);
    }

    getById(id: number): Observable<UserPerformanceScore> {
        return this.http.get<UserPerformanceScore>(`${this.apiUrl}/${id}`);
    }

    create(dto: CreateUserPerformanceScoreDto): Observable<UserPerformanceScore> {
        return this.http.post<UserPerformanceScore>(this.apiUrl, dto);
    }

    update(id: number, dto: UpdateUserPerformanceScoreDto): Observable<UserPerformanceScore> {
        return this.http.put<UserPerformanceScore>(`${this.apiUrl}/${id}`, dto);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}
