using EquipmentBorrowing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;

namespace EquipmentBorrowing.Tests.Lab6;

/// <summary>
/// Owns reset/setup for the disposable Lab 6 PostgreSQL database.
/// The connection must be supplied by the local process and is never committed.
/// </summary>
public sealed class Lab6DatabaseFixture : IAsyncLifetime
{
    private const string ConnectionVariable = "LAB6_TEST_CONNECTION";

    public string ConnectionString =>
        Environment.GetEnvironmentVariable(ConnectionVariable)
        ?? throw new InvalidOperationException(
            $"Set {ConnectionVariable} to an isolated disposable PostgreSQL database before running Lab 6 tests.");

    public DateTime D => new(2026, 11, 27);

    public async ValueTask InitializeAsync()
    {
        await using var db = CreateContext();
        await db.Database.MigrateAsync();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public EquipmentBorrowingDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<EquipmentBorrowingDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new EquipmentBorrowingDbContext(options);
    }

    public async Task ExecuteResetScriptAsync()
    {
        var scriptPath = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..", "docs", "seed-test-data.sql");

        var script = await File.ReadAllTextAsync(Path.GetFullPath(scriptPath));
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(script, connection);
        await command.ExecuteNonQueryAsync();
    }
}
