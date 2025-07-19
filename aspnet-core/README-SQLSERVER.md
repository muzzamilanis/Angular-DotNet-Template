# SQL Server Configuration

This project has been configured to use SQL Server instead of SQLite. The following changes have been made:

## Changes Made

1. **Package References Updated** (`BusinessApi.Infrastructure.csproj`):
   - Replaced `Microsoft.EntityFrameworkCore.Sqlite` with `Microsoft.EntityFrameworkCore.SqlServer`

2. **Connection Strings Updated**:
   - `appsettings.json`: Uses LocalDB for production
   - `appsettings.Development.json`: Uses LocalDB for development with separate database name

3. **DbContext Configuration Updated** (`Program.cs`):
   - Changed from `UseSqlite()` to `UseSqlServer()`

## Connection String Options

### Default (LocalDB)
```json
"Server=(localdb)\\mssqllocaldb;Database=BusinessApiDb;Trusted_Connection=true;MultipleActiveResultSets=true"
```

### SQL Server Express
```json
"Server=.\\SQLEXPRESS;Database=BusinessApiDb;Trusted_Connection=true;MultipleActiveResultSets=true"
```

### SQL Server with Authentication
```json
"Server=localhost;Database=BusinessApiDb;User Id=sa;Password=YourPassword;TrustServerCertificate=true;MultipleActiveResultSets=true"
```

### Azure SQL Database
```json
"Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=BusinessApiDb;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

## Prerequisites

- SQL Server LocalDB, SQL Server Express, or SQL Server installed
- Or Azure SQL Database available

## Running the Application

1. Ensure SQL Server is installed and running
2. Update the connection string in `appsettings.json` if needed
3. Run the application: `dotnet run`

The application will automatically create the database and tables on first run using `EnsureCreated()`.

## Migration to Entity Framework Migrations (Optional)

If you want to use Entity Framework migrations instead of `EnsureCreated()`:

1. Remove the `EnsureCreated()` call from `Program.cs`
2. Create initial migration: `dotnet ef migrations add InitialCreate`
3. Update database: `dotnet ef database update`

## Database Schema

The application creates the following tables:
- Customers
- Products  
- SaleOrders
- SaleOrderItems

With appropriate relationships and seed data as defined in `ApplicationDbContext.cs`.