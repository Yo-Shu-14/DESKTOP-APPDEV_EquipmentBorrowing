using EquipmentBorrowing.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using global::EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class ReturnEquipmentService
{
    private readonly IBorrowingRepository _borrowingRepository;
    private readonly IEquipmentRepository _equipmentRepository;

    public ReturnEquipmentService(
        IBorrowingRepository borrowingRepository,
        IEquipmentRepository equipmentRepository)
    {
        _borrowingRepository = borrowingRepository;
        _equipmentRepository = equipmentRepository;
    }

    public async Task ReturnEquipmentAsync(Guid borrowingId)
    {
        var borrowing = await _borrowingRepository.GetByIdAsync(borrowingId);

        if (borrowing is null)
        {
            throw new InvalidOperationException("Borrowing does not exist.");
        }

        if (borrowing.Status == BorrowingStatus.Returned)
        {
            throw new InvalidOperationException("Equipment has already been returned.");
        }

        borrowing.MarkAsReturned();

        borrowing.Equipment.MarkAsAvailable();

        await _borrowingRepository.UpdateAsync(borrowing);

        await _equipmentRepository.UpdateAsync(borrowing.Equipment);
    }
}