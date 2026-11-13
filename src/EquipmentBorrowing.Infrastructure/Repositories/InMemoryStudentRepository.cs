using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Infrastructure.Repositories;

public class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<Student> _students;

    public InMemoryStudentRepository(IEnumerable<Student> students)
    {
        _students = students.ToList();
    }

    public Task<Student?> GetByIdAsync(int id)
    {
        var student = _students.FirstOrDefault(
            student => student.Id == id);

        return Task.FromResult(student);
    }

    public Task<IReadOnlyList<Student>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyList<Student>>(_students);
    }

    public Task<Student?> GetByInstitutionalNumberAsync(
        string institutionalNumber)
    {
        string normalizedNumber = institutionalNumber.Trim().ToUpper();

        var student = _students.FirstOrDefault(
            student => student.InstitutionalNumber.ToUpper() == normalizedNumber);

        return Task.FromResult(student);
    }

    public Task AddAsync(Student student)
    {
        _students.Add(student);

        return Task.CompletedTask;
    }
    public Task UpdateAsync(Student student)
    {
        var existingStudent = _students
            .FirstOrDefault(s => s.Id == student.Id);

        if (existingStudent is null)
            throw new InvalidOperationException("Student not found.");

        existingStudent.UpdateInformation(
            student.InstitutionalNumber,
            student.Name,
            student.ContactInformation);

        return Task.CompletedTask;
    }
}