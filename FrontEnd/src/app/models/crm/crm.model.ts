export enum LeadStatus {
    New = 1,
    Contacted = 2,
    Qualified = 3,
    Proposal = 4,
    Negotiation = 5,
    Converted = 6,
    Lost = 7,
    Unqualified = 8
}

export enum DealStatus {
    Proposal = 1,
    Negotiation = 2,
    Won = 3,
    Lost = 4,
    Cancelled = 5
}

export enum CrmTaskStatus {
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4,
    Overdue = 5
}

export enum CrmTaskPriority {
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

export interface Lead {
    id: number;
    fullName: string;
    email: string;
    phone?: string;
    source?: string;
    notes?: string;
    status: LeadStatus;
    businessNatureId: number;
    businessNatureName?: string;
    companyId: number;
    companyName?: string;
    isActive: boolean;
    createdAt: Date;
    updatedAt: Date;
}

export interface CreateLeadDto {
    fullName: string;
    email: string;
    phone?: string;
    source?: string;
    notes?: string;
    status: LeadStatus;
    businessNatureId: number;
    isActive: boolean;
}

export interface UpdateLeadDto {
    id: number;
    fullName: string;
    email: string;
    phone?: string;
    source?: string;
    notes?: string;
    status: LeadStatus;
    businessNatureId: number;
    isActive: boolean;
}

export interface Contact {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    phone?: string;
    jobTitle?: string;
    notes?: string;
    leadId?: number;
    leadName?: string;
    companyId: number;
    companyName?: string;
    isActive: boolean;
    createdAt: Date;
    updatedAt: Date;
}

export interface CreateContactDto {
    firstName: string;
    lastName: string;
    email: string;
    phone?: string;
    jobTitle?: string;
    notes?: string;
    leadId?: number;
    isActive: boolean;
}

export interface UpdateContactDto {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    phone?: string;
    jobTitle?: string;
    notes?: string;
    leadId?: number;
    isActive: boolean;
}

export interface Deal {
    id: number;
    title: string;
    description?: string;
    value: number;
    status: DealStatus;
    expectedCloseDate?: Date;
    actualCloseDate?: Date;
    leadId?: number;
    leadName?: string;
    contactId?: number;
    contactName?: string;
    companyId: number;
    companyName?: string;
    isActive: boolean;
    createdAt: Date;
    updatedAt: Date;
}

export interface CreateDealDto {
    title: string;
    description?: string;
    value: number;
    status: DealStatus;
    expectedCloseDate?: Date;
    leadId?: number;
    contactId?: number;
    isActive: boolean;
}

export interface UpdateDealDto {
    id: number;
    title: string;
    description?: string;
    value: number;
    status: DealStatus;
    expectedCloseDate?: Date;
    actualCloseDate?: Date;
    leadId?: number;
    contactId?: number;
    isActive: boolean;
}

export interface CrmFormTemplate {
    id: number;
    name: string;
    description?: string;
    jsonSchema: string;
    businessNatureId: number;
    businessNatureName?: string;
    companyId: number;
    companyName?: string;
    isActive: boolean;
    createdAt: Date;
    updatedAt: Date;
}

export interface CreateCrmFormTemplateDto {
    name: string;
    description?: string;
    jsonSchema: string;
    businessNatureId: number;
    isActive: boolean;
}

export interface UpdateCrmFormTemplateDto {
    id: number;
    name: string;
    description?: string;
    jsonSchema: string;
    businessNatureId: number;
    isActive: boolean;
}

export interface CrmFormSubmission {
    id: number;
    formTemplateId: number;
    formTemplateName?: string;
    submittedData: string;
    leadId?: number;
    leadName?: string;
    contactId?: number;
    contactName?: string;
    companyId: number;
    companyName?: string;
    isActive: boolean;
    submittedAt: Date;
}

export interface CreateCrmFormSubmissionDto {
    formTemplateId: number;
    submittedData: string;
    leadId?: number;
    contactId?: number;
    isActive: boolean;
}