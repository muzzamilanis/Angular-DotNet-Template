# SmartBizApp

SmartBizApp is a modern business management application built with **ASP.NET Core** backend and **Angular** frontend, designed to streamline business operations and enhance productivity.

## Features

- **User Management**: Comprehensive user administration with role-based access control
- **Dashboard**: Real-time business insights and analytics
- **Company Setup**: Easy configuration and setup workflows
- **Master Data Management**: Manage products, customers, and other core business entities
- **Transaction Processing**: Handle sales orders and business transactions
- **Modern UI**: Responsive design based on AdminLTE theme
- **Secure Authentication**: JWT-based authentication with advanced security features

## Technology Stack

- **Backend**: ASP.NET Core Web API
- **Frontend**: Angular with TypeScript
- **Database**: SQL Server with Entity Framework Core
- **UI Framework**: AdminLTE theme with Bootstrap
- **Authentication**: JWT tokens with refresh token support
- **Deployment**: Docker support for containerized deployment

# Default Login Credentials

After setting up the project and running the database migrations, you can log in using these default credentials:

**Username:** `admin`  
**Password:** `123qwe`

> ⚠️ **Security Warning**: These are default credentials for development purposes only. **Always change the default password immediately** after first login, especially in production environments.

The default admin user is created during database seeding with the email `admin@smartbizapp.com`.

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Node.js 18+ and npm
- SQL Server (LocalDB, Express, or full version)
- Docker (optional, for containerized deployment)

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
   dotnet ef database update
   dotnet run
   ```

3. **Frontend Setup**
   ```bash
   cd FrontEnd
   npm install
   npm start
   ```

4. **Access the Application**
   - Frontend: http://localhost:4200
   - Backend API: http://localhost:5000

#### Option 2: Docker Deployment

For easy deployment using Docker, see [DOCKER_README.md](DOCKER_README.md) for complete instructions.

**Quick Start with Docker:**
```bash
# Run the complete stack (API + SQL Server)
docker-compose up --build

# Access the API at:
# HTTP: http://localhost:8080
# HTTPS: http://localhost:8081
```

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
├── Backend/           # ASP.NET Core Web API
├── FrontEnd/         # Angular application
├── _screenshots/     # Application screenshots
└── README.md
```

### Building for Production

**Backend:**
```bash
cd Backend
dotnet publish -c Release
```

**Frontend:**
```bash
cd FrontEnd
npm run build
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## Support

For support and questions, please create an issue in the repository.

# License

[MIT](LICENSE).
