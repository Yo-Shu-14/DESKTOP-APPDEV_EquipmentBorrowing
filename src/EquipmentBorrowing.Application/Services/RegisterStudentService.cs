using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class RegisterStudentService
{
    private readonly IStudentRepository _studentRepository;

    public RegisterStudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<Student> RegisterStudentAsync(
        string institutionalNumber,
        string name,
        string? contactInformation = null)
    {
        if (string.IsNullOrWhiteSpace(institutionalNumber))
            throw new ArgumentException("Institutional number is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Student name is required.");

        string normalizedNumber =
            institutionalNumber.Trim().ToUpperInvariant();

        var existingStudent =
            await _studentRepository.GetByInstitutionalNumberAsync(
                normalizedNumber);

        if (existingStudent is not null)
            throw new InvalidOperationException(
                "A student with this institutional number already exists.");

        var student = new Student(
            normalizedNumber,
            name,
            contactInformation);

        await _studentRepository.AddAsync(student);

        return student;
    }
}