export interface Company {
    id?: number;
    name: string;
    code: string;
    email: string;
    phone: string;
    address: string;
    city: string;
    state: string;
    country: string;
    zipCode: string;
    website?: string;
    taxNumber?: string;
    registrationNumber?: string;
    logo?: string;
    logoUrl?: string;
    primaryColor?: string;
    isActive: boolean;
    BusinessNatureId?: number; // "Digital Marketing" or "General"
    createdDate?: Date;
    modifiedDate?: Date;
}

export interface CreateCompanyDto {
    name: string;
    code: string;
    email: string;
    phone: string;
    address: string;
    city: string;
    state: string;
    country: string;
    zipCode: string;
    website?: string;
    taxNumber?: string;
    registrationNumber?: string;
    logo?: string;
    logoUrl?: string;
    primaryColor?: string;
    BusinessNatureId?: number;
}

export interface UpdateCompanyDto extends CreateCompanyDto {
    id: number;
    logoUrl?: string;
    primaryColor?: string;
    isActive: boolean;
}
