using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class ChangeStudentEligibilityService
{
    private readonly IStudentRepository _studentRepository;

    public ChangeStudentEligibilityService(
        IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Student> ChangeEligibilityAsync(
        int studentId,
        bool isAllowed)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student is null)
            throw new InvalidOperationException("Student not found.");

        if (student.IsAllowedToBorrow == isAllowed)
            throw new InvalidOperationException(
                isAllowed
                    ? "This student is already permitted to borrow."
                    : "This student is already restricted from borrowing.");

        student.ChangeBorrowingEligibility(isAllowed);

        await _studentRepository.UpdateAsync(student);

        return student;
    }
}