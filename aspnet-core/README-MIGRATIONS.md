# Entity Framework Migrations

This project now supports Entity Framework migrations for proper database schema management.

## Prerequisites

Ensure you have the EF Core tools installed globally:
```bash
dotnet tool install --global dotnet-ef
```

## Common Migration Commands

All commands should be run from the `aspnet-core/src/BusinessApi.Infrastructure` directory:

### Create a new migration
```bash
dotnet ef migrations add <MigrationName> --project . --startup-project ../BusinessApi.Api
```

Example:
```bash
dotnet ef migrations add AddNewColumn --project . --startup-project ../BusinessApi.Api
```

### Apply migrations to database
```bash
dotnet ef database update --project . --startup-project ../BusinessApi.Api
```

### List all migrations
```bash
dotnet ef migrations list --project . --startup-project ../BusinessApi.Api
```

### Remove the last migration (if not applied to database)
```bash
dotnet ef migrations remove --project . --startup-project ../BusinessApi.Api
```

### Generate SQL script for migrations
```bash
dotnet ef migrations script --project . --startup-project ../BusinessApi.Api
```

## Database Configuration

The project is configured to use SQL Server with LocalDB:
- **Development**: `Server=(localdb)\\mssqllocaldb;Database=BusinessApiDb_Dev;Trusted_Connection=true;MultipleActiveResultSets=true`
- **Production**: `Server=(localdb)\\mssqllocaldb;Database=BusinessApiDb;Trusted_Connection=true;MultipleActiveResultSets=true`

The database is automatically created when the application starts and migrations are applied.

## Automatic Migration on Startup

The application is configured to automatically apply pending migrations when it starts up. This is done in `Program.cs`:

```csharp
// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}
```

## Migration History

- **InitialCreate** (20250719074230): Initial database schema with Customer, Product, SaleOrder, and SaleOrderItem tables including seed data for SQL Server.

## Troubleshooting

If you encounter issues:

1. **Build errors**: Ensure the solution builds successfully before running migration commands
2. **Connection issues**: Verify the connection string in `appsettings.json` and `appsettings.Development.json`
3. **LocalDB issues**: Ensure SQL Server LocalDB is installed and running
4. **Migration conflicts**: Use `dotnet ef migrations remove` to undo pending migrations before creating new ones

