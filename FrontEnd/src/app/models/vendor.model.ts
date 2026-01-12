export interface Vendor {
    id: number;
    name: string;
    address?: string;
    phone?: string;
    email?: string;
    contactPerson?: string;
    description?: string;
    isActive: boolean;
    createdAt: Date;
    updatedAt?: Date;
    companyId: number;
    companyName?: string;
}

export interface CreateVendorDto {
    name: string;
    address?: string;
    phone?: string;
    email?: string;
    contactPerson?: string;
    description?: string;
    isActive: boolean;
}

export interface UpdateVendorDto {
    id: number;
    name: string;
    address?: string;
    phone?: string;
    email?: string;
    contactPerson?: string;
    description?: string;
    isActive: boolean;
}
