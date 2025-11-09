export interface BusinessNature {
    id: number;
    name: string;
    displayName: string;
    description?: string;
    configSchema?: string;
    isActive: boolean;
    createdDate: Date;
}

export interface CompanyProfile {
    id: number;
    tenantId: number;
    businessNatureId: number;
    companyName: string;
    address?: string;
    phone?: string;
    email?: string;
    customFields?: string;
    isActive: boolean;
    createdDate: Date;
    businessNature?: BusinessNature;
}

export interface CreateCompanyProfileRequest {
    businessNatureId: number;
    companyName: string;
    address?: string;
    phone?: string;
    email?: string;
    contactPerson?: string;
    customFields?: string;
}

export interface IndustryModule {
    id: number;
    businessNatureId: number;
    moduleName: string;
    displayName?: string;
    moduleConfig?: string;
    isEnabled: boolean;
    createdDate: Date;
}

export interface IndustryFormConfig {
    id: number;
    businessNatureId: number;
    formType: string;
    fieldSchema: string;
    validationRules?: string;
    isActive: boolean;
    createdDate: Date;
}

export interface IndustryConfig {
    businessNatureId: number;
    businessNature?: BusinessNature;
    modules: IndustryModule[];
    formConfigs: IndustryFormConfig[];
}

// Form field configuration interfaces
export interface FormField {
    name: string;
    type: 'text' | 'number' | 'select' | 'textarea' | 'date' | 'email' | 'tel';
    label: string;
    required: boolean;
    options?: string[];
    min?: number;
    max?: number;
    pattern?: string;
    placeholder?: string;
}

export interface FormSchema {
    fields: FormField[];
}

export interface ValidationRule {
    minLength?: number;
    maxLength?: number;
    min?: number;
    max?: number;
    pattern?: string;
    message?: string;
}

export interface FormValidationRules {
    [fieldName: string]: ValidationRule;
}

export enum BusinessNatureType {
    DIGITALMARKETINGAGENCY = 1,
    AUTOMOBILE = 2,
    GENERAL = 3,
    RETAIL = 4,
    MANUFACTURING = 5,
    REALESTATE = 6,
}
