
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class ViewEquipmentCatalogService
{
    private readonly IEquipmentRepository _equipmentRepository;

    public ViewEquipmentCatalogService(
        IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<IReadOnlyList<Equipment>> ViewEquipmentCatalogAsync()
    {
        return await _equipmentRepository.GetAllAsync();
    }
}
