# Backend Testing Guide

## Overview

The backend has a comprehensive functional test framework that validates API endpoints, database operations, and business logic. All tests use an in-memory SQLite database for fast, isolated testing.

## Quick Start

### Running Tests

**From the test project directory:**
```bash
cd Backend/AspNetCore/SimpleApi.FunctionalTests
dotnet test
```

**From the solution directory:**
```bash
cd Backend/AspNetCore/SimpleApi
dotnet test SimpleApi.sln
```

**Using the convenience scripts:**
```bash
# Linux/Mac
cd Backend/AspNetCore
./run-tests.sh

# Windows
cd Backend\AspNetCore
run-tests.bat
```

## Test Framework Stack

- **xUnit** - Test framework with excellent .NET Core integration
- **FluentAssertions** - Readable assertion library
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing for ASP.NET Core
- **SQLite In-Memory** - Fast, isolated test database
- **WebApplicationFactory** - Hosts the API in-memory for testing

## Test Structure

```
SimpleApi.FunctionalTests/
├── Controllers/                    # Controller endpoint tests
│   ├── HealthControllerTests.cs   # Health check endpoints
│   ├── ApiBasicTests.cs           # Basic API functionality
│   └── DatabaseOperationsTests.cs # Database operations
├── Helpers/                        # Test utilities
│   └── AuthHelper.cs              # Authentication helpers
├── CustomWebApplicationFactory.cs # Test server setup
├── README.md                      # Detailed test documentation
└── SimpleApi.FunctionalTests.csproj
```

## Current Test Coverage

### Health Endpoints (3 tests)
- ✅ `/health` - Root health endpoint
- ✅ `/api/Health/ping` - API health check
- ✅ `/api/Health/dbping` - Database connectivity

### API Authorization (5 tests)
- ✅ Customers endpoint requires authentication
- ✅ Products endpoint requires authentication
- ✅ Projects endpoint requires authentication
- ✅ SaleOrders endpoint requires authentication
- ✅ Invalid endpoints return 404

### Database Operations (5 tests)
- ✅ Database connection validation
- ✅ Schema integrity checks
- ✅ Insert and retrieve operations
- ✅ Multiple table support
- ✅ Health check integration

**Total: 13 tests, all passing ✓**

## Writing New Tests

### Basic Test Pattern

```csharp
public class MyControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MyControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetEndpoint_ReturnsSuccess()
    {
        // Arrange
        // ... setup test data

        // Act
        var response = await _client.GetAsync("/api/MyController");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

### Testing with Database Access

```csharp
[Fact]
public async Task CanAccessDatabase()
{
    // Arrange
    using var scope = _factory.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Act
    var canConnect = await dbContext.Database.CanConnectAsync();

    // Assert
    canConnect.Should().BeTrue();
}
```

### Testing with Authentication

```csharp
[Fact]
public async Task AuthenticatedEndpoint_WithValidToken_ReturnsSuccess()
{
    // Arrange
    var token = await AuthHelper.GetAuthTokenAsync(_client, "admin", "password");
    AuthHelper.AddAuthToken(_client, token);

    // Act
    var response = await _client.GetAsync("/api/SecureEndpoint");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

## Test Isolation

Each test run:
1. Creates a fresh in-memory SQLite database
2. Applies all entity framework migrations
3. Runs tests in isolation
4. Disposes the database after completion

This ensures:
- No test pollution between runs
- Consistent, repeatable results
- Fast execution (no network I/O)
- No external dependencies

## CI/CD Integration

The tests are integrated into the GitHub Actions workflow (`backend-ci.yml`):

```yaml
- name: Run functional tests
  run: |
    cd SimpleApi
    dotnet test SimpleApi.sln --no-build --configuration Release --verbosity normal
```

Tests run automatically on:
- Push to `main`, `master`, or `develop` branches
- Pull requests targeting these branches
- Manual workflow dispatch

## Best Practices

1. **Keep tests focused** - One assertion per test when possible
2. **Use descriptive names** - `MethodName_StateUnderTest_ExpectedBehavior`
3. **Arrange-Act-Assert** - Follow the AAA pattern
4. **Test behavior, not implementation** - Focus on API contracts
5. **Use FluentAssertions** - More readable than basic Assert
6. **Clean up resources** - Use `using` statements for disposables
7. **Avoid test interdependencies** - Each test should run independently

## Common Test Scenarios

### Testing GET endpoints
```csharp
var response = await _client.GetAsync("/api/Resource");
response.StatusCode.Should().Be(HttpStatusCode.OK);
var content = await response.Content.ReadFromJsonAsync<ResourceDto>();
content.Should().NotBeNull();
```

### Testing POST endpoints
```csharp
var request = new CreateResourceDto { Name = "Test" };
var response = await _client.PostAsJsonAsync("/api/Resource", request);
response.StatusCode.Should().Be(HttpStatusCode.Created);
```

### Testing error handling
```csharp
var response = await _client.GetAsync("/api/Resource/999");
response.StatusCode.Should().Be(HttpStatusCode.NotFound);
```

## Troubleshooting

### Tests failing with database errors
- Ensure `appsettings.Testing.json` exists
- Check that `Program.cs` skips DB config in Testing environment
- Verify SQLite package is installed

### Tests failing inconsistently
- Check for shared state between tests
- Ensure proper disposal of resources
- Use fresh `HttpClient` from factory

### Performance issues
- SQLite in-memory should be fast (<5s for all tests)
- If slow, check for unnecessary database operations
- Consider parallelization settings in xUnit

## Future Enhancements

Planned test additions:
- [ ] CRUD tests for all controllers
- [ ] Business logic validation tests
- [ ] Error handling scenarios
- [ ] Authorization rule tests
- [ ] Performance/load tests
- [ ] Background service tests
- [ ] Email notification tests

## Resources

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [ASP.NET Core Testing](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests)
- [Test Project README](SimpleApi.FunctionalTests/README.md)
