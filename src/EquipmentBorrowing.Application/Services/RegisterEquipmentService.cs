using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class RegisterEquipmentService
{
    private readonly IEquipmentRepository _equipmentRepository;

    public RegisterEquipmentService(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task<Equipment> RegisterEquipmentAsync(
        string equipmentIdentifier,
        string name,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(equipmentIdentifier))
            throw new ArgumentException("Equipment identifier is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Equipment name is required.");

        string normalizedIdentifier =
            equipmentIdentifier.Trim().ToUpperInvariant();

        var existingEquipment =
            await _equipmentRepository.GetByIdentifierAsync(
                normalizedIdentifier);

        if (existingEquipment is not null)
            throw new InvalidOperationException(
                "Equipment with this identifier already exists.");

        var equipment = new Equipment(
            normalizedIdentifier,
            name,
            description);

        await _equipmentRepository.AddAsync(equipment);

        return equipment;
    }
}