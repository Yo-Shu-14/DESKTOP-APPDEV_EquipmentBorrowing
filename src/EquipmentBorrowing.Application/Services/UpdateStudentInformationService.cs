using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class UpdateStudentInformationService
{
    private readonly IStudentRepository _studentRepository;

    public UpdateStudentInformationService(
        IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Student> UpdateStudentAsync(
        int studentId,
        string institutionalNumber,
        string name,
        string? contactInformation = null)
    {
        if (string.IsNullOrWhiteSpace(institutionalNumber))
            throw new ArgumentException("Institutional number is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Student name is required.");

        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student is null)
            throw new InvalidOperationException("Student not found.");

        string normalizedNumber =
            institutionalNumber.Trim().ToUpperInvariant();

        var existingStudent =
            await _studentRepository.GetByInstitutionalNumberAsync(
                normalizedNumber);

        if (existingStudent is not null && existingStudent.Id != studentId)
            throw new InvalidOperationException(
                "A student with this institutional number already exists.");

        student.UpdateInformation(
            normalizedNumber,
            name,
            contactInformation);

        await _studentRepository.UpdateAsync(student);

        return student;
    }
}