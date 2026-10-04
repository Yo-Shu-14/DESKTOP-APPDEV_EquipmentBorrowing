using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class EfBorrowingRepository : IBorrowingRepository
{
    private readonly EquipmentBorrowingDbContext _db;

    public EfBorrowingRepository(EquipmentBorrowingDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Borrowing>> GetActiveByStudentIdAsync(int studentId)
    {
        return await _db.Borrowings
            .Where(b => b.StudentId == studentId && b.Status == BorrowingStatus.Active)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Borrowing?> GetByIdAsync(Guid borrowingId)
    {
        return await _db.Borrowings
            .Include(b => b.Equipment)
            .Include(b => b.Student)
            .FirstOrDefaultAsync(b => b.BorrowingId == borrowingId);
    }

    public async Task AddAsync(Borrowing borrowing)
    {
        await _db.Borrowings.AddAsync(borrowing);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Borrowing borrowing)
    {
        _db.Borrowings.Update(borrowing);
        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Borrowing>> GetAllActiveAsync()
    {
        return await _db.Borrowings
            .Where(b => b.Status == BorrowingStatus.Active)
            .AsNoTracking()
            .ToListAsync();
    }
}