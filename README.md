# SmartBizApp

[![Backend CI/CD](https://github.com/muzzamilanis/Angular-DotNet-Template/workflows/Backend%20CI%2FCD/badge.svg)](https://github.com/muzzamilanis/Angular-DotNet-Template/actions/workflows/backend-ci.yml)
[![Frontend CI/CD](https://github.com/muzzamilanis/Angular-DotNet-Template/workflows/Frontend%20CI%2FCD/badge.svg)](https://github.com/muzzamilanis/Angular-DotNet-Template/actions/workflows/frontend-ci.yml)
[![Full Stack Integration](https://github.com/muzzamilanis/Angular-DotNet-Template/workflows/Full%20Stack%20Integration/badge.svg)](https://github.com/muzzamilanis/Angular-DotNet-Template/actions/workflows/integration.yml)
[![Code Quality](https://github.com/muzzamilanis/Angular-DotNet-Template/workflows/Code%20Quality/badge.svg)](https://github.com/muzzamilanis/Angular-DotNet-Template/actions/workflows/code-quality.yml)

SmartBizApp is a comprehensive business management platform with **ASP.NET Core** backend, **Angular** web frontend, and **React Native** mobile app, designed to streamline business operations and enhance productivity across all devices.

## Features

### Core Business Management
- **User Management**: Comprehensive user administration with role-based access control (RBAC)
- **Dashboard**: Real-time business insights and analytics with deadline notifications
- **Company Setup**: Multi-tenant company configuration with business nature selection (Retail, Manufacturing, Services, Hospitality, Real Estate)
- **Project Management**: Complete project lifecycle management with status tracking, deadlines, and service type assignments
- **Sales Order Management**: Full sales order processing with customer assignments, project linking, and deadline tracking
- **Customer Management**: Comprehensive customer database with company associations and relationship management
- **Product Management**: Product catalog management with detailed information and company-based organization
- **Service Types Management**: Configurable service categories for project classification and workflow management

### Real Estate Management (NEW)
- **Property Registration**: Complete property/land management with detailed structure information (rooms, lawns, galleries, floors, custom details)
- **Land Purchase Transactions**: Property acquisition tracking with automated accounting voucher generation
- **Land Selling Transactions**: Property sale management with automatic profit/loss calculation and accounting entries
- **Ownership Transfer**: Track property ownership changes with legal documentation
- **Chart of Accounts**: Comprehensive accounting system with hierarchical account structure
- **Automated Accounting**: Automatic debit/credit entries for purchase and sale transactions
- **Voucher Management**: Journal, Cash, and Bank vouchers with automatic balance updates
- **Financial Reports**: General Ledger, Trial Balance, Balance Sheet, and Profit & Loss statements
- **Multi-tenant Support**: Complete data isolation per company with business-nature-based menu visibility

### Advanced Features
- **Role-Based Access Control (RBAC)**: Super Admin and regular user roles with granular permissions and route protection
- **Email Notification System**: Automated deadline notifications with configurable SMTP settings and manual triggers
- **Multi-tenant Architecture**: Company-based data isolation, access control, and business nature support
- **Real-time Notifications**: Dashboard notifications for approaching deadlines with background service monitoring
- **Team Lead & Writer Portals**: Specialized interfaces for different user roles with tailored functionality
- **Currency Management**: Multi-currency support for international business operations
- **Registration System**: Self-service user registration with company association and approval workflows
- **Background Services**: Automated deadline monitoring, email notifications, and system maintenance tasks
- **Responsive Design**: Modern UI that works seamlessly on desktop, tablet, and mobile devices
- **API Documentation**: Comprehensive Swagger/OpenAPI documentation with interactive testing capabilities

### Security & Authentication
- **JWT Authentication**: Secure token-based authentication with refresh capabilities
- **Authorization Guards**: Route and component-level permission checking
- **Secure Configuration**: Environment-specific settings with production-ready defaults
- **Password Security**: Secure password hashing and validation

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 Web API with .NET 8.0 runtime
- **Database**: PostgreSQL 15+ with Entity Framework Core 9.0 ORM
- **Authentication**: JWT Bearer tokens with System.IdentityModel.Tokens.Jwt 7.4.1
- **API Documentation**: Swagger/OpenAPI integration with Swashbuckle.AspNetCore 6.6.2
- **Object Mapping**: AutoMapper 12.0.1 for entity-to-DTO mapping
- **Real-time Communication**: SignalR integration for live notifications
- **Background Services**: .NET hosted services for automated tasks
- **Development Database**: SQLite support for local development environment

### Frontend
- **Framework**: Angular 19.2.1 with standalone components and modern routing
- **Language**: TypeScript 5.8.2 with strict typing configuration
- **UI Components**: PrimeNG 19.0.9 with comprehensive component library and theming
- **CSS Framework**: Bootstrap 5.3.7 with responsive grid system
- **Admin Theme**: AdminLTE 3.2.0 for professional dashboard interface
- **Icons**: FontAwesome 6.7.2 with extensive icon library
- **State Management**: RxJS 7.8.2 with reactive programming patterns
- **Real-time Updates**: Microsoft SignalR 8.0.7 client integration
- **Date/Time**: Moment.js 2.30.1 with timezone support (Moment-timezone 0.5.47)
- **Notifications**: SweetAlert2 11.22.2 for user-friendly alert system

### Mobile App (NEW)
- **Framework**: React Native with Expo SDK 53.0.0
- **Language**: TypeScript 5.8.3 with strict typing
- **Navigation**: React Navigation 6.0 with stack and tab navigators
- **State Management**: React Context API with AsyncStorage persistence
- **HTTP Client**: Axios with request/response interceptors
- **UI Components**: Expo Vector Icons with native iOS/Android styling
- **Authentication**: JWT token management with secure storage
- **Platform Support**: iOS, Android, and Web (Expo Web)

### Development & Deployment
- **Linting**: ESLint 9.21.0 with Angular-specific rules and TypeScript integration
- **Code Quality**: Prettier 5.2.3 for consistent code formatting
- **Containerization**: Docker with multi-stage builds and Docker Compose orchestration
- **Production Database**: PostgreSQL 15+ with Npgsql.EntityFrameworkCore.PostgreSQL 9.0.0
- **Development Database**: SQLite for streamlined local development
- **Package Management**: npm with lock file support and security auditing
- **Testing**: Jasmine 5.6.0 and Karma 6.4.4 for unit testing
- **Build Tools**: Angular CLI 19.2.1 with Webpack bundling and optimization

# Default Login Credentials

After setting up the project and running the database migrations, you can log in using these default credentials:

**Username:** `admin`  
**Password:** `123qwe`

> ⚠️ **Security Warning**: These are default credentials for development purposes only. **Always change the default password immediately** after first login, especially in production environments.

The default admin user is created during database seeding with the email `admin@smartbizapp.com`.

## Getting Started

### Prerequisites

- **.NET 8.0 SDK** or later
- **Node.js 18+** and npm (or yarn)
- **PostgreSQL 15+** (for production) or SQLite (for development)
- **Docker** (optional, for containerized deployment)
- **Visual Studio Code** or **Visual Studio 2022** (recommended IDEs)

### Installation

#### Option 1: Traditional Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd SmartBizApp
   ```

2. **Backend Setup**
   ```bash
   cd Backend/AspNetCore/SimpleApi
   dotnet restore
   
   # For development (uses SQLite)
   dotnet ef database update
   
   # For production with PostgreSQL, update connection string in appsettings.json first
   dotnet run
   ```

3. **Frontend Setup**
   ```bash
   cd FrontEnd
   npm install
   
   # For development server
   npm start
   # or with yarn
   # yarn install && yarn start
   ```

4. **Mobile App Setup (NEW)**
   ```bash
   cd Mobile/SmartBizApp
   npm install
   
   # Install Expo CLI globally if not already installed
   npm install -g @expo/cli
   
   # Start the development server
   npm start
   
   # Run on specific platform
   npm run ios     # iOS simulator
   npm run android # Android emulator
   npm run web     # Web browser
   ```

5. **Access the Applications**
   - Web Frontend: http://localhost:4200
   - Mobile App: Use Expo Go app to scan QR code or run on simulator
   - Backend API: http://localhost:5000 (or 5001 for HTTPS)
   - API Documentation: http://localhost:5000/swagger

6. **Database Configuration**
   
   **Development (SQLite - Default):**
   - No additional setup required
   - Database file created automatically as `testdb.sqlite`
   
   **Production (PostgreSQL):**
   ```json
   // Update appsettings.json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=SmartBizAppDB;Username=your_user;Password=your_password"
     }
   }
   ```

#### Option 2: Docker Deployment

For easy deployment using Docker, see [DOCKER_README.md](DOCKER_README.md) for complete instructions.

**Quick Start with Docker:**
```bash
# Run the complete stack (API + PostgreSQL)
docker-compose up --build

# Access the API at:
# HTTP: http://localhost:8080
# HTTPS: http://localhost:8081
```

#### Option 3: Render.com Deployment

For deploying to Render.com, set these environment variables in your Render service:

**Required Environment Variables:**
- `ConnectionStrings__DefaultConnection` - Your PostgreSQL connection string
- `Jwt__Key` - JWT signing key (minimum 256 bits)

**Example PostgreSQL connection string format for Render:**
```
Host=your-postgres-host;Database=your-db-name;Username=your-username;Password=your-password;Port=5432;SSL Mode=Require;Trust Server Certificate=true
```

## Latest Features & Architecture

### Role-Based Access Control (RBAC)
The application implements a sophisticated RBAC system with comprehensive security features:
- **Super Admin**: Complete system access, user management, company management, and system administration
- **Regular Users**: Company-specific access with role-based limitations and data isolation
- **Team Leads**: Enhanced permissions for project oversight and team management
- **Writers**: Specialized portal access with task-specific functionality
- **Route Guards**: Automatic redirection and access control based on user permissions
- **API Authorization**: Backend validation for all protected endpoints with JWT verification
- **Component-level Security**: UI elements dynamically shown/hidden based on user roles

For detailed implementation, see [RBAC_IMPLEMENTATION.md](RBAC_IMPLEMENTATION.md)

### Email Notification System
Comprehensive automated notification system with advanced features:
- **Deadline Notifications**: Intelligent email alerts for approaching project deadlines (3-day threshold)
- **Background Services**: Continuous monitoring service that runs immediately on startup and every 24 hours
- **Manual Triggers**: API endpoints for testing and immediate notification execution (Super Admin only)
- **SMTP Configuration**: Full support for Gmail, Outlook, and custom SMTP providers
- **Rich HTML Emails**: Professional email templates with order details and urgency indicators
- **Multi-recipient Support**: Notifications sent to Super Admins and assigned team members
- **Configuration Validation**: Comprehensive email setting validation with detailed error logging

For setup and configuration, see [NOTIFICATION_SYSTEM.md](NOTIFICATION_SYSTEM.md)

### Multi-Tenant Architecture
Enterprise-grade multi-tenancy with complete data isolation:
- **Company-Based Segmentation**: Complete data separation between different companies
- **Business Nature Support**: Configurable business types (Retail, Manufacturing, Services, Hospitality)
- **Tenant-Aware APIs**: All endpoints respect company boundaries and access controls
- **Scalable Design**: Architecture supports unlimited companies with isolated data
- **Secure Access Control**: Users can only access data within their assigned company

### Advanced Business Modules
Comprehensive business management capabilities:
- **Project Lifecycle Management**: Complete project tracking from initiation to completion
- **Sales Order Processing**: Full order management with customer linking and deadline tracking
- **Customer Relationship Management**: Comprehensive customer database with company associations
- **Product Catalog Management**: Detailed product information with company-based organization
- **Service Type Configuration**: Flexible service categorization for workflow management
- **Currency Management**: Multi-currency support for international operations
- **Team Collaboration Tools**: Specialized portals for different user roles and responsibilities

### Modern Development Stack & Best Practices
- **ESLint 9.21.0**: Advanced linting with Angular-specific rules, TypeScript integration, and code quality enforcement
- **TypeScript 5.8.2**: Latest TypeScript features with strict typing, enhanced IntelliSense, and compile-time error detection
- **PrimeNG 19.0.9**: Enterprise-grade UI component library with comprehensive theming and accessibility features
- **Angular 19.2.1**: Latest Angular features including standalone components, improved performance, and modern development patterns
- **Entity Framework Core 9.0**: Advanced ORM with PostgreSQL optimization, query performance, and database relationship management
- **AutoMapper 12.0.1**: Efficient object-to-object mapping with performance optimization and configuration validation
- **JWT Security**: Industry-standard authentication with secure token management and refresh capabilities
- **SignalR Integration**: Real-time communication for live notifications and collaborative features
- **Docker Support**: Complete containerization with multi-stage builds, development and production configurations
- **Code Quality Tools**: Prettier formatting, ESLint rules, TypeScript strict mode, and automated testing frameworks

# Screenshots

## Dashboard and Analytics
The main dashboard provides real-time business insights with deadline notifications and key performance indicators.

#### Dashboard
![](_screenshots/module-zero-core-template-ui-home.png)

## Administration and User Management
Comprehensive user management interface with role-based access control and company association features.

#### User Management
![](_screenshots/module-zero-core-template-ui-user-create-modal.png)

## Authentication System
Secure login interface with JWT token-based authentication and role-based redirection.

#### Login Page
![](_screenshots/module-zero-core-template-ui-login.png)

## Business Modules
The application includes comprehensive modules for:
- **Project Management**: Complete project lifecycle with status tracking and deadline management
- **Sales Order Processing**: Customer-linked order management with project associations
- **Customer Relationship Management**: Comprehensive customer database with company segmentation
- **Product Catalog**: Detailed product management with categorization and company-based organization
- **Service Type Configuration**: Flexible service categorization for workflow optimization
- **Team Collaboration**: Specialized portals for Team Leads and Writers with role-specific functionality

## Development

### Project Structure

```
SmartBizApp/
├── Backend/AspNetCore/SimpleApi/    # ASP.NET Core 8.0 Web API
│   ├── Controllers/                 # API endpoints for all business modules
│   │   ├── AuthController.cs        # Authentication and JWT token management
│   │   ├── CompaniesController.cs   # Company setup and multi-tenant management
│   │   ├── UsersController.cs       # User management with RBAC
│   │   ├── ProjectsController.cs    # Project lifecycle management
│   │   ├── SaleOrdersController.cs  # Sales order processing
│   │   ├── CustomersController.cs   # Customer relationship management
│   │   ├── ProductsController.cs    # Product catalog management
│   │   ├── ServiceTypesController.cs # Service type configuration
│   │   ├── NotificationsController.cs # Email notifications and alerts
│   │   ├── CurrencyController.cs    # Multi-currency support
│   │   ├── TeamLeadController.cs    # Team lead portal functionality
│   │   └── WritersController.cs     # Writer portal management
│   ├── Services/                    # Business logic and email services
│   │   ├── AuthService.cs           # Authentication and user management
│   │   ├── EmailService.cs          # SMTP email functionality
│   │   ├── DeadlineNotificationService.cs # Background deadline monitoring
│   │   └── UserService.cs           # User operations and RBAC logic
│   ├── Models/                      # Entity models and database schema
│   ├── DTOs/                        # Data transfer objects with validation
│   ├── Data/                        # Entity Framework DbContext and configurations
│   └── Migrations/                  # Database migrations and schema updates
├── FrontEnd/                        # Angular 19.2.1 Application
│   ├── src/app/components/          # Angular components for all modules
│   │   ├── dashboard/               # Main dashboard with notifications
│   │   ├── login/                   # Authentication interface
│   │   ├── user-management/         # User administration (Super Admin only)
│   │   ├── company-setup/           # Company configuration and setup
│   │   ├── project/                 # Project management interface
│   │   ├── sale-order/              # Sales order processing
│   │   ├── customer/                # Customer management
│   │   ├── product/                 # Product catalog
│   │   ├── service-type/            # Service type configuration
│   │   ├── team-lead/               # Team lead portal
│   │   ├── writer-portal/           # Writer-specific interface
│   │   ├── currency-setup/          # Currency management
│   │   ├── registration/            # User registration system
│   │   └── main-layout/             # Application shell and navigation
│   ├── src/app/services/            # HTTP services and business logic
│   ├── src/app/guards/              # Route guards for RBAC and authentication
│   ├── src/app/models/              # TypeScript interfaces and data models
│   ├── src/app/interceptors/        # HTTP interceptors for JWT and error handling
│   └── src/assets/                  # Static assets, themes, and styling
├── _screenshots/                    # Application screenshots and documentation
├── RBAC_IMPLEMENTATION.md           # Comprehensive RBAC documentation
├── NOTIFICATION_SYSTEM.md           # Email notification system setup guide
├── COMPANY_SETUP_WORKFLOW.md        # Company setup and user management workflow
├── DOCKER_README.md                 # Docker deployment and containerization guide
├── docker-compose.yml               # Multi-container orchestration configuration
└── README.md                        # This comprehensive documentation file
```

### Building for Production

**Backend:**
```bash
cd Backend/AspNetCore/SimpleApi
dotnet publish -c Release -o ./publish
```

**Frontend:**
```bash
cd FrontEnd
npm run build
# Build artifacts will be in dist/ folder
```

### Development Workflow

**Code Quality and Linting:**
```bash
# Frontend linting with ESLint
cd FrontEnd
npm run lint

# Format code with Prettier for consistent styling
npm run prettier

# Type checking with TypeScript compiler
npx tsc --noEmit
```

**Testing and Validation:**
```bash
# Frontend unit tests with Jasmine and Karma
cd FrontEnd
npm test

# Backend compilation and validation
cd Backend/AspNetCore/SimpleApi
dotnet build

# Run backend with hot reload for development
dotnet watch run

# Frontend development server with hot module replacement
cd FrontEnd
npm start
```

**Database Management:**
```bash
# Create new migration for schema changes
cd Backend/AspNetCore/SimpleApi
dotnet ef migrations add YourMigrationName

# Apply migrations to update database schema
dotnet ef database update

# Generate migration script for production deployment
dotnet ef migrations script
```

## Contributing

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Install dependencies and setup development environment**
   ```bash
   # Backend
   cd Backend/AspNetCore/SimpleApi
   dotnet restore
   
   # Frontend  
   cd FrontEnd
   npm install
   ```
4. **Make your changes following the project conventions**
   - Follow ESLint rules for frontend code
   - Use proper TypeScript typing
   - Follow .NET coding standards
   - Add appropriate error handling
5. **Test your changes**
   ```bash
   # Lint frontend code
   cd FrontEnd && npm run lint
   
   # Run frontend tests
   npm test
   
   # Test backend compilation
   cd Backend/AspNetCore/SimpleApi && dotnet build
   ```
6. **Submit a pull request** with a clear description of changes

### Development Guidelines
- **Code Style**: Follow ESLint and Prettier configurations
- **TypeScript**: Use strict typing, avoid `any` type
- **Components**: Follow Angular best practices and component structure
- **API**: Follow RESTful conventions and proper HTTP status codes
- **Database**: Use Entity Framework migrations for schema changes

## Support

For support and questions, please create an issue in the repository.

## Additional Documentation

- **[RBAC Implementation Guide](RBAC_IMPLEMENTATION.md)**: Comprehensive documentation on role-based access control, security features, and user permission management
- **[Email Notification System](NOTIFICATION_SYSTEM.md)**: Complete setup guide for automated deadline notifications, SMTP configuration, and testing procedures  
- **[Docker Deployment Guide](DOCKER_README.md)**: Detailed containerization instructions, multi-container orchestration, and production deployment
- **[Company Setup Workflow](COMPANY_SETUP_WORKFLOW.md)**: Step-by-step guide for company configuration, business nature selection, and multi-tenant setup

## Technical Resources

- **API Documentation**: Interactive Swagger UI available at `/swagger` endpoint when running the backend application
- **Frontend Architecture**: Angular 19.2.1 with standalone components, modern routing, and reactive programming patterns
- **Database Schema**: Complete Entity Framework migrations located in `Backend/AspNetCore/SimpleApi/Migrations/`
- **Configuration Management**: Environment-specific settings in `appsettings.json` files with secure production defaults
- **Development Tools**: ESLint configuration, Prettier formatting, and TypeScript strict mode for code quality
- **Container Support**: Multi-stage Docker builds with development and production optimizations

# License

[MIT](LICENSE).
