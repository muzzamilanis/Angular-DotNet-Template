using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleApi.Data;
using SimpleApi.Models;

namespace SimpleApi.FunctionalTests.Controllers;

public class DatabaseOperationsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public DatabaseOperationsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Database_CanBeAccessed()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var canConnect = await dbContext.Database.CanConnectAsync();

        // Assert
        canConnect.Should().BeTrue();
    }

    [Fact]
    public async Task Database_HasCorrectSchema()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act & Assert - Just checking that tables exist by querying them
        var usersCount = dbContext.Users.Count();
        var companiesCount = dbContext.Companies.Count();
        var customersCount = dbContext.Customers.Count();
        var productsCount = dbContext.Products.Count();
        var projectsCount = dbContext.Projects.Count();

        // All counts should be 0 or more (meaning tables exist)
        usersCount.Should().BeGreaterThanOrEqualTo(0);
        companiesCount.Should().BeGreaterThanOrEqualTo(0);
        customersCount.Should().BeGreaterThanOrEqualTo(0);
        productsCount.Should().BeGreaterThanOrEqualTo(0);
        projectsCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Database_CanInsertAndRetrieveData()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // First create a BusinessNature if it doesn't exist
        var businessNature = dbContext.BusinessNatures.FirstOrDefault();
        if (businessNature == null)
        {
            businessNature = new BusinessNature
            {
                Name = "TestBusiness",
                DisplayName = "Test Business",
                Description = "Test business nature",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            dbContext.BusinessNatures.Add(businessNature);
            await dbContext.SaveChangesAsync();
        }

        var testCompany = new Company
        {
            Name = "Test Company",
            BusinessNatureId = businessNature.Id,
            Address = "123 Test St",
            Phone = "555-0100",
            Email = "test@company.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        dbContext.Companies.Add(testCompany);
        await dbContext.SaveChangesAsync();

        var retrievedCompany = await dbContext.Companies.FindAsync(testCompany.Id);

        // Assert
        retrievedCompany.Should().NotBeNull();
        retrievedCompany!.Name.Should().Be("Test Company");
        retrievedCompany.BusinessNatureId.Should().Be(businessNature.Id);
        retrievedCompany.Email.Should().Be("test@company.com");
    }

    [Fact]
    public async Task HealthEndpoint_ChecksDatabase_Successfully()
    {
        // Act
        var response = await _client.GetAsync("/api/Health/dbping");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
        content.Should().Contain("Healthy");
    }
}
