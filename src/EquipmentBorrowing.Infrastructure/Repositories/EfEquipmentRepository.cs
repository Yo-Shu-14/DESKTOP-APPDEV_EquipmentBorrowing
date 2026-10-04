using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task UpdateAsync(Equipment equipment)
    {
        _db.Equipment.Update(equipment);
        await _db.SaveChangesAsync();
    }
}