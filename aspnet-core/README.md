# Simple .NET Core Web API

This is a simplified .NET Core Web API project that provides the core functionality for Customer, Product, and Sale Order management with basic authentication.

## Project Structure

```
src/
  SimpleApi/
    Controllers/        # API Controllers
    Services/           # Business logic services
    Models/             # Entity models
    DTOs/               # Data Transfer Objects
    Data/               # Entity Framework DbContext
    Program.cs          # Application entry point
    appsettings.json    # Configuration
```

## Features

- **Customer Management**: CRUD operations for customers
- **Product Management**: CRUD operations for products  
- **Sale Order Management**: CRUD operations for sale orders with line items
- **JWT Authentication**: Simple token-based authentication
- **SQLite Database**: Lightweight database for development
- **AutoMapper**: Object-to-object mapping
- **Entity Framework Core**: ORM for data access
- **Swagger**: API documentation

## Getting Started

### Prerequisites
- .NET 8.0 SDK

### Running the Application

1. Navigate to the SimpleApi directory:
   ```bash
   cd src/SimpleApi
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. The API will be available at `http://localhost:5000`
4. Swagger documentation will be available at `http://localhost:5000/swagger`

### Authentication

Use the following credentials for authentication:
- **Username**: admin
- **Password**: 123qwe

### API Endpoints

#### Authentication
- `POST /api/tokenauth/authenticate` - Login and get JWT token
- `GET /api/session/GetCurrentLoginInformations` - Get current user info

#### Customers
- `GET /api/customers` - Get all customers (paginated)
- `GET /api/customers/active` - Get active customers
- `GET /api/customers/{id}` - Get customer by ID
- `POST /api/customers` - Create new customer
- `PUT /api/customers/{id}` - Update customer
- `DELETE /api/customers/{id}` - Delete customer

#### Products
- `GET /api/products` - Get all products (paginated)
- `GET /api/products/active` - Get active products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

#### Sale Orders
- `GET /api/saleorders` - Get all sale orders (paginated)
- `GET /api/saleorders/{id}` - Get sale order by ID
- `POST /api/saleorders` - Create new sale order
- `PUT /api/saleorders/{id}` - Update sale order
- `DELETE /api/saleorders/{id}` - Delete sale order

### Example Usage

#### Login
```bash
curl -X POST "http://localhost:5000/api/tokenauth/authenticate" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"123qwe"}'
```

#### Get Customers
```bash
curl -X GET "http://localhost:5000/api/customers"
```

#### Create Sale Order
```bash
curl -X POST "http://localhost:5000/api/saleorders" \
  -H "Content-Type: application/json" \
  -d '{
    "orderNumber":"SO001",
    "orderDate":"2024-01-18T10:00:00",
    "customerName":"John Doe",
    "customerAddress":"123 Main St",
    "items":[
      {"productId":1,"quantity":2,"price":10.5,"date":"2024-01-18T10:00:00"},
      {"productId":2,"quantity":1,"price":25.0,"date":"2024-01-18T10:00:00"}
    ]
  }'
```

## Database

The application uses SQLite database (`simpleapi.db`) which is automatically created on first run with sample data:

- 2 sample customers
- 3 sample products

## Configuration

Key configuration settings in `appsettings.json`:

- `ConnectionStrings:DefaultConnection` - Database connection string
- `Jwt:Key` - JWT signing key
- `Jwt:Issuer` - JWT issuer
- `Jwt:Audience` - JWT audience

## Legacy Code

The previous complex ABP framework-based code has been moved to the `legacy/` folder for reference but is no longer used.