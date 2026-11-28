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

    public string? ConnectionString =>
        Environment.GetEnvironmentVariable(ConnectionVariable);

    public DateTime D => new(2026, 11, 27);

    public async ValueTask InitializeAsync()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            return;
        }

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
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            Assert.Skip($"Set {ConnectionVariable} to an isolated disposable PostgreSQL database.");
        }

        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Lab6", "seed-test-data.sql"),
            Path.Combine(Directory.GetCurrentDirectory(), "docs", "seed-test-data.sql"),
            Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory, "..", "..", "..", "..", "..", "docs", "seed-test-data.sql"))
        };
        var scriptPath = candidates.FirstOrDefault(File.Exists)
            ?? throw new FileNotFoundException(
                "Lab 6 seed script was not found. Expected docs/seed-test-data.sql.",
                string.Join("; ", candidates));

        var script = await File.ReadAllTextAsync(scriptPath);
        await using var connection = new NpgsqlConnection(ConnectionString!);
        await connection.OpenAsync();

        await using (var cleanup = new NpgsqlCommand(
            "TRUNCATE TABLE \"Borrowings\", \"Equipment\", \"Students\" RESTART IDENTITY CASCADE;",
            connection))
        {
            await cleanup.ExecuteNonQueryAsync();
        }

        await using var command = new NpgsqlCommand(script, connection);
        await command.ExecuteNonQueryAsync();

        await using var verification = new NpgsqlCommand(
            "SELECT (SELECT COUNT(*) FROM \"Students\"), (SELECT COUNT(*) FROM \"Equipment\"), (SELECT COUNT(*) FROM \"Borrowings\");",
            connection);
        await using var reader = await verification.ExecuteReaderAsync();
        if (!await reader.ReadAsync() || reader.GetInt64(0) != 4 || reader.GetInt64(1) != 7 || reader.GetInt64(2) != 4)
        {
            throw new InvalidOperationException("Lab 6 fixture reset did not produce the expected 4 students, 7 equipment items, and 4 borrowings.");
        }
    }
}
