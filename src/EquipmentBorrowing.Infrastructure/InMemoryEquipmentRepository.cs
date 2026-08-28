using System;
using System.Collections.Generic;
using System.Text;

using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
namespace EquipmentBorrowing.Infrastructure;

public class InMemoryEquipmentRepository : IEquipmentRepository
{
    private readonly List<Equipment> _equipment = new();

    public Task<Equipment?> GetByIdAsync(Guid id)
    {
        var equipment = _equipment
            .FirstOrDefault(equipment => equipment.EquipmentId == id);

        return Task.FromResult(equipment);
    }

    public Task<IReadOnlyList<Equipment>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyList<Equipment>>(_equipment);
    }

    public Task UpdateAsync(Equipment equipment)
    {
        return Task.CompletedTask;
    }
}

