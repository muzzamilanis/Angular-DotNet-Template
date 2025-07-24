import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  Project, 
  ProjectListItem, 
  CreateProjectDto, 
  UpdateProjectDto,
  CompanyNatureResponse 
} from '../models/project.model';
import { PaginatedResponse } from '../models/common.model';

export interface ProjectQueryParams {
  skipCount?: number;
  maxResultCount?: number;
  filter?: string;
  sorting?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private readonly apiUrl = `${environment.apiUrl}/api/projects`;

  constructor(private http: HttpClient) { }

  getAll(params?: ProjectQueryParams): Observable<PaginatedResponse<ProjectListItem>> {
    // Mock data for Digital Marketing projects
    const mockProjects: ProjectListItem[] = [
      {
        id: 1,
        projectName: 'Website Content Creation',
        projectDescription: 'Content writing for corporate website',
        projectStatus: 1, // Active
        createdDate: new Date('2024-01-15'),
        updatedDate: new Date('2024-01-20'),
        isActive: true,
        relatedSaleOrdersCount: 3
      },
      {
        id: 2,
        projectName: 'SEO Blog Articles',
        projectDescription: 'Monthly blog articles for SEO optimization',
        projectStatus: 1, // Active
        createdDate: new Date('2024-02-01'),
        updatedDate: new Date('2024-02-10'),
        isActive: true,
        relatedSaleOrdersCount: 5
      },
      {
        id: 3,
        projectName: 'Social Media Campaign',
        projectDescription: 'Social media content for product launch',
        projectStatus: 3, // Completed
        createdDate: new Date('2024-01-01'),
        updatedDate: new Date('2024-01-31'),
        isActive: false,
        relatedSaleOrdersCount: 2
      },
      {
        id: 4,
        projectName: 'Academic Research Papers',
        projectDescription: 'Research papers for university clients',
        projectStatus: 1, // Active
        createdDate: new Date('2024-02-15'),
        updatedDate: new Date('2024-02-20'),
        isActive: true,
        relatedSaleOrdersCount: 8
      }
    ];

    const response: PaginatedResponse<ProjectListItem> = {
      totalCount: mockProjects.length,
      items: mockProjects
    };

    return of(response);
    
    // Original implementation:
    // let httpParams = new HttpParams();
    // 
    // if (params?.skipCount !== undefined) {
    //   httpParams = httpParams.set('skipCount', params.skipCount.toString());
    // }
    // if (params?.maxResultCount !== undefined) {
    //   httpParams = httpParams.set('maxResultCount', params.maxResultCount.toString());
    // }
    // if (params?.filter) {
    //   httpParams = httpParams.set('filter', params.filter);
    // }
    // if (params?.sorting) {
    //   httpParams = httpParams.set('sorting', params.sorting);
    // }
    //
    // return this.http.get<PaginatedResponse<ProjectListItem>>(this.apiUrl, { params: httpParams });
  }

  getById(id: number): Observable<Project> {
    return this.http.get<Project>(`${this.apiUrl}/${id}`);
  }

  create(project: CreateProjectDto): Observable<Project> {
    return this.http.post<Project>(this.apiUrl, project);
  }

  update(id: number, project: UpdateProjectDto): Observable<Project> {
    return this.http.put<Project>(`${this.apiUrl}/${id}`, project);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getForDropdown(): Observable<Project[]> {
    return this.http.get<Project[]>(`${this.apiUrl}/dropdown`);
  }

  getCompanyNature(): Observable<CompanyNatureResponse> {
    return this.http.get<CompanyNatureResponse>(`${this.apiUrl}/company-nature`);
  }
}