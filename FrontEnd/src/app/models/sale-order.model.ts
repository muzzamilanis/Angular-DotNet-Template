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

  // ServiceType relationship
  serviceTypeId?: number;
  serviceTypeName?: string;

  // Digital Marketing specific fields
  code?: string;
  clientName?: string;
  deadline?: Date;
  status?: string;
  amountReceived?: number;
  paymentStatus?: string;
  sourceOfPayment?: string;
  contactEmail?: string;
  contactPhone?: string;
  assignedTo?: string;
  orderDetails?: string;
  wordsOrPages?: string;
  totalPayment?: number;
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
  // ServiceType relationship
  serviceTypeId?: number;

  // Digital Marketing specific fields
  code?: string;
  clientName?: string;
  deadline?: Date;
  status?: string;
  amountReceived?: number;
  paymentStatus?: string;
  sourceOfPayment?: string;
  contactEmail?: string;
  contactPhone?: string;
  assignedTo?: string;
  academicLevel?: string;
  orderDetails?: string;
  wordsOrPages?: string;
  totalPayment?: number;
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
  // ServiceType relationship
  serviceTypeId?: number;

  // Digital Marketing specific fields
  code?: string;
  clientName?: string;
  deadline?: Date;
  status?: string;
  amountReceived?: number;
  paymentStatus?: string;
  sourceOfPayment?: string;
  contactEmail?: string;
  contactPhone?: string;
  assignedTo?: string;
  academicLevel?: string;
  country?: string;
  orderDetails?: string;
  wordsOrPages?: string;
  totalPayment?: number;
}