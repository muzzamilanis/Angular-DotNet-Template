export interface SaleOrderItem {
  id: number;
  saleOrderId: number;
  productId: number;
  productName: string;
  quantity: number;
  date: Date;
  price: number;
  lineTotal: number;

  // Digital Marketing specific fields
  workTitle?: string;
  paperType?: string;
  serviceType?: string;
  wordCount?: number;
  pageCount?: number;
  itemDeadline?: Date;
}

export interface CreateSaleOrderItemDto {
  productId: number;
  quantity: number;
  date: Date;
  price: number;

  // Digital Marketing specific fields
  workTitle?: string;
  paperType?: string;
  serviceType?: string;
  wordCount?: number;
  pageCount?: number;
  itemDeadline?: Date;
}