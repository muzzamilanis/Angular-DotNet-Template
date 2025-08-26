// User and Authentication Types
export interface User {
  id: number;
  userName: string;
  email: string;
  fullName: string;
  isSuperAdmin: boolean;
  isActive: boolean;
  companyId?: number;
  company?: Company;
  role?: string;
  lastLoginTime?: Date;
  creationTime: Date;
}

export interface LoginRequest {
  usernameOrEmailAddress: string;
  password: string;
  rememberClient?: boolean;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  user: User;
  success: boolean;
  error?: string;
}

// Company Types
export interface Company {
  id: number;
  name: string;
  address?: string;
  phone?: string;
  email?: string;
  businessNature?: string;
  description?: string;
  isActive: boolean;
  creationTime: Date;
  primaryColor?: string;
}

// Project Types
export interface Project {
  id: number;
  name: string;
  description?: string;
  startDate: Date;
  endDate?: Date;
  status: string;
  companyId: number;
  company?: Company;
  serviceTypeId?: number;
  serviceType?: ServiceType;
}

// Service Type
export interface ServiceType {
  id: number;
  name: string;
  description?: string;
  isActive: boolean;
}

// Sale Order Types
export interface SaleOrder {
  id: number;
  orderDate: Date;
  customerName: string;
  companyName?: string;
  totalAmount: number;
  status: string;
  deadline?: Date;
  projectId?: number;
  project?: Project;
}

// API Response Types
export interface ApiResponse<T> {
  success: boolean;
  result?: T;
  error?: string;
  targetUrl?: string;
  unAuthorizedRequest?: boolean;
}

// Navigation Types
export type RootStackParamList = {
  Login: undefined;
  Register: undefined;
  Main: undefined;
  Dashboard: undefined;
  Profile: undefined;
  Companies: undefined;
  CompanyDetail: { companyId: number };
  Users: undefined;
  UserDetail: { userId: number };
  Projects: undefined;
  ProjectDetail: { projectId: number };
  Orders: undefined;
  OrderDetail: { orderId: number };
};

export type TabParamList = {
  Dashboard: undefined;
  Companies: undefined;
  Users: undefined;
  Projects: undefined;
  Orders: undefined;
  Profile: undefined;
};