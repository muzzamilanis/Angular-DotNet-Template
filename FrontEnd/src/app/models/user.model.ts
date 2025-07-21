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
  Manager = 'Manager'
}

export interface ChangePasswordDto {
  userId: number;
  currentPassword: string;
  newPassword: string;
}