export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  isActive: boolean;
}

export interface CreateProductDto {
  name: string;
  description: string;
  price: number;
  isActive: boolean;
}

export interface UpdateProductDto {
  id: number;
  name: string;
  description: string;
  price: number;
  isActive: boolean;
}