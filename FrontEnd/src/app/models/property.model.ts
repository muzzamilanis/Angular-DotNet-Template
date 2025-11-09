export interface Property {
    id: number;
    propertyName: string;
    propertyType: string;
    address?: string;
    city?: string;
    state?: string;
    zipCode?: string;
    totalArea: number;
    areaUnit?: string;
    numberOfRooms: number;
    numberOfFloors: number;
    hasLawn: boolean;
    lawnArea?: number;
    hasGallery: boolean;
    numberOfGalleries: number;
    customStructureDetails?: string;
    propertyStatus?: string;
    currentValue: number;
    acquisitionDate: Date;
    description?: string;
    imageUrls?: string;
    isActive: boolean;
    companyId: number;
}

export interface CreatePropertyDto {
    propertyName: string;
    propertyType: string;
    address?: string;
    city?: string;
    state?: string;
    zipCode?: string;
    totalArea: number;
    areaUnit?: string;
    numberOfRooms: number;
    numberOfFloors: number;
    hasLawn: boolean;
    lawnArea?: number;
    hasGallery: boolean;
    numberOfGalleries: number;
    customStructureDetails?: string;
    propertyStatus?: string;
    currentValue: number;
    acquisitionDate: Date;
    description?: string;
    imageUrls?: string;
}

export interface PropertyPurchase {
    id: number;
    propertyId: number;
    purchaseNumber: string;
    sellerName: string;
    sellerAddress?: string;
    sellerPhone?: string;
    sellerEmail?: string;
    purchaseDate: Date;
    purchasePrice: number;
    advancePayment?: number;
    remainingPayment?: number;
    paymentStatus?: string;
    paymentMethod?: string;
    chequeNumber?: string;
    bankName?: string;
    notes?: string;
    companyId: number;
    voucherId?: number;
    property?: Property;
}

export interface CreatePropertyPurchaseDto {
    propertyId: number;
    purchaseNumber: string;
    sellerName: string;
    sellerAddress?: string;
    sellerPhone?: string;
    sellerEmail?: string;
    purchaseDate: Date;
    purchasePrice: number;
    advancePayment?: number;
    remainingPayment?: number;
    paymentStatus?: string;
    paymentMethod?: string;
    chequeNumber?: string;
    bankName?: string;
    notes?: string;
}

export interface PropertySale {
    id: number;
    propertyId: number;
    saleNumber: string;
    buyerName: string;
    buyerAddress?: string;
    buyerPhone?: string;
    buyerEmail?: string;
    saleDate: Date;
    salePrice: number;
    costPrice: number;
    profitOrLoss: number;
    advanceReceived?: number;
    remainingReceivable?: number;
    paymentStatus?: string;
    paymentMethod?: string;
    chequeNumber?: string;
    bankName?: string;
    notes?: string;
    companyId: number;
    voucherId?: number;
    property?: Property;
}

export interface CreatePropertySaleDto {
    propertyId: number;
    saleNumber: string;
    buyerName: string;
    buyerAddress?: string;
    buyerPhone?: string;
    buyerEmail?: string;
    saleDate: Date;
    salePrice: number;
    costPrice: number;
    advanceReceived?: number;
    remainingReceivable?: number;
    paymentStatus?: string;
    paymentMethod?: string;
    chequeNumber?: string;
    bankName?: string;
    notes?: string;
}
