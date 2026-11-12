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
            .Include(b => b.Student)
            .Include(b => b.Equipment)
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
            .Include(b => b.Student)
            .Include(b => b.Equipment)
            .Where(b => b.Status == BorrowingStatus.Active)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddBorrowingWithEquipmentUpdateAsync(
    Borrowing borrowing,
    Equipment equipment)
    {
        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        try
        {
            var updatedRows = await _db.Equipment
                .Where(e => e.EquipmentId == equipment.EquipmentId
                         && e.IsAvailable)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(e => e.IsAvailable, false));

            if (updatedRows == 0)
            {
                throw new InvalidOperationException(
                    "Equipment is no longer available. Please refresh and try again.");
            }

            await _db.Borrowings.AddAsync(borrowing);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task ReturnBorrowingWithEquipmentUpdateAsync(
    Borrowing borrowing,
    Equipment equipment)
    {
        await using var transaction =
            await _db.Database.BeginTransactionAsync();

        try
        {
            var updatedRows = await _db.Borrowings
                .Where(b => b.BorrowingId == borrowing.BorrowingId
                         && b.Status == BorrowingStatus.Active)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(b => b.Status, BorrowingStatus.Returned)
                    .SetProperty(b => b.ActualReturnDate, borrowing.ActualReturnDate));

            if (updatedRows == 0)
            {
                throw new InvalidOperationException(
                    "Borrowing has already been returned or does not exist.");
            }

            await _db.Equipment
                .Where(e => e.EquipmentId == equipment.EquipmentId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(e => e.IsAvailable, true));

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}