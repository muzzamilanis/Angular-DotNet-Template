# Real Estate Business Workflow - Frontend Implementation Guide

## Overview
This document provides guidance for implementing the frontend components for the Real Estate business workflow in SmartBizApp.

## Completed Backend Implementation

### Database Models
- ✅ Property (with rooms, floors, lawns, galleries, custom structure)
- ✅ PropertyPurchase (with automated accounting)
- ✅ PropertySale (with automated accounting and profit/loss calculation)
- ✅ OwnershipTransfer
- ✅ Account (Chart of Accounts)
- ✅ Voucher (Journal, Cash, Bank)
- ✅ JournalEntry (Debit/Credit entries)

### Backend Services & Controllers
- ✅ PropertyService & PropertiesController
- ✅ PropertyPurchaseService & PropertyPurchasesController (auto-creates vouchers)
- ✅ PropertySaleService & PropertySalesController (auto-creates vouchers)
- ✅ AccountService & AccountsController
- ✅ VoucherService & VouchersController
- ✅ AccountingService (handles automated accounting entries)

### API Endpoints Available

#### Properties
- GET /api/properties - Get all properties (paginated)
- GET /api/properties/active - Get active properties
- GET /api/properties/{id} - Get property by ID
- POST /api/properties - Create property
- PUT /api/properties/{id} - Update property
- DELETE /api/properties/{id} - Delete property

#### Property Purchases
- GET /api/propertypurchases - Get all purchases (paginated)
- GET /api/propertypurchases/{id} - Get purchase by ID
- POST /api/propertypurchases - Create purchase (auto-creates accounting voucher)
- PUT /api/propertypurchases/{id} - Update purchase
- DELETE /api/propertypurchases/{id} - Delete purchase

#### Property Sales
- GET /api/propertysales - Get all sales (paginated)
- GET /api/propertysales/{id} - Get sale by ID
- POST /api/propertysales - Create sale (auto-creates accounting voucher)
- PUT /api/propertysales/{id} - Update sale
- DELETE /api/propertysales/{id} - Delete sale

#### Accounts (Chart of Accounts)
- GET /api/accounts - Get all accounts
- GET /api/accounts/by-type/{accountType} - Get accounts by type
- GET /api/accounts/{id} - Get account by ID
- POST /api/accounts - Create account
- PUT /api/accounts/{id} - Update account
- DELETE /api/accounts/{id} - Delete account
- POST /api/accounts/initialize - Initialize default accounts

#### Vouchers
- GET /api/vouchers - Get all vouchers (paginated)
- GET /api/vouchers/by-type/{voucherType} - Get vouchers by type
- GET /api/vouchers/{id} - Get voucher by ID with journal entries
- POST /api/vouchers - Create voucher with journal entries
- DELETE /api/vouchers/{id} - Delete voucher

## Frontend Implementation Status

### Menu Integration ✅ COMPLETED
- Real Estate menu items added to main-layout.component.html
- Conditional display based on businessNatureId === 6 (Real Estate)
- Menu structure:
  - Real Estate
    - Property Registration
    - Land Purchase
    - Land Selling
    - Ownership Transfer
  - Accounting
    - Chart of Accounts
    - Voucher Management
  - Financial Reports
    - General Ledger
    - Trial Balance
    - Balance Sheet
    - Profit & Loss

### Required Frontend Components (TO BE IMPLEMENTED)

#### 1. Property Registration Component
**Path**: `/realestate/properties`
**Purpose**: Manage property/land registration with structure details

**Features**:
- List all properties (table with pagination)
- Add new property (modal/form)
- Edit property details
- View property structure (rooms, floors, lawns, galleries)
- Property search and filter

**Form Fields**:
- Property Name*
- Property Type* (Land, Building, Commercial, Residential)
- Address, City, State, Zip Code
- Total Area*, Area Unit (sqft/sqm)
- Number of Rooms, Number of Floors
- Has Lawn (checkbox), Lawn Area
- Has Gallery (checkbox), Number of Galleries
- Custom Structure Details (textarea)
- Property Status (Available, Sold, Rented, Under Development)
- Current Value*
- Acquisition Date*
- Description
- Images

#### 2. Land Purchase Component
**Path**: `/realestate/purchases`
**Purpose**: Record property purchases (automatically creates accounting vouchers)

**Features**:
- List all purchases (table with pagination)
- Create new purchase (form/modal)
- View purchase details
- Link to generated voucher
- Payment tracking (advance, remaining)

**Form Fields**:
- Property* (dropdown)
- Purchase Number* (auto-generated)
- Seller Name*, Address, Phone, Email
- Purchase Date*
- Purchase Price*
- Advance Payment, Remaining Payment
- Payment Status (Pending, Partial, Completed)
- Payment Method (Cash, Bank, Cheque)
- Bank Name, Cheque Number (if applicable)
- Notes

**Automatic Accounting**:
- On submit, backend creates voucher with:
  - Debit: Property Assets (purchase price)
  - Credit: Cash/Bank (advance payment)
  - Credit: Accounts Payable (remaining payment)

#### 3. Land Selling Component
**Path**: `/realestate/sales`
**Purpose**: Record property sales (automatically creates accounting vouchers)

**Features**:
- List all sales (table with pagination)
- Create new sale (form/modal)
- View sale details
- Calculate profit/loss automatically
- Link to generated voucher
- Payment tracking (advance, remaining)

**Form Fields**:
- Property* (dropdown)
- Sale Number* (auto-generated)
- Buyer Name*, Address, Phone, Email
- Sale Date*
- Sale Price*
- Cost Price* (for profit/loss calculation)
- Profit/Loss (auto-calculated, read-only)
- Advance Received, Remaining Receivable
- Payment Status (Pending, Partial, Completed)
- Payment Method (Cash, Bank, Cheque)
- Bank Name, Cheque Number (if applicable)
- Notes

