import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
    ProjectClient,
    CreateProjectClient,
    UpdateProjectClient,
    ProjectInvestor,
    CreateProjectInvestor,
    UpdateProjectInvestor,
    ProjectPayment,
    CreateProjectPayment,
    ProjectExpense,
    CreateProjectExpense,
    ClientLedgerEntry,
    InvestorLedgerEntry,
    ProjectLedgerEntry,
} from '../models/construction.model';

@Injectable({
    providedIn: 'root',
})
export class ConstructionClientService {
    private readonly apiUrl = `${environment.apiUrl}/construction/clients`;

    constructor(private http: HttpClient) {}

    getByProject(projectId: number): Observable<ProjectClient[]> {
        return this.http.get<ProjectClient[]>(`${this.apiUrl}/by-project/${projectId}`);
    }

    getById(id: number): Observable<ProjectClient> {
        return this.http.get<ProjectClient>(`${this.apiUrl}/${id}`);
    }

    create(client: CreateProjectClient): Observable<ProjectClient> {
        return this.http.post<ProjectClient>(this.apiUrl, client);
    }

    update(id: number, client: UpdateProjectClient): Observable<ProjectClient> {
        return this.http.put<ProjectClient>(`${this.apiUrl}/${id}`, client);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}

@Injectable({
    providedIn: 'root',
})
export class ConstructionInvestorService {
    private readonly apiUrl = `${environment.apiUrl}/construction/investors`;

    constructor(private http: HttpClient) {}

    getByProject(projectId: number): Observable<ProjectInvestor[]> {
        return this.http.get<ProjectInvestor[]>(`${this.apiUrl}/by-project/${projectId}`);
    }

    getById(id: number): Observable<ProjectInvestor> {
        return this.http.get<ProjectInvestor>(`${this.apiUrl}/${id}`);
    }

    create(investor: CreateProjectInvestor): Observable<ProjectInvestor> {
        return this.http.post<ProjectInvestor>(this.apiUrl, investor);
    }

    update(id: number, investor: UpdateProjectInvestor): Observable<ProjectInvestor> {
        return this.http.put<ProjectInvestor>(`${this.apiUrl}/${id}`, investor);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}

@Injectable({
    providedIn: 'root',
})
export class ConstructionPaymentService {
    private readonly apiUrl = `${environment.apiUrl}/construction/payments`;

    constructor(private http: HttpClient) {}

    getByProject(projectId: number): Observable<ProjectPayment[]> {
        return this.http.get<ProjectPayment[]>(`${this.apiUrl}/by-project/${projectId}`);
    }

    getById(id: number): Observable<ProjectPayment> {
        return this.http.get<ProjectPayment>(`${this.apiUrl}/${id}`);
    }

    create(payment: CreateProjectPayment): Observable<ProjectPayment> {
        return this.http.post<ProjectPayment>(this.apiUrl, payment);
    }
}

@Injectable({
    providedIn: 'root',
})
export class ConstructionExpenseService {
    private readonly apiUrl = `${environment.apiUrl}/construction/expenses`;

    constructor(private http: HttpClient) {}

    getByProject(projectId: number): Observable<ProjectExpense[]> {
        return this.http.get<ProjectExpense[]>(`${this.apiUrl}/by-project/${projectId}`);
    }

    getById(id: number): Observable<ProjectExpense> {
        return this.http.get<ProjectExpense>(`${this.apiUrl}/${id}`);
    }

    create(expense: CreateProjectExpense): Observable<ProjectExpense> {
        return this.http.post<ProjectExpense>(this.apiUrl, expense);
    }
}

@Injectable({
    providedIn: 'root',
})
export class ConstructionLedgerService {
    private readonly apiUrl = `${environment.apiUrl}/construction/ledgers`;

    constructor(private http: HttpClient) {}

    getClientLedger(clientId: number): Observable<ClientLedgerEntry[]> {
        return this.http.get<ClientLedgerEntry[]>(`${this.apiUrl}/client/${clientId}`);
    }

    getInvestorLedger(investorId: number): Observable<InvestorLedgerEntry[]> {
        return this.http.get<InvestorLedgerEntry[]>(`${this.apiUrl}/investor/${investorId}`);
    }

    getProjectLedger(projectId: number): Observable<ProjectLedgerEntry[]> {
        return this.http.get<ProjectLedgerEntry[]>(`${this.apiUrl}/project/${projectId}`);
    }
}
