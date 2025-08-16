# SmartBizApp

SmartBizApp is a modern business management application built with **ASP.NET Core** backend and **Angular** frontend, designed to streamline business operations and enhance productivity.

## Features

### Core Business Management
- **User Management**: Comprehensive user administration with role-based access control (RBAC)
- **Dashboard**: Real-time business insights and analytics with deadline notifications
- **Company Setup**: Multi-tenant company configuration and management
- **Master Data Management**: Manage products, customers, and other core business entities
- **Transaction Processing**: Handle sales orders and business transactions
- **Project Management**: Full project lifecycle management with service types

### Advanced Features
- **Role-Based Access Control (RBAC)**: Super Admin and regular user roles with granular permissions
- **Email Notification System**: Automated deadline notifications with configurable SMTP settings
- **Multi-tenant Architecture**: Company-based data isolation and access control
- **Real-time Notifications**: Dashboard notifications for approaching deadlines
- **Responsive Design**: Modern UI that works on desktop, tablet, and mobile devices

### Security & Authentication
- **JWT Authentication**: Secure token-based authentication with refresh capabilities
- **Authorization Guards**: Route and component-level permission checking
- **Secure Configuration**: Environment-specific settings with production-ready defaults
- **Password Security**: Secure password hashing and validation

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 Web API
- **Database**: PostgreSQL with Entity Framework Core 9.0
- **Authentication**: JWT Bearer tokens with secure configuration
- **API Documentation**: Swagger/OpenAPI integration
- **Mapping**: AutoMapper for object-to-object mapping

### Frontend
- **Framework**: Angular 19.2.1 (Latest)
- **Language**: TypeScript 5.8.2
- **UI Components**: PrimeNG 19.0.9 with custom theming
- **CSS Framework**: Bootstrap 5.3.7
- **Theme**: AdminLTE 3.2.0 for modern admin interface
- **Icons**: FontAwesome 6.7.2
- **State Management**: RxJS 7.8.2 with reactive patterns

### Development & Deployment
- **Linting**: ESLint 9.21.0 with Angular-specific rules
- **Containerization**: Docker with multi-stage builds
- **Database**: PostgreSQL 15+ for production
- **Development DB**: SQLite for local development
- **Package Management**: npm with lock file support

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

4. **Access the Application**
   - Frontend: http://localhost:4200
   - Backend API: http://localhost:5000 (or 5001 for HTTPS)
   - API Documentation: http://localhost:5000/swagger

5. **Database Configuration**
   
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

## Latest Features & Architecture

### Role-Based Access Control (RBAC)
The application implements a comprehensive RBAC system with:
- **Super Admin**: Full system access, user management, company management
- **Regular Users**: Company-specific access with limited permissions
- **Route Guards**: Automatic redirection based on user permissions
- **API Authorization**: Backend validation for all protected endpoints

For detailed implementation, see [RBAC_IMPLEMENTATION.md](RBAC_IMPLEMENTATION.md)

### Email Notification System
Automated notification system featuring:
- **Deadline Notifications**: Automatic emails for approaching project deadlines
- **Background Services**: Runs continuously to monitor deadlines
- **Manual Triggers**: API endpoints for testing and immediate notifications
- **SMTP Configuration**: Supports Gmail and other SMTP providers

For setup and configuration, see [NOTIFICATION_SYSTEM.md](NOTIFICATION_SYSTEM.md)

### Modern Development Stack
- **ESLint 9.21.0**: Modern linting with Angular-specific rules
- **TypeScript 5.8.2**: Latest TypeScript features and strict typing
- **PrimeNG Components**: Rich UI component library with theming
- **Angular 19.2.1**: Latest Angular features and performance improvements
- **Entity Framework Core 9.0**: Modern ORM with PostgreSQL support

# Screenshots

#### Dashboard
![](_screenshots/module-zero-core-template-ui-home.png)

#### User Management
![](_screenshots/module-zero-core-template-ui-user-create-modal.png)

#### Login Page
![](_screenshots/module-zero-core-template-ui-login.png)

## Development

### Project Structure

```
SmartBizApp/
├── Backend/AspNetCore/SimpleApi/    # ASP.NET Core 8.0 Web API
│   ├── Controllers/                 # API endpoints
│   ├── Services/                    # Business logic and email services
│   ├── Models/                      # Entity models
│   ├── DTOs/                        # Data transfer objects
│   ├── Data/                        # Entity Framework DbContext
│   └── Migrations/                  # Database migrations
├── FrontEnd/                        # Angular 19.2.1 Application
│   ├── src/app/components/          # Angular components
│   ├── src/app/services/            # HTTP services and business logic
│   ├── src/app/guards/              # Route guards for RBAC
│   ├── src/app/models/              # TypeScript interfaces
│   └── src/assets/                  # Static assets and themes
├── _screenshots/                    # Application screenshots
├── RBAC_IMPLEMENTATION.md           # Role-based access control documentation
├── NOTIFICATION_SYSTEM.md           # Email notification system documentation
├── DOCKER_README.md                 # Docker deployment guide
├── docker-compose.yml               # Multi-container deployment
└── README.md                        # This file
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

**Linting and Code Quality:**
```bash
# Frontend linting
cd FrontEnd
npm run lint

# Format code with Prettier
npm run prettier
```

**Testing:**
```bash
# Frontend unit tests
cd FrontEnd
npm test

# Backend tests (if available)
cd Backend/AspNetCore/SimpleApi
dotnet test
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

- **[RBAC Implementation Guide](RBAC_IMPLEMENTATION.md)**: Detailed documentation on role-based access control
- **[Email Notification System](NOTIFICATION_SYSTEM.md)**: Configuration and usage of the notification system  
- **[Docker Deployment Guide](DOCKER_README.md)**: Complete containerization and deployment instructions
- **[Company Setup Workflow](COMPANY_SETUP_WORKFLOW.md)**: Guide for company configuration and setup

## Technical Resources

- **API Documentation**: Available at `/swagger` endpoint when running the backend
- **Frontend Architecture**: Angular 19.2.1 with standalone components and modern routing
- **Database Schema**: Entity Framework migrations in `Backend/AspNetCore/SimpleApi/Migrations/`
- **Configuration**: Environment-specific settings in `appsettings.json` files

# License

[MIT](LICENSE).
