import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { MainLayoutComponent } from './components/main-layout/main-layout.component';
import { SaleOrderComponent } from './components/sale-order/sale-order.component';
import { ProductComponent } from './components/product/product.component';
import { CustomerComponent } from './components/customer/customer.component';
import { CompanySetupComponent } from './components/company-setup/company-setup.component';
import { UserManagementComponent } from './components/user-management/user-management.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { ProjectListComponent } from './components/project/project-list.component';
import { ProjectFormComponent } from './components/project/project-form.component';
import { ProjectDetailComponent } from './components/project/project-detail.component';
import { ServiceTypeComponent } from './components/service-type/service-type.component';
import { WriterAssignmentsComponent } from './components/writer-portal/writer-assignments.component';
import { WriterDashboardComponent } from './components/writer-portal/writer-dashboard.component';
import { TeamLeadOrdersComponent } from './components/team-lead/team-lead-orders.component';
import { TeamLeadDashboardComponent } from './components/team-lead/team-lead-dashboard.component';
import { BrandingDemoComponent } from './components/branding-demo/branding-demo.component';
// Real Estate components
import { RealEstateDashboardComponent } from './components/realestate/realestate-dashboard/realestate-dashboard.component';
import { PropertyRegistrationComponent } from './components/realestate/property-registration/property-registration.component';
import { PropertyPurchaseComponent } from './components/realestate/property-purchase/property-purchase.component';
import { PropertySaleComponent } from './components/realestate/property-sale/property-sale.component';
import { OwnershipTransferComponent } from './components/realestate/ownership-transfer/ownership-transfer.component';
// Accounting components
import { ChartOfAccountsComponent } from './components/accounting/chart-of-accounts/chart-of-accounts.component';
import { VoucherManagementComponent } from './components/accounting/voucher-management/voucher-management.component';
// Reports components
import { GeneralLedgerComponent } from './components/reports/general-ledger/general-ledger.component';
import { TrialBalanceComponent } from './components/reports/trial-balance/trial-balance.component';
import { BalanceSheetComponent } from './components/reports/balance-sheet/balance-sheet.component';
import { ProfitLossComponent } from './components/reports/profit-loss/profit-loss.component';
// import { CurrencySetupComponent } from './components/currency-setup/currency-setup.component';
import { AuthGuard } from './guards/auth.guard';
import { SuperAdminGuard } from './guards/super-admin.guard';
import { WriterGuard } from './guards/writer.guard';
import { TeamLeadGuard } from './guards/team-lead.guard';
import { AdminDashboardGuard } from './guards/admin-dashboard.guard';
import { OrderChatbotComponent } from './components/order-chatbot/order-chatbot.component';
import { OrderWorkflowDiagramComponent } from './components/order-workflow-diagram/order-workflow-diagram.component';

