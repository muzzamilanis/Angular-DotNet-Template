export interface Customer {
  id: number;
  name: string;
  address: string;
  phone: string;
  email: string;
  isActive: boolean;
  companyId: number;
  companyName?: string;
}

export interface CreateCustomerDto {
  name: string;
  address: string;
  phone: string;
  email: string;
  isActive: boolean;
}

export interface UpdateCustomerDto {
  id: number;
  name: string;
  address: string;
  phone: string;
  email: string;
  isActive: boolean;
}