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
        var equipmentList = await _equipmentRepository.GetAllAsync();

        var availableEquipment = equipmentList
            .Where(equipment => equipment.IsAvailable)
            .ToList();

        return availableEquipment;
    }
}