# SimpleApi Functional Tests

This project contains functional (integration) tests for the SimpleApi backend application.

## Overview

The functional test framework uses:
- **xUnit** - Test framework
- **FluentAssertions** - Fluent assertion library for more readable tests
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing infrastructure for ASP.NET Core
- **SQLite In-Memory Database** - Fast, isolated test database

## Running Tests

### Run all tests
```bash
cd Backend/AspNetCore/SimpleApi.FunctionalTests
dotnet test
```

### Run with detailed output
```bash
dotnet test --verbosity normal
```

### Run specific test
```bash
dotnet test --filter "FullyQualifiedName~HealthControllerTests"
```

## Test Structure

```
SimpleApi.FunctionalTests/
├── Controllers/           # Controller endpoint tests
│   ├── HealthControllerTests.cs
│   └── ApiBasicTests.cs
├── Helpers/              # Test helper utilities
│   └── AuthHelper.cs
├── CustomWebApplicationFactory.cs  # Test server configuration
└── README.md
```

## Writing New Tests

### Basic Test Example

```csharp
public class MyControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MyControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task MyEndpoint_ReturnsSuccess()
    {
        // Arrange
        var request = new { data = "test" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/MyController", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

### Testing with Authentication

```csharp
[Fact]
public async Task AuthenticatedEndpoint_WithToken_ReturnsSuccess()
{
    // Arrange
    var token = await AuthHelper.GetAuthTokenAsync(_client);
    AuthHelper.AddAuthToken(_client, token);

    // Act
    var response = await _client.GetAsync("/api/Customers");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

## Test Database

The test framework uses an in-memory SQLite database that is:
- Created fresh for each test run
- Automatically seeded with required schema
- Isolated between tests
- Disposed after tests complete

## CI/CD Integration

These tests are designed to run in CI/CD pipelines:
- No external dependencies required
- Fast execution (in-memory database)
- Deterministic results
- Easy to integrate with GitHub Actions, Azure DevOps, etc.

## Current Test Coverage

- ✅ Health check endpoints
- ✅ API accessibility
- ✅ Authentication requirements
- ✅ Database connectivity
- ✅ Unauthorized access protection

## Future Enhancements

- Add tests for CRUD operations on all controllers
- Add tests for business logic validation
- Add tests for error handling scenarios
- Add performance tests
- Add tests for background services