const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegistrationComponent },
    { path: 'demo', component: BrandingDemoComponent },
    {
        path: '',
        component: MainLayoutComponent,
        canActivate: [AuthGuard],
        children: [
            { path: '', component: DashboardComponent, canActivate: [AdminDashboardGuard] },
            { path: 'dashboard', component: DashboardComponent, canActivate: [AdminDashboardGuard] },

            // Real Estate Routes
            { path: 'realestate/dashboard', component: RealEstateDashboardComponent },
            { path: 'realestate/properties', component: PropertyRegistrationComponent },
            { path: 'realestate/purchases', component: PropertyPurchaseComponent },
            { path: 'realestate/sales', component: PropertySaleComponent },
            { path: 'realestate/ownership-transfers', component: OwnershipTransferComponent },

            // Accounting Routes
            { path: 'accounting/chart-of-accounts', component: ChartOfAccountsComponent },
            { path: 'accounting/vouchers', component: VoucherManagementComponent },

            // Reports Routes
            { path: 'reports/general-ledger', component: GeneralLedgerComponent },
            { path: 'reports/trial-balance', component: TrialBalanceComponent },
            { path: 'reports/balance-sheet', component: BalanceSheetComponent },
            { path: 'reports/profit-loss', component: ProfitLossComponent },

            // Construction Routes
            {
                path: 'construction/dashboard',
                loadComponent: () =>
                    import('./components/construction/construction-dashboard.component').then(
                        (m) => m.ConstructionDashboardComponent
                    ),
            },
            {
                path: 'construction/projects',
                loadComponent: () =>
                    import('./components/construction/projects/construction-projects.component').then(
                        (m) => m.ConstructionProjectsComponent
                    ),
            },
            {
                path: 'construction/clients',
                loadComponent: () =>
                    import('./components/construction/clients/construction-clients.component').then(
                        (m) => m.ConstructionClientsComponent
                    ),
            },
            {
                path: 'construction/investors',
                loadComponent: () =>
                    import('./components/construction/investors/construction-investors.component').then(
                        (m) => m.ConstructionInvestorsComponent
                    ),
            },
            {
                path: 'construction/payments',
                loadComponent: () =>
                    import('./components/construction/payments/construction-payments.component').then(
                        (m) => m.ConstructionPaymentsComponent
                    ),
            },
            {
                path: 'construction/expenses',
                loadComponent: () =>
                    import('./components/construction/expenses/construction-expenses.component').then(
                        (m) => m.ConstructionExpensesComponent
                    ),
            },
            {
                path: 'construction/project-ledger',
                data: { ledgerType: 'project' },
                loadComponent: () =>
                    import('./components/construction/ledgers/construction-ledgers.component').then(
                        (m) => m.ConstructionLedgersComponent
                    ),
            },
            {
                path: 'construction/client-ledger',
                data: { ledgerType: 'client' },
                loadComponent: () =>
                    import('./components/construction/ledgers/construction-ledgers.component').then(
                        (m) => m.ConstructionLedgersComponent
                    ),
            },
            {
                path: 'construction/investor-ledger',
                data: { ledgerType: 'investor' },
                loadComponent: () =>
                    import('./components/construction/ledgers/construction-ledgers.component').then(
                        (m) => m.ConstructionLedgersComponent
                    ),
            },
            {
                path: 'construction/profit-loss',
                loadComponent: () =>
                    import('./components/construction/ledgers/construction-profit-loss.component').then(
                        (m) => m.ConstructionProfitLossComponent
                    ),
            },

            // Existing Routes
            { path: 'transactions/sale-orders', component: SaleOrderComponent },
            { path: 'order-entry', component: OrderChatbotComponent, canActivate: [AuthGuard] },
            { path: 'order-workflow/:id', component: OrderWorkflowDiagramComponent, canActivate: [AuthGuard] },
            { path: 'masters/products', component: ProductComponent },
            { path: 'masters/customers', component: CustomerComponent },
            { path: 'masters/projects', component: ProjectListComponent },
            { path: 'masters/projects/create', component: ProjectFormComponent },
            { path: 'masters/projects/edit/:id', component: ProjectFormComponent },
            { path: 'masters/projects/:id', component: ProjectDetailComponent },
            { path: 'masters/service-types', component: ServiceTypeComponent },
            {
                path: 'masters/currency-setup',
                loadComponent: () =>
                    import('./components/currency-setup/currency-setup.component').then(
                        (m) => m.CurrencySetupComponent
                    ),
            },
            { path: 'administration/company-setup', component: CompanySetupComponent, canActivate: [SuperAdminGuard] },
            {
                path: 'administration/user-management',
                component: UserManagementComponent,
                canActivate: [SuperAdminGuard],
            },
            {
                path: 'writer/assignments',
                component: WriterAssignmentsComponent,
                canActivate: [WriterGuard],
            },
            {
                path: 'writer/dashboard',
                component: WriterDashboardComponent,
                canActivate: [WriterGuard],
            },
            {
                path: 'team-lead/dashboard',
                component: TeamLeadDashboardComponent,
                canActivate: [TeamLeadGuard],
            },
            {
                path: 'team-lead/orders',
                component: TeamLeadOrdersComponent,
                canActivate: [TeamLeadGuard],
            },
        ],
    },
    { path: '**', redirectTo: '/login' },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})
export class AppRoutingModule { }
