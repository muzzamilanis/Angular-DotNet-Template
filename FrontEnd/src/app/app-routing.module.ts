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
import { AuthGuard } from './guards/auth.guard';
import { SuperAdminGuard } from './guards/super-admin.guard';

const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegistrationComponent },
    {
        path: '',
        component: MainLayoutComponent,
        canActivate: [AuthGuard],
        children: [
            { path: '', component: DashboardComponent },
            { path: 'dashboard', component: DashboardComponent },
            { path: 'transactions/sale-orders', component: SaleOrderComponent },
            { path: 'masters/products', component: ProductComponent },
            { path: 'masters/customers', component: CustomerComponent },
            { path: 'masters/projects', component: ProjectListComponent },
            { path: 'masters/projects/create', component: ProjectFormComponent },
            { path: 'masters/projects/edit/:id', component: ProjectFormComponent },
            { path: 'masters/projects/:id', component: ProjectDetailComponent },
            { path: 'masters/service-types', component: ServiceTypeComponent },
            { path: 'administration/company-setup', component: CompanySetupComponent, canActivate: [SuperAdminGuard] },
            {
                path: 'administration/user-management',
                component: UserManagementComponent,
                canActivate: [SuperAdminGuard],
            },
        ],
    },
    { path: '**', redirectTo: '/login' },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})
export class AppRoutingModule {}
