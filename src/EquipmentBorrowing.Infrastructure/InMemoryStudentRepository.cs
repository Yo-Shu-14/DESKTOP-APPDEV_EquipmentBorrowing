using System;
using System.Collections.Generic;
using System.Text;

using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
namespace EquipmentBorrowing.Infrastructure;

public class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<Student> _students = new();

    public Task<Student?> GetByIdAsync(int id)
    {
        var student = _students.FirstOrDefault(student => student.Id == id);

        return Task.FromResult(student);
    }
}

