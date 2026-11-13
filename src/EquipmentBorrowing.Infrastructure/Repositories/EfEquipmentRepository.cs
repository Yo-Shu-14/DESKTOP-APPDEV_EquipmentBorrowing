using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class EfEquipmentRepository : IEquipmentRepository
{
    private readonly EquipmentBorrowingDbContext _db;

    public EfEquipmentRepository(EquipmentBorrowingDbContext db)
    {
        _db = db;
    }

    public async Task<Equipment?> GetByIdAsync(Guid id)
    {
        return await _db.Equipment
            .FirstOrDefaultAsync(e => e.EquipmentId == id);
    }

    public async Task<IReadOnlyList<Equipment>> GetAllAsync()
    {
        return await _db.Equipment
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Equipment>> GetAvailableAsync()
    {
        return await _db.Equipment
        .Where(e => e.IsAvailable)
        .AsNoTracking()
        .ToListAsync();

    }
    public async Task<Equipment?> GetByIdentifierAsync(string identifier)
    {
        string normalizedIdentifier = identifier.Trim().ToUpperInvariant();

        return await _db.Equipment
            .FirstOrDefaultAsync(e =>
                e.EquipmentIdentifier == normalizedIdentifier);
    }

    public async Task AddAsync(Equipment equipment)
    {
        try
        {
            await _db.Equipment.AddAsync(equipment);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException postgresException
                  && postgresException.SqlState == "23505"
                  && postgresException.ConstraintName == "IX_Equipment_EquipmentIdentifier")
        {
            throw new InvalidOperationException(
                "Equipment with this identifier already exists.");
        }
    }
    public async Task UpdateAsync(Equipment equipment)
    {
        try
        {
            _db.Equipment.Update(equipment);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException postgresException
                  && postgresException.SqlState == "23505"
                  && postgresException.ConstraintName == "IX_Equipment_EquipmentIdentifier")
        {
            throw new InvalidOperationException(
                "Equipment with this identifier already exists.");
        }
    }
}