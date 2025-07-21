import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { MainLayoutComponent } from './components/main-layout/main-layout.component';
import { SaleOrderComponent } from './components/sale-order/sale-order.component';
import { ProductComponent } from './components/product/product.component';
import { CustomerComponent } from './components/customer/customer.component';
import { CompanySetupComponent } from './components/company-setup/company-setup.component';
import { UserManagementComponent } from './components/user-management/user-management.component';
import { AuthGuard } from './guards/auth.guard';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { 
    path: '', 
    component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: '/transactions/sale-orders', pathMatch: 'full' },
      { path: 'transactions/sale-orders', component: SaleOrderComponent },
      { path: 'masters/products', component: ProductComponent },
      { path: 'masters/customers', component: CustomerComponent },
      { path: 'administration/company-setup', component: CompanySetupComponent },
      { path: 'administration/user-management', component: UserManagementComponent },
    ]
  },
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }