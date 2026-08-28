
using System;
using System.Collections.Generic;
using System.Text;

using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
namespace EquipmentBorrowing.Application.Services;

public class BorrowEquipmentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IBorrowingRepository _borrowingRepository;

    public BorrowEquipmentService(IStudentRepository studentRepository, IEquipmentRepository equipmentRepository, IBorrowingRepository borrowingRepository)
    {
        _studentRepository = studentRepository;
        _equipmentRepository = equipmentRepository;
        _borrowingRepository = borrowingRepository;
    }

    public async Task BorrowEquipmentAsync(int studentId, Guid equipmentId, int maximumActiveBorrowings, DateTime expectedReturnDate)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student is null)
        {
            throw new InvalidOperationException("Student does not exist.");
        }


        if (!student.IsAllowedToBorrow)
        {
            throw new InvalidOperationException("Student is not allowed to borrow equipment.");
        }

        var equipment = await _equipmentRepository.GetByIdAsync(equipmentId);

        if (equipment is null)
        {
            throw new InvalidOperationException("Equipment does not exist.");
        }


        if (!equipment.IsAvailable)
        {
            throw new InvalidOperationException("Equipment is not available.");
        }


        var activeBorrowings = await _borrowingRepository.GetActiveByStudentIdAsync(studentId);
        if (activeBorrowings.Count >= maximumActiveBorrowings)
        {
            throw new InvalidOperationException("Student has reached the maximum number of active borrowings.");
        }





        var borrowing = new Borrowing(Guid.NewGuid(), student, equipment, DateTime.Now, expectedReturnDate, BorrowingStatus.Active);

        equipment.MarkAsUnavailable();

        await _borrowingRepository.AddAsync(borrowing);
    }
}
