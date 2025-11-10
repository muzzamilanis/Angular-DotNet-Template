import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
    ConstructionProject,
    CreateConstructionProject,
    UpdateConstructionProject,
    ConstructionDashboard,
    ProjectProfitLoss,
} from '../models/construction.model';
import { PaginatedResponse } from '../models/common.model';

export interface ConstructionProjectQueryParams {
    skipCount?: number;
    maxResultCount?: number;
    filter?: string;
    sorting?: string;
}

@Injectable({
    providedIn: 'root',
})
export class ConstructionProjectService {
    private readonly apiUrl = `${environment.apiUrl}/construction/projects`;

    constructor(private http: HttpClient) {}

    getAll(params?: ConstructionProjectQueryParams): Observable<PaginatedResponse<ConstructionProject>> {
        let httpParams = new HttpParams();

        if (params?.skipCount !== undefined) {
            httpParams = httpParams.set('skipCount', params.skipCount.toString());
        }
        if (params?.maxResultCount !== undefined) {
            httpParams = httpParams.set('maxResultCount', params.maxResultCount.toString());
        }
        if (params?.filter) {
            httpParams = httpParams.set('filter', params.filter);
        }
        if (params?.sorting) {
            httpParams = httpParams.set('sorting', params.sorting);
        }

        return this.http.get<PaginatedResponse<ConstructionProject>>(this.apiUrl, { params: httpParams });
    }

    getById(id: number): Observable<ConstructionProject> {
        return this.http.get<ConstructionProject>(`${this.apiUrl}/${id}`);
    }

    create(project: CreateConstructionProject): Observable<ConstructionProject> {
        return this.http.post<ConstructionProject>(this.apiUrl, project);
    }

    update(id: number, project: UpdateConstructionProject): Observable<ConstructionProject> {
        return this.http.put<ConstructionProject>(`${this.apiUrl}/${id}`, project);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

    getDashboard(): Observable<ConstructionDashboard> {
        return this.http.get<ConstructionDashboard>(`${this.apiUrl}/dashboard`);
    }

    getProfitLoss(projectId: number): Observable<ProjectProfitLoss> {
        return this.http.get<ProjectProfitLoss>(`${this.apiUrl}/${projectId}/profit-loss`);
    }
}