**Automatic Accounting**:
- On submit, backend creates voucher with:
  - Debit: Cash/Bank (advance received)
  - Debit: Accounts Receivable (remaining)
  - Credit: Property Assets (cost price)
  - Credit/Debit: Capital Gain/Loss (profit or loss)

#### 4. Ownership Transfer Component
**Path**: `/realestate/ownership-transfers`
**Purpose**: Track ownership changes

**Features**:
- List all transfers (table with pagination)
- Create new transfer (form/modal)
- View transfer details

**Form Fields**:
- Property* (dropdown)
- Transfer Number* (auto-generated)
- From Owner*
- To Owner*
- Transfer Date*
- Transfer Type (Sale, Gift, Inheritance, Court Order)
- Transfer Value
- Legal Document Number
- Notes

#### 5. Chart of Accounts Component
**Path**: `/accounting/chart-of-accounts`
**Purpose**: Manage the chart of accounts

**Features**:
- List all accounts (tree structure or table)
- Initialize default accounts (button)
- Add new account
- Edit account
- Delete account (if no transactions)
- View account balance

**Account Types**:
- Asset (Cash, Bank, Property Assets, Accounts Receivable)
- Liability (Accounts Payable)
- Equity (Capital, Retained Earnings)
- Revenue (Sales Revenue, Capital Gain)
- Expense (Capital Loss)

#### 6. Voucher Management Component
**Path**: `/accounting/vouchers`
**Purpose**: View and manage vouchers (auto-generated or manual)

**Features**:
- List all vouchers (table with pagination)
- Filter by type (Journal, Cash, Bank)
- View voucher details with journal entries
- Create manual voucher (with debit/credit validation)
- Delete voucher (reverses account balances)

#### 7. Financial Reports

##### General Ledger
**Path**: `/reports/general-ledger`
**Purpose**: Show all transactions for each account

**Features**:
- Select account (dropdown)
- Select date range
- Display transactions with running balance
- Export to PDF/Excel

##### Trial Balance
**Path**: `/reports/trial-balance`
**Purpose**: Show debit and credit totals for all accounts

**Features**:
- Select date
- Display all accounts with debit/credit balances
- Verify debits = credits
- Export to PDF/Excel

##### Balance Sheet
**Path**: `/reports/balance-sheet`
**Purpose**: Show financial position (Assets = Liabilities + Equity)

**Features**:
- Select date
- Display Assets, Liabilities, Equity
- Calculate totals
- Export to PDF/Excel

##### Profit & Loss Statement
**Path**: `/reports/profit-loss`
**Purpose**: Show income and expenses

**Features**:
- Select date range
- Display Revenue and Expenses
- Calculate Net Profit/Loss
- Show Capital Gains/Losses separately
- Export to PDF/Excel

## Implementation Steps

### Step 1: Create Models
Create TypeScript interfaces in `/src/app/models/`:
- property.model.ts
- property-purchase.model.ts
- property-sale.model.ts
- ownership-transfer.model.ts
- account.model.ts
- voucher.model.ts

### Step 2: Create Services
Create Angular services in `/src/app/services/`:
- property.service.ts
- property-purchase.service.ts
- property-sale.service.ts
- ownership-transfer.service.ts
- account.service.ts
- voucher.service.ts

### Step 3: Create Components
Generate components using Angular CLI:
```bash
ng generate component components/realestate/property-registration
ng generate component components/realestate/property-purchase
ng generate component components/realestate/property-sale
ng generate component components/realestate/ownership-transfer
ng generate component components/accounting/chart-of-accounts
ng generate component components/accounting/voucher-management
ng generate component components/reports/general-ledger
ng generate component components/reports/trial-balance
ng generate component components/reports/balance-sheet
ng generate component components/reports/profit-loss
```

### Step 4: Add Routes
Update `app-routing.module.ts` with new routes (see menu structure above)

### Step 5: Implement Forms
- Use PrimeNG components (p-table, p-dialog, p-dropdown, etc.)
- Add validation (required fields, number validation, date validation)
- Implement CRUD operations
- Show success/error notifications using NotificationService

### Step 6: Testing
- Test with Real Estate business nature (businessNatureId = 6)
- Verify menus appear only for Real Estate companies
- Test property CRUD operations
- Test purchase with automatic voucher creation
- Test sale with automatic voucher and profit/loss calculation
- Verify accounting entries are correct
- Test reports with sample data

## Database Migration

To apply the database changes:
```bash
cd Backend/AspNetCore/SimpleApi
dotnet ef database update
```

This will create all necessary tables:
- Properties
- PropertyPurchases
- PropertySales
- OwnershipTransfers
- Accounts
- Vouchers
- JournalEntries

## Business Nature Setup

To use Real Estate features:
1. Login as Super Admin
2. Go to Administration > Company Setup
3. Select "Real Estate" as Business Nature
4. Real Estate and Accounting menus will appear automatically

## Next Steps

1. Implement the frontend components as described above
2. Test the complete workflow end-to-end
3. Add validation and error handling
4. Implement report generation logic
5. Add export functionality (PDF/Excel)
6. Create user documentation
7. Add screenshots and demo video

## Notes

- All API endpoints are multi-tenant aware (use CompanyId from JWT)
- Accounting entries are created automatically on purchase/sale
- Vouchers validate that debits equal credits
- Account balances are updated automatically
- Property status is updated automatically on sale
- All operations respect RBAC (only Super Admin can access by default)
