using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EquipmentBorrowing.Tests.Lab6;

public sealed class Lab6ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public Lab6ApiFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("EQUIPMENT_BORROWING_CONNECTION", _connectionString);
        builder.UseSetting("ConnectionStrings:DefaultConnection", _connectionString);
    }
}
