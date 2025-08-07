export interface Currency {
    id?: number;
    name: string;
    symbol: string;
    isActive: boolean;
    createdAt?: Date;
    companyId?: number;
}

export interface CreateCurrencyDto {
    name: string;
    symbol: string;
}

export interface UpdateCurrencyDto extends CreateCurrencyDto {
    id: number;
    isActive: boolean;
}

export interface ExchangeRateResponse {
    currencyCode: string;
    exchangeRate: number;
    baseCurrency: string;
    lastUpdated: Date;
}