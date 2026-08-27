using System;
using System.Collections.Generic;
using System.Text;


using EquipmentBorrowing.Domain;
namespace EquipmentBorrowing.Application.Interfaces;

public interface IBorrowingRepository
{
    Task<IReadOnlyList<Borrowing>> GetActiveByStudentIdAsync(int studentId);

    Task AddAsync(Borrowing borrowing);
}
