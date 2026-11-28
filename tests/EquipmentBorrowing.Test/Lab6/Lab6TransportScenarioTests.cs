using System.Net;
using EquipmentBorrowing.Desktop.ApiClient;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EquipmentBorrowing.Tests.Lab6;

[Collection("Lab 6 PostgreSQL database")]
public sealed class Lab6TransportScenarioTests
{
    private readonly Lab6DatabaseFixture _database;

    public Lab6TransportScenarioTests(Lab6DatabaseFixture database) => _database = database;

    [Fact(DisplayName = "TC6-E07a Unreachable API")]
    public async Task Client_reports_connection_failure_without_an_http_response()
    {
        using var http = new HttpClient(new AlwaysFailHandler())
        {
            BaseAddress = new Uri("http://localhost:5000/")
        };
        var client = new EquipmentBorrowingApiClient(http);

        var error = await Assert.ThrowsAsync<ApiConnectionException>(() => client.GetEquipmentAsync());
        Assert.Contains("Cannot reach the API server", error.Message);
    }

    [Fact(DisplayName = "TC6-E07b Received server error")]
    public async Task Client_distinguishes_received_server_error_from_transport_failure()
    {
        using var http = new HttpClient(new FixedResponseHandler(HttpStatusCode.InternalServerError))
        {
            BaseAddress = new Uri("http://localhost:5000/")
        };
        var client = new EquipmentBorrowingApiClient(http);

        await Assert.ThrowsAsync<ApiServerException>(() => client.GetEquipmentAsync());
    }

    [Fact(DisplayName = "TC6-E07c Lost commit acknowledgement")]
    public async Task Client_reports_unknown_outcome_without_retry_after_commit()
    {
        await _database.ExecuteResetScriptAsync();
        using var factory = new Lab6ApiFactory(_database.ConnectionString!);
        using var http = factory.CreateDefaultClient(new CommitThenDisconnectHandler());
        var client = new EquipmentBorrowingApiClient(http);

        await Assert.ThrowsAsync<ApiConnectionException>(() => client.CreateBorrowingAsync(new CreateBorrowingRequest(
            4,
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            _database.D.AddDays(2).ToString("yyyy-MM-dd"))));

        await using var db = _database.CreateContext();
        Assert.Equal(1, await db.Borrowings.CountAsync(b => b.StudentId == 4 && b.EquipmentId == Guid.Parse("22222222-2222-2222-2222-222222222222")));
    }

    private sealed class AlwaysFailHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            throw new HttpRequestException("simulated unreachable API");
    }

    private sealed class FixedResponseHandler(HttpStatusCode status) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(status)
            {
                Content = new StringContent("{\"code\":\"INTERNAL_SERVER_ERROR\",\"message\":\"safe error\"}")
            });
    }

    private sealed class CommitThenDisconnectHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using var response = await base.SendAsync(request, cancellationToken);
            throw new HttpRequestException("simulated lost commit acknowledgement");
        }
    }
}
