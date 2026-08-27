using System;
using System.Collections.Generic;
using System.Text;

using EquipmentBorrowing.Domain;
namespace EquipmentBorrowing.Application.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id);
}
