export interface User {
    id?: number;
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
    phone?: string;
    isActive: boolean;
    role: UserRole;
    companyId?: number;
    companyName?: string;
    createdDate?: Date;
    modifiedDate?: Date;
    lastLoginDate?: Date;
    isSuperAdmin?: boolean; // Add this property
}

export interface CreateUserDto {
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
    phone?: string;
    password: string;
    role: UserRole;
    companyId?: number;
}

export interface UpdateUserDto {
    id: number;
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
    phone?: string;
    isActive: boolean;
    role: UserRole;
    companyId?: number;
}

export enum UserRole {
    SuperAdmin = 'SuperAdmin',
    CompanyAdmin = 'CompanyAdmin',
    User = 'User',
    Manager = 'Manager',
    Writer = 'Writer',
}

export interface ChangePasswordDto {
    userId: number;
    currentPassword: string;
    newPassword: string;
}

// Backend-aligned User interfaces
export interface BackendUser {
    id?: number;
    name: string;
    email: string;
    isActive: boolean;
    isSuperAdmin: boolean;
    role: UserRole;
    createdAt?: Date;
    companyId: number;
    companyName?: string;
}

export interface CreateBackendUserDto {
    name: string;
    email: string;
    password: string;
    isSuperAdmin: boolean;
    companyId: number;
}

export interface UpdateBackendUserDto {
    id: number;
    name: string;
    email: string;
    isActive: boolean;
    isSuperAdmin: boolean;
    companyId: number;
}

export interface BackendChangePasswordDto {
    userId: number;
    newPassword: string;
}
