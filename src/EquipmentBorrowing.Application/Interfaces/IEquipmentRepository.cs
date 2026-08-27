using System;
using System.Collections.Generic;
using System.Text;

using EquipmentBorrowing.Domain;
namespace EquipmentBorrowing.Application.Interfaces;

public interface IEquipmentRepository
{
    Task<Equipment?> GetByIdAsync(Guid id);
}
