using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using System;
using System.Collections.Generic;
using System.Text;

using EquipmentBorrowing.Application.Interfaces;
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

    public async Task BorrowEquipmentAsync(int studentId, Guid equipmentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student is null)
        {
            throw new InvalidOperationException("Student does not exist.");
        }
    }
}
