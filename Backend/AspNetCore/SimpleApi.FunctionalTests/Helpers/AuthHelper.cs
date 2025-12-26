using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SimpleApi.FunctionalTests.Helpers;

public static class AuthHelper
{
    public static async Task<string> GetAuthTokenAsync(HttpClient client, string username = "admin", string password = "admin123")
    {
        var loginDto = new
        {
            username = username,
            password = password
        };

        var response = await client.PostAsJsonAsync("/api/TokenAuth/authenticate", loginDto);
        
        if (!response.IsSuccessStatusCode)
        {
            return string.Empty;
        }

        var content = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(content);
        
        if (jsonDoc.RootElement.TryGetProperty("result", out var result) &&
            result.TryGetProperty("token", out var token))
        {
            return token.GetString() ?? string.Empty;
        }

        return string.Empty;
    }

    public static void AddAuthToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
