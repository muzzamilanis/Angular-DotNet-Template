export enum ConstructionProjectStatus {
    Planning = 'Planning',
    InProgress = 'InProgress',
    OnHold = 'OnHold',
    Completed = 'Completed',
    Cancelled = 'Cancelled',
}

export enum PaymentSource {
    Client = 'Client',
    Investor = 'Investor',
}

export interface ConstructionProject {
    id: number;
    projectName: string;
    projectDescription?: string;
    location?: string;
    status: ConstructionProjectStatus;
    estimatedBudget: number;
    actualCost: number;
    startDate?: Date;
    expectedEndDate?: Date;
    actualEndDate?: Date;
    projectManager?: string;
    contractor?: string;
    isActive: boolean;
    createdAt: Date;
    companyId: number;
    companyName?: string;
    clientsCount?: number;
    investorsCount?: number;
    totalPayments?: number;
    totalExpenses?: number;
    profitOrLoss?: number;
}

export interface CreateConstructionProject {
    projectName: string;
    projectDescription?: string;
    location?: string;
    status: ConstructionProjectStatus;
    estimatedBudget: number;
    startDate?: Date;
    expectedEndDate?: Date;
    projectManager?: string;
    contractor?: string;
}

export interface UpdateConstructionProject {
    id: number;
    projectName: string;
    projectDescription?: string;
    location?: string;
    status: ConstructionProjectStatus;
    estimatedBudget: number;
    startDate?: Date;
    expectedEndDate?: Date;
    actualEndDate?: Date;
    projectManager?: string;
    contractor?: string;
    isActive: boolean;
}

export interface ProjectClient {
    id: number;
    clientName: string;
    address?: string;
    phone?: string;
    email?: string;
    contactPerson?: string;
    cnicOrNtn?: string;
    sharePercentage: number;
    isActive: boolean;
    constructionProjectId: number;
    projectName?: string;
    ledgerBalance?: number;
}

export interface CreateProjectClient {
    clientName: string;
    address?: string;
    phone?: string;
    email?: string;
    contactPerson?: string;
    cnicOrNtn?: string;
    sharePercentage: number;
    constructionProjectId: number;
}

export interface UpdateProjectClient {
    id: number;
    clientName: string;
    address?: string;
    phone?: string;
    email?: string;
    contactPerson?: string;
    cnicOrNtn?: string;
    sharePercentage: number;
    isActive: boolean;
}

export interface ProjectInvestor {
    id: number;
    investorName: string;
    address?: string;
    phone?: string;
    email?: string;
    contactPerson?: string;
    cnicOrNtn?: string;
    investmentAmount: number;
    sharePercentage: number;
    isActive: boolean;
    constructionProjectId: number;
    projectName?: string;
    ledgerBalance?: number;
}

export interface CreateProjectInvestor {
    investorName: string;
    address?: string;
    phone?: string;
    email?: string;
    contactPerson?: string;
    cnicOrNtn?: string;
    investmentAmount: number;
    sharePercentage: number;
    constructionProjectId: number;
}

export interface UpdateProjectInvestor {
    id: number;
    investorName: string;
    address?: string;
    phone?: string;
    email?: string;
    contactPerson?: string;
    cnicOrNtn?: string;
    investmentAmount: number;
    sharePercentage: number;
    isActive: boolean;
}

export interface ProjectPayment {
    id: number;
    paymentNumber: string;
    paymentSource: PaymentSource;
    projectClientId?: number;
    clientName?: string;
    projectInvestorId?: number;
    investorName?: string;
    amount: number;
    paymentDate: Date;
    paymentMethod?: string;
    referenceNumber?: string;
    description?: string;
    isActive: boolean;
    constructionProjectId: number;
    projectName?: string;
    voucherId?: number;
}

export interface CreateProjectPayment {
    paymentSource: PaymentSource;
    projectClientId?: number;
    projectInvestorId?: number;
    amount: number;
    paymentDate: Date;
    paymentMethod?: string;
    referenceNumber?: string;
    description?: string;
    constructionProjectId: number;
}

export interface ProjectExpense {
    id: number;
    expenseNumber: string;
    expenseCategory: string;
    description: string;
    amount: number;
    expenseDate: Date;
    vendorName?: string;
    invoiceNumber?: string;
    paymentMethod?: string;
    referenceNumber?: string;
    isActive: boolean;
    constructionProjectId: number;
    projectName?: string;
    voucherId?: number;
}

export interface CreateProjectExpense {
    expenseCategory: string;
    description: string;
    amount: number;
    expenseDate: Date;
    vendorName?: string;
    invoiceNumber?: string;
    paymentMethod?: string;
    referenceNumber?: string;
    constructionProjectId: number;
}

export interface ClientLedgerEntry {
    id: number;
    entryNumber: string;
    entryDate: Date;
    entryType: string;
    amount: number;
    balance: number;
    description?: string;
    referenceNumber?: string;
    projectClientId: number;
    clientName?: string;
    constructionProjectId: number;
    projectName?: string;
}

export interface InvestorLedgerEntry {
    id: number;
    entryNumber: string;
    entryDate: Date;
    entryType: string;
    amount: number;
    balance: number;
    description?: string;
    referenceNumber?: string;
    projectInvestorId: number;
    investorName?: string;
    constructionProjectId: number;
    projectName?: string;
}

export interface ProjectLedgerEntry {
    id: number;
    entryNumber: string;
    entryDate: Date;
    transactionType: string;
    entryType: string;
    amount: number;
    balance: number;
    description?: string;
    referenceNumber?: string;
    constructionProjectId: number;
    projectName?: string;
}

export interface ConstructionDashboard {
    totalProjects: number;
    activeProjects: number;
    completedProjects: number;
    onHoldProjects: number;
    totalEstimatedBudget: number;
    totalActualCost: number;
    totalPaymentsReceived: number;
    totalExpenses: number;
    overallProfitOrLoss: number;
    totalClients: number;
    totalInvestors: number;
    recentPayments?: ProjectPayment[];
    recentExpenses?: ProjectExpense[];
    activeProjectsList?: ConstructionProject[];
}

export interface ProjectProfitLoss {
    projectId: number;
    projectName: string;
    totalRevenue: number;
    totalExpenses: number;
    profitOrLoss: number;
    profitMargin: number;
    expenseBreakdown?: ExpenseCategory[];
}

export interface ExpenseCategory {
    category: string;
    amount: number;
    percentage: number;
}
