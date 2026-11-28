using System.Net;
using System.Text.Json;
using Xunit;

namespace EquipmentBorrowing.Tests.Lab6;

public sealed class Lab6OpenApiTests
{
    [Fact(DisplayName = "API contract coverage through OpenAPI")]
    public async Task OpenApi_documents_required_routes_and_methods()
    {
        // OpenAPI generation does not query the database. Use an unreachable
        // placeholder so this contract test cannot touch a real database.
        using var factory = new Lab6ApiFactory(
            "Host=127.0.0.1;Port=1;Database=lab6_openapi_only;Username=unused;Password=unused");
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var paths = document.RootElement.GetProperty("paths");

        var required = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["/api/v1/equipment"] = ["get", "post"],
            ["/api/v1/equipment/{id}"] = ["get", "put"],
            ["/api/v1/equipment/{id}/borrowings"] = ["get"],
            ["/api/v1/students"] = ["get", "post"],
            ["/api/v1/students/{id}"] = ["get", "put"],
            ["/api/v1/students/{id}/eligibility"] = ["patch"],
            ["/api/v1/students/{id}/borrowings"] = ["get"],
            ["/api/v1/borrowings"] = ["get", "post"],
            ["/api/v1/borrowings/{id}"] = ["get"],
            ["/api/v1/borrowings/{id}/return"] = ["post"],
            ["/api/v1/checkout-requests"] = ["post"]
        };

        foreach (var (path, methods) in required)
        {
            Assert.True(paths.TryGetProperty(path, out var pathItem), $"OpenAPI is missing {path}");
            foreach (var method in methods)
            {
                Assert.True(pathItem.TryGetProperty(method, out _), $"OpenAPI is missing {method.ToUpperInvariant()} {path}");
            }
        }
    }
}
