
using EquipmentBorrowing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EquipmentBorrowing.Infrastructure.Data;

public class EquipmentBorrowingDbContextFactory
    : IDesignTimeDbContextFactory<EquipmentBorrowingDbContext>
{
    public EquipmentBorrowingDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("EQUIPMENT_BORROWING_CONNECTION");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=localhost;Port=1403;Database=equipment_borrowing;Username=postgres;Password=databaseADMIN";
        }

        var optionsBuilder =
            new DbContextOptionsBuilder<EquipmentBorrowingDbContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new EquipmentBorrowingDbContext(optionsBuilder.Options);
    }
}
