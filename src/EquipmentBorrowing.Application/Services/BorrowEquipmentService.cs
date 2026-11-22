
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

    public async Task<Borrowing> BorrowEquipmentAsync(int studentId, Guid equipmentId, int maximumActiveBorrowings, DateTime expectedReturnDate)
    {
        if (expectedReturnDate.Date <= DateTime.Today)
        {
            throw new InvalidOperationException(
                "Expected return date must be in the future.");
        }
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





        var borrowing = new Borrowing(
            Guid.NewGuid(),
            student,
            equipment,
            DateTime.UtcNow,
            expectedReturnDate.Kind == DateTimeKind.Utc
                ? expectedReturnDate
                : expectedReturnDate.ToUniversalTime(),
            BorrowingStatus.Active);

        await _borrowingRepository.AddBorrowingWithEquipmentUpdateAsync(
            borrowing,
            equipment);

        return borrowing;
    }

    public async Task<IReadOnlyList<Borrowing>> BorrowMultipleEquipmentAsync(
        int studentId,
        IReadOnlyList<Guid> equipmentIds,
        int maximumActiveBorrowings,
        DateTime expectedReturnDate)
    {
        if (equipmentIds.Count < 2)
        {
            throw new InvalidOperationException(
                "Please select at least two equipment items.");
        }

        if (equipmentIds.Distinct().Count() != equipmentIds.Count)
        {
            throw new InvalidOperationException(
                "Duplicate equipment items are not allowed.");
        }

        if (expectedReturnDate.Date <= DateTime.Today)
        {
            throw new InvalidOperationException(
                "Expected return date must be in the future.");
        }

        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student is null)
        {
            throw new InvalidOperationException("Student does not exist.");
        }

        if (!student.IsAllowedToBorrow)
        {
            throw new InvalidOperationException(
                "Student is not allowed to borrow equipment.");
        }

        var activeBorrowings =
            await _borrowingRepository.GetActiveByStudentIdAsync(studentId);

        if (activeBorrowings.Count + equipmentIds.Count >
            maximumActiveBorrowings)
        {
            throw new InvalidOperationException(
                "Student would exceed the maximum number of active borrowings.");
        }

        var equipmentList = new List<Equipment>();

        foreach (var equipmentId in equipmentIds)
        {
            var equipment = await _equipmentRepository.GetByIdAsync(equipmentId);

            if (equipment is null)
            {
                throw new InvalidOperationException(
                    "One or more equipment items do not exist.");
            }

            if (!equipment.IsAvailable)
            {
                throw new InvalidOperationException(
                    $"{equipment.Name} is not available.");
            }

            equipmentList.Add(equipment);
        }

        var borrowDate = DateTime.UtcNow;

        var returnDate = expectedReturnDate.Kind == DateTimeKind.Utc
            ? expectedReturnDate
            : expectedReturnDate.ToUniversalTime();

        var borrowings = equipmentList.Select(equipment =>
            new Borrowing(
                Guid.NewGuid(),
                student,
                equipment,
                borrowDate,
                returnDate,
                BorrowingStatus.Active))
            .ToList();

        await _borrowingRepository.AddBorrowingsWithEquipmentUpdatesAsync(
            borrowings,
            equipmentList);

        return borrowings;
    }

}
