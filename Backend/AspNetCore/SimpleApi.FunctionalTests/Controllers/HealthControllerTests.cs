using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace SimpleApi.FunctionalTests.Controllers;

public class HealthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Ping_ReturnsHealthyStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/Health/ping");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<HealthResponse>();
        content.Should().NotBeNull();
        content!.Status.Should().Be("Healthy");
        content.Api.Should().Be("Running");
    }

    [Fact]
    public async Task DbPing_ReturnsHealthyStatus()
    {
        // Act
        var response = await _client.GetAsync("/api/Health/dbping");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<DbHealthResponse>();
        content.Should().NotBeNull();
        content!.Status.Should().Be("Healthy");
        content.Db.Should().Be("Connected");
    }

    [Fact]
    public async Task RootHealthEndpoint_ReturnsHealthyStatus()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<RootHealthResponse>();
        content.Should().NotBeNull();
        content!.Status.Should().Be("healthy");
        content.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    private record HealthResponse(string Status, string Api);
    private record DbHealthResponse(string Status, string Db);
    private record RootHealthResponse(string Status, DateTime Timestamp);
}
