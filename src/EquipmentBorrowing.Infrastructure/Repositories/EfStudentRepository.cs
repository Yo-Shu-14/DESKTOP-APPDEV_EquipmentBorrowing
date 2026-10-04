using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class EfStudentRepository : IStudentRepository
{
    private readonly EquipmentBorrowingDbContext _db;

    public EfStudentRepository(EquipmentBorrowingDbContext db)
    {
        _db = db;
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _db.Students
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IReadOnlyList<Student>> GetAllAsync()
    {
        return await _db.Students
            .AsNoTracking()
            .ToListAsync();
    }
}