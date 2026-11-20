using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EquipmentBorrowing.Infrastructure.Data;

public class EquipmentBorrowingDbContext : DbContext
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<Borrowing> Borrowings => Set<Borrowing>();

    public EquipmentBorrowingDbContext(
        DbContextOptions<EquipmentBorrowingDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(EquipmentBorrowingDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}


public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("EQUIPMENT_BORROWING_CONNECTION");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=localhost;Port=1403;Database=equipment_borrowing;Username=postgres;Password=databaseADMIN";
        }

        services.AddDbContext<EquipmentBorrowingDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}
