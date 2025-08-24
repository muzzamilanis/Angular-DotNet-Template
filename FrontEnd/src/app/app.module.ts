import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { MainLayoutComponent } from './components/main-layout/main-layout.component';
import { SaleOrderComponent } from './components/sale-order/sale-order.component';
import { ProductComponent } from './components/product/product.component';
import { CustomerComponent } from './components/customer/customer.component';
import { CompanySetupComponent } from './components/company-setup/company-setup.component';
import { UserManagementComponent } from './components/user-management/user-management.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { DeadlineNotificationsComponent } from './components/dashboard/deadline-notifications/deadline-notifications.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { ProjectComponent } from './components/project/project.component';
import { ProjectListComponent } from './components/project/project-list.component';
import { ProjectFormComponent } from './components/project/project-form.component';
import { ProjectDetailComponent } from './components/project/project-detail.component';
import { ServiceTypeComponent } from './components/service-type/service-type.component';
import { WriterAssignmentsComponent } from './components/writer-portal/writer-assignments.component';
import { WriterDashboardComponent } from './components/writer-portal/writer-dashboard.component';
import { TeamLeadOrdersComponent } from './components/team-lead/team-lead-orders.component';
import { TeamLeadDashboardComponent } from './components/team-lead/team-lead-dashboard.component';
import { BrandingDemoComponent } from './components/branding-demo/branding-demo.component';
// import { CurrencySetupComponent } from './components/currency-setup/currency-setup.component';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { ThemeService } from './services/theme.service';
import { CompanyService } from './services/company.service';

// Initialize the theme before the app renders
export function initializeAppTheme(themeService: ThemeService, companyService: CompanyService) {
    return () =>
        new Promise<void>((resolve) => {
            companyService.getCurrentCompany().subscribe({
                next: (company) => {
                    if (company?.primaryColor) {
                        themeService.setTheme(company.primaryColor);
                    }
                    resolve();
                },
                error: () => resolve(), // continue even if not authenticated yet
            });
        });
}

@NgModule({
    declarations: [
        AppComponent,
        LoginComponent,
        MainLayoutComponent,
        SaleOrderComponent,
        ProductComponent,
        CustomerComponent,
        CompanySetupComponent,
        UserManagementComponent,
        DashboardComponent,
        DeadlineNotificationsComponent,
        RegistrationComponent,
        ProjectComponent,
        ProjectListComponent,
        ProjectFormComponent,
        ProjectDetailComponent,
        ServiceTypeComponent,
        WriterAssignmentsComponent,
        WriterDashboardComponent,
        TeamLeadOrdersComponent,
        TeamLeadDashboardComponent,
        BrandingDemoComponent,
        // CurrencySetupComponent,
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        HttpClientModule,
        ReactiveFormsModule,
        FormsModule,
        BrowserAnimationsModule,
    ],
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true,
        },
        {
            provide: APP_INITIALIZER,
            useFactory: initializeAppTheme,
            deps: [ThemeService, CompanyService],
            multi: true,
        },
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
