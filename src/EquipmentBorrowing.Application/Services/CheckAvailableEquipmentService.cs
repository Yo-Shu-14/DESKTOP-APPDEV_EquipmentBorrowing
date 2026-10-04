using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentBorrowing.Application.Services;

public class CheckAvailableEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepository;

    public CheckAvailableEquipmentService(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<IReadOnlyList<Equipment>> CheckAvailableEquipmentAsync()
    {
        return await _equipmentRepository.GetAvailableAsync();
    }
}