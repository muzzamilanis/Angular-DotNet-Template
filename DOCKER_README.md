# Docker Deployment Guide

This guide explains how to deploy the ASP.NET Core API using Docker.

## Files Created

- `Dockerfile` - Multi-stage Docker build file for the API
- `docker-compose.yml` - Complete deployment with SQL Server
- `appsettings.Production.json` - Production configuration
- `.dockerignore` - Optimizes Docker build context

## Quick Start

### Option 1: Using Docker Compose (Recommended)

1. Build and run the complete stack (API + SQL Server):
```bash
docker-compose up --build
```

2. The API will be available at:
   - HTTP: http://localhost:8080
   - HTTPS: http://localhost:8081

3. SQL Server will be available at `localhost:1433`

### Option 2: Docker Build Only

1. Build the API image:
```bash
cd Backend/AspNetCore/SimpleApi
docker build -t simpleapi .
```

2. Run with external database:
```bash
docker run -p 8080:8080 -p 8081:8081 \
  -e ConnectionStrings__DefaultConnection="Your-Database-Connection-String" \
  -e Jwt__Key="your-jwt-secret-key" \
  simpleapi
```

## Configuration

### Environment Variables

The application supports the following environment variables:

#### Database
- `ConnectionStrings__DefaultConnection` - Database connection string

#### JWT Authentication
- `Jwt__Key` - JWT signing key (minimum 256 bits)
- `Jwt__Issuer` - JWT issuer (default: SimpleApi)
- `Jwt__Audience` - JWT audience (default: SimpleApi)

#### Email Settings (Optional)
- `Email__SmtpHost` - SMTP server hostname
- `Email__SmtpPort` - SMTP server port
- `Email__Username` - SMTP username
- `Email__Password` - SMTP password
- `Email__FromEmail` - From email address
- `Email__FromName` - From name

### Example with custom configuration:

```bash
docker run -p 8080:8080 -p 8081:8081 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Server=your-sql-server;Database=BusinessDB;User Id=sa;Password=YourPassword123;TrustServerCertificate=true;" \
  -e Jwt__Key="your-very-long-secret-key-for-jwt-token-signing-must-be-256-bits" \
  -e Email__SmtpHost="smtp.your-email-provider.com" \
  -e Email__SmtpPort="587" \
  -e Email__Username="your-email@domain.com" \
  -e Email__Password="your-email-password" \
  simpleapi
```

## Database Setup

The application will automatically create the database and run migrations on startup when using Entity Framework's `EnsureCreated()` method.

For production deployments, consider:
1. Running migrations manually
2. Using a proper database migration strategy
3. Setting up database backups

## Security Considerations

1. **JWT Secret**: Use a strong, randomly generated key of at least 256 bits
2. **Database Password**: Use strong passwords for SQL Server
3. **Email Credentials**: Store email credentials securely
4. **HTTPS**: Configure proper SSL certificates for production

## Volumes

The docker-compose setup creates a persistent volume for SQL Server data:
- `sqlserver_data` - Stores SQL Server database files

## Network

The docker-compose setup creates a bridge network (`app-network`) for internal communication between services.

## Troubleshooting

### Common Issues

1. **Port conflicts**: Change port mappings if 8080/8081 are in use
2. **Database connection**: Ensure connection string is correct
3. **JWT errors**: Verify JWT key is properly set and long enough

### Logs

View application logs:
```bash
docker-compose logs api
```

View SQL Server logs:
```bash
docker-compose logs sqlserver
```

## Production Considerations

1. Use proper SSL certificates
2. Configure reverse proxy (nginx/traefik)
3. Set up monitoring and logging
4. Use external database for production
5. Implement proper backup strategies
6. Use secrets management for sensitive data