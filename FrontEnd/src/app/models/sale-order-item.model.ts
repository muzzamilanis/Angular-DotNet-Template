export interface SaleOrderItem {
  id: number;
  saleOrderId: number;
  productId: number;
  productName: string;
  quantity: number;
  date: Date;
  price: number;
  lineTotal: number;
}

export interface CreateSaleOrderItemDto {
  productId: number;
  quantity: number;
  date: Date;
  price: number;
}