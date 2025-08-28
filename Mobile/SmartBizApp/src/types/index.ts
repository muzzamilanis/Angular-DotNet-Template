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
  UsernameOrEmail: string;
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
  projectName: string;
  description?: string;
  startDate: Date;
  endDate?: Date;
  deadline?: Date;
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
  orderNumber: string;
  orderDate: Date;
  customerName: string;
  customerAddress: string;
  totalAmount: number;
  items?: SaleOrderItem[];
  companyId: number;
  companyName?: string;

  // Project relationship
  projectId?: number;
  projectName?: string;

  // ServiceType relationship
  serviceTypeId?: number;
  serviceTypeName?: string;

  // Digital Marketing specific fields
  code?: string;
  clientName?: string;
  deadline?: Date;
  status?: string;
  amountReceived?: number;
  amountReceivedInPkr?: number;
  paymentStatus?: string;
  sourceOfPayment?: string;
  contactEmail?: string;
  contactPhone?: string;
  assignedTo?: string;
  
  attachmentFilenames?: string;
  submittedFilename?: string;
  submissionStatus?: string;
  submissionDate?: Date;
  
  orderDetails?: string;
  wordsOrPages?: string;
  totalPayment?: number;
  academicLevel?: string;
  country?: string;
  
  // Currency-related fields for Digital Marketing companies
  currencyCode?: string;
  foreignCurrencyAmount?: number;
  pkrAmount?: number;
  exchangeRateUsed?: number;
}

// Sale Order Item Types
export interface SaleOrderItem {
  id: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  saleOrderId: number;
}

// Create DTOs
export interface CreateSaleOrderDto {
  orderNumber: string;
  orderDate: Date;
  customerName: string;
  customerAddress: string;
  items?: CreateSaleOrderItemDto[];

  // Project relationship
  projectId?: number;
  // ServiceType relationship
  serviceTypeId?: number;

  // Digital Marketing specific fields
  code?: string;
  clientName?: string;
  deadline?: Date;
  status?: string;
  amountReceived?: number;
  amountReceivedInPkr?: number;
  paymentStatus?: string;
  sourceOfPayment?: string;
  contactEmail?: string;
  contactPhone?: string;
  assignedTo?: string;
  
  attachmentFilenames?: string;
  
  academicLevel?: string;
  orderDetails?: string;
  wordsOrPages?: string;
  totalPayment?: number;
  country?: string;
  
  // Currency-related fields for Digital Marketing companies
  currencyCode?: string;
  foreignCurrencyAmount?: number;
  pkrAmount?: number;
  exchangeRateUsed?: number;
}

export interface CreateSaleOrderItemDto {
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
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