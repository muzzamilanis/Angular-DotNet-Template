import { SaleOrderItem, CreateSaleOrderItemDto } from './sale-order-item.model';

export interface SaleOrder {
  id: number;
  orderNumber: string;
  orderDate: Date;
  customerName: string;
  customerAddress: string;
  totalAmount: number;
  items: SaleOrderItem[];
}

export interface CreateSaleOrderDto {
  orderNumber: string;
  orderDate: Date;
  customerName: string;
  customerAddress: string;
  items: CreateSaleOrderItemDto[];
}

export interface UpdateSaleOrderDto {
  id: number;
  orderNumber: string;
  orderDate: Date;
  customerName: string;
  customerAddress: string;
  items: CreateSaleOrderItemDto[];
}