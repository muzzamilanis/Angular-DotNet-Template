import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CrmFormTemplate, CreateCrmFormTemplateDto, UpdateCrmFormTemplateDto, CrmFormSubmission, CreateCrmFormSubmissionDto } from '../../models/crm/crm.model';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class CrmFormService {
    private templateApiUrl = environment.apiUrl + '/crm/formtemplates';
    private submissionApiUrl = environment.apiUrl + '/crm/formsubmissions';

    constructor(private http: HttpClient) {}

    // Form Template methods
    getFormTemplates(skipCount: number = 0, maxResultCount: number = 10, filter?: string): Observable<any> {
        let params = new HttpParams()
            .set('skipCount', skipCount.toString())
            .set('maxResultCount', maxResultCount.toString());
        
        if (filter) {
            params = params.set('filter', filter);
        }

        return this.http.get<any>(this.templateApiUrl, { params });
    }

    getActiveFormTemplates(): Observable<CrmFormTemplate[]> {
        return this.http.get<CrmFormTemplate[]>(`${this.templateApiUrl}/active`);
    }

    getFormTemplatesByBusinessNature(businessNatureId: number): Observable<CrmFormTemplate[]> {
        return this.http.get<CrmFormTemplate[]>(`${this.templateApiUrl}/by-business-nature/${businessNatureId}`);
    }

    getFormTemplate(id: number): Observable<CrmFormTemplate> {
        return this.http.get<CrmFormTemplate>(`${this.templateApiUrl}/${id}`);
    }

    createFormTemplate(template: CreateCrmFormTemplateDto): Observable<CrmFormTemplate> {
        return this.http.post<CrmFormTemplate>(this.templateApiUrl, template);
    }

    updateFormTemplate(id: number, template: UpdateCrmFormTemplateDto): Observable<CrmFormTemplate> {
        return this.http.put<CrmFormTemplate>(`${this.templateApiUrl}/${id}`, template);
    }

    deleteFormTemplate(id: number): Observable<void> {
        return this.http.delete<void>(`${this.templateApiUrl}/${id}`);
    }

    // Form Submission methods
    getFormSubmissions(skipCount: number = 0, maxResultCount: number = 10): Observable<any> {
        let params = new HttpParams()
            .set('skipCount', skipCount.toString())
            .set('maxResultCount', maxResultCount.toString());

        return this.http.get<any>(this.submissionApiUrl, { params });
    }

    getFormSubmissionsByTemplate(templateId: number): Observable<CrmFormSubmission[]> {
        return this.http.get<CrmFormSubmission[]>(`${this.submissionApiUrl}/by-template/${templateId}`);
    }

    getFormSubmissionsByLead(leadId: number): Observable<CrmFormSubmission[]> {
        return this.http.get<CrmFormSubmission[]>(`${this.submissionApiUrl}/by-lead/${leadId}`);
    }

    getFormSubmission(id: number): Observable<CrmFormSubmission> {
        return this.http.get<CrmFormSubmission>(`${this.submissionApiUrl}/${id}`);
    }

    createFormSubmission(submission: CreateCrmFormSubmissionDto): Observable<CrmFormSubmission> {
        return this.http.post<CrmFormSubmission>(this.submissionApiUrl, submission);
    }

    deleteFormSubmission(id: number): Observable<void> {
        return this.http.delete<void>(`${this.submissionApiUrl}/${id}`);
    }
}