export interface ServiceType {
    id: number;
    name: string;
    description?: string;
    isActive: boolean;
    companyId: number;
    companyName?: string;
}

export interface CreateServiceTypeDto {
    name: string;
    description?: string;
    isActive: boolean;
}
