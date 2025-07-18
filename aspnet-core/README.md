# Business API - Modern .NET Core Web API

A modern, clean architecture .NET Core Web API following industry best practices and design patterns.

## Architecture Overview

This API follows the **Clean Architecture** pattern with clear separation of concerns across multiple layers:

```
BusinessApi/
├── BusinessApi.Domain/         # Domain Layer (Entities, Interfaces)
├── BusinessApi.Application/    # Application Layer (Services, DTOs, Mappings)
├── BusinessApi.Infrastructure/ # Infrastructure Layer (Data Access, Repositories)
└── BusinessApi.Api/           # Presentation Layer (Controllers, Configuration)
```

### Layer Responsibilities

#### 1. Domain Layer (`BusinessApi.Domain`)
- **Purpose**: Contains the core business entities and domain interfaces
- **Dependencies**: None (pure business logic)
- **Contents**:
  - `Entities/` - Domain entities (Customer, Product, SaleOrder, SaleOrderItem)
  - `Interfaces/` - Repository interfaces (ICustomerRepository, IProductRepository, etc.)

#### 2. Application Layer (`BusinessApi.Application`)
- **Purpose**: Contains application services, business logic, and DTOs
- **Dependencies**: Domain layer only
- **Contents**:
  - `Services/` - Application services (CustomerService, ProductService, etc.)
  - `DTOs/` - Data transfer objects for API communication
  - `Mappings/` - AutoMapper profiles for entity-DTO mapping

#### 3. Infrastructure Layer (`BusinessApi.Infrastructure`)
- **Purpose**: Contains data access implementations and external dependencies
- **Dependencies**: Domain and Application layers
- **Contents**:
  - `Data/` - Entity Framework DbContext
  - `Repositories/` - Repository implementations

#### 4. API Layer (`BusinessApi.Api`)
- **Purpose**: Contains controllers, middleware, and application configuration
- **Dependencies**: All other layers
- **Contents**:
  - `Controllers/` - API controllers
  - `Program.cs` - Application configuration and DI setup

## Key Features

### 🏗️ Modern Architecture Patterns
- **Clean Architecture** with clear layer separation
- **Repository Pattern** for data access abstraction
- **Dependency Injection** for loose coupling
- **CQRS-style** service layer separation

### 🔧 Technology Stack
- **.NET 8.0** - Latest .NET version
- **Entity Framework Core** with SQLite (easily configurable for SQL Server)
- **AutoMapper** for object mapping
- **JWT Authentication** for secure API access
- **Swagger/OpenAPI** for API documentation

### 📋 API Features
- **Customer Management** - Full CRUD operations
- **Product Management** - Full CRUD operations  
- **Sale Order Management** - Complex entity management with line items
- **JWT Authentication** - Secure token-based authentication
- **Pagination** - Built-in pagination support
- **CORS** - Cross-origin request support

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### Running the Application

1. **Clone and navigate to the project**:
   ```bash
   cd aspnet-core
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore BusinessApi.sln
   ```

3. **Build the solution**:
   ```bash
   dotnet build BusinessApi.sln
   ```

4. **Run the API**:
   ```bash
   cd src/BusinessApi.Api
   dotnet run
   ```

5. **Access the API**:
   - API Base URL: `http://localhost:5101`
   - Swagger UI: `http://localhost:5101/swagger`
## API Endpoints

### Authentication
```http
POST /api/tokenauth/authenticate
Content-Type: application/json

{
  "username": "admin",
  "password": "123qwe"
}
```

### Customer Management
```http
GET    /api/customers           # Get all customers (paginated)
GET    /api/customers/active    # Get active customers only
GET    /api/customers/{id}      # Get customer by ID
POST   /api/customers           # Create customer
PUT    /api/customers/{id}      # Update customer
DELETE /api/customers/{id}      # Delete customer
```

### Product Management
```http
GET    /api/products            # Get all products (paginated)
GET    /api/products/active     # Get active products only
GET    /api/products/{id}       # Get product by ID
POST   /api/products            # Create product
PUT    /api/products/{id}       # Update product
DELETE /api/products/{id}       # Delete product
```

### Sale Order Management
```http
GET    /api/saleorders          # Get all sale orders (paginated)
GET    /api/saleorders/{id}     # Get sale order by ID (with line items)
POST   /api/saleorders          # Create sale order with line items
PUT    /api/saleorders/{id}     # Update sale order with line items
DELETE /api/saleorders/{id}     # Delete sale order
```

## Sample Requests

### Create Customer
```json
POST /api/customers
{
  "name": "John Doe",
  "address": "123 Main St",
  "phone": "555-1234",
  "email": "john@example.com"
}
```

### Create Sale Order
```json
POST /api/saleorders
{
  "orderNumber": "SO001",
  "orderDate": "2024-01-18T10:00:00",
  "customerName": "John Doe",
  "customerAddress": "123 Main St",
  "items": [
    {
      "productId": 1,
      "quantity": 2,
      "price": 10.5
    },
    {
      "productId": 2,
      "quantity": 1,
      "price": 25.0
    }
  ]
}
```

## Configuration

### Database
The API uses SQLite by default for development. To switch to SQL Server:

1. Update `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BusinessApiDb;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

2. Update `Program.cs`:
   ```csharp
   builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
   ```

3. Update package references to use `Microsoft.EntityFrameworkCore.SqlServer`.

### JWT Settings
Configure JWT settings in `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "your-secret-key-must-be-at-least-256-bits-long-for-security",
    "Issuer": "BusinessApi",
    "Audience": "BusinessApi"
  }
}
```

## Development

### Adding New Features

1. **Add Domain Entity** in `BusinessApi.Domain/Entities/`
2. **Add Repository Interface** in `BusinessApi.Domain/Interfaces/`
3. **Add DTOs** in `BusinessApi.Application/DTOs/`
4. **Add Application Service** in `BusinessApi.Application/Services/`
5. **Add Repository Implementation** in `BusinessApi.Infrastructure/Repositories/`
6. **Add Controller** in `BusinessApi.Api/Controllers/`
7. **Register Dependencies** in `Program.cs`

### Testing
Use the included `BusinessApi.http` file with Visual Studio Code REST Client extension to test API endpoints.

## Migration from SimpleApi

This project has been completely restructured from a simple folder-based API to a modern n-tier architecture:

- **Before**: Single project with basic folder structure
- **After**: Four-layer architecture with clear separation of concerns

All existing functionality has been preserved while significantly improving:
- Code maintainability
- Testability
- Scalability
- Industry standard practices

## Benefits of This Architecture

✅ **Separation of Concerns** - Each layer has a single responsibility  
✅ **Testability** - Easy to unit test each layer independently  
✅ **Maintainability** - Changes in one layer don't affect others  
✅ **Scalability** - Easy to extend and add new features  
✅ **Industry Standard** - Follows .NET community best practices  
✅ **Clean Code** - Readable and well-organized codebase  

## Legacy Code

The original SimpleApi code has been moved to `legacy/` folder for reference purposes.