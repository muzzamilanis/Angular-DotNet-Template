import { SaleOrderItem, CreateSaleOrderItemDto } from './sale-order-item.model';

export interface SaleOrder {
  id: number;
  orderNumber: string;
  orderDate: Date;
  customerName: string;
  customerAddress: string;
  totalAmount: number;
  items: SaleOrderItem[];
  companyId: number;
  companyName?: string;

  // Project relationship
  projectId?: number;
  projectName?: string;

  // Digital Marketing specific fields
  code?: string;
  clientName?: string;
  deadline?: Date;
  status?: string;
  amountReceived?: number;
  paymentStatus?: string;
  contactEmail?: string;
  contactPhone?: string;
  assignedTo?: string;
  academicLevel?: string;
  country?: string;
}

export interface CreateSaleOrderDto {
  orderNumber: string;
  orderDate: Date;
  customerName: string;
  customerAddress: string;
  items: CreateSaleOrderItemDto[];

  // Project relationship
  projectId?: number;

  // Digital Marketing specific fields
  code?: string;
  clientName?: string;
  deadline?: Date;
  status?: string;
  amountReceived?: number;
  paymentStatus?: string;
  contactEmail?: string;
  contactPhone?: string;
  assignedTo?: string;
  academicLevel?: string;
  country?: string;
}

export interface UpdateSaleOrderDto {
  id: number;
  orderNumber: string;
  orderDate: Date;
  customerName: string;
  customerAddress: string;
  items: CreateSaleOrderItemDto[];

  // Project relationship
  projectId?: number;

  // Digital Marketing specific fields
  code?: string;
  clientName?: string;
  deadline?: Date;
  status?: string;
  amountReceived?: number;
  paymentStatus?: string;
  contactEmail?: string;
  contactPhone?: string;
  assignedTo?: string;
  academicLevel?: string;
  country?: string;
}