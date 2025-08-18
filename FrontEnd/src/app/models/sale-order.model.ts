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
    
    academicLevel?: string;
    country?: string;
    orderDetails?: string;
    wordsOrPages?: string;
    totalPayment?: number;
    
    // Currency-related fields for Digital Marketing companies
    currencyCode?: string;
    foreignCurrencyAmount?: number;
    pkrAmount?: number;
    exchangeRateUsed?: number;
}

// Writer Portal specific interfaces
export interface WriterSubmissionDto {
    saleOrderId: number;
    submissionStatus: string; // "Completed" or "Need clarity"
    submittedFilename?: string;
    notes?: string;
}

export interface WriterAssignmentDto {
    id: number;
    orderNumber: string;
    orderDate: Date;
    clientName?: string;
    deadline?: Date;
    status?: string;
    orderDetails?: string;
    wordsOrPages?: string;
    academicLevel?: string;
    attachmentFilenames?: string;
    submissionStatus?: string;
    submissionDate?: Date;
    submittedFilename?: string;
}
