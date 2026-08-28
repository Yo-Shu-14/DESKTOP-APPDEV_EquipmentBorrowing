using System;
using System.Collections.Generic;
using System.Text;

using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
namespace EquipmentBorrowing.Infrastructure;

internal class InMemoryBorrowingRepository : IBorrowingRepository
{
    private readonly List<Borrowing> _borrowings = new();

    public Task<IReadOnlyList<Borrowing>> GetActiveByStudentIdAsync(int studentId)
    {
        var activeBorrowings = _borrowings
            .Where(borrowing =>
                borrowing.Student.Id == studentId &&
                borrowing.Status == BorrowingStatus.Active)
            .ToList();

        return Task.FromResult<IReadOnlyList<Borrowing>>(activeBorrowings);
    }

    public Task<Borrowing?> GetByIdAsync(Guid borrowingId)
    {
        var borrowing = _borrowings
            .FirstOrDefault(borrowing => borrowing.BorrowingId == borrowingId);

        return Task.FromResult(borrowing);
    }

    public Task AddAsync(Borrowing borrowing)
    {
        _borrowings.Add(borrowing);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Borrowing borrowing)
    {
        return Task.CompletedTask;
    }
}
