using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class UpdateEquipmentInformationService
{
    private readonly IEquipmentRepository _equipmentRepository;

    public UpdateEquipmentInformationService(
        IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    public async Task UpdateEquipmentInformationAsync(
        Guid equipmentId,
        string equipmentIdentifier,
        string name,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(equipmentIdentifier))
            throw new ArgumentException("Equipment identifier is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Equipment name is required.");

        var equipment = await _equipmentRepository.GetByIdAsync(equipmentId);

        if (equipment is null)
            throw new InvalidOperationException("Equipment not found.");

        string normalizedIdentifier =
            equipmentIdentifier.Trim().ToUpperInvariant();

        var existingEquipment =
            await _equipmentRepository.GetByIdentifierAsync(
                normalizedIdentifier);

        if (existingEquipment is not null &&
            existingEquipment.EquipmentId != equipmentId)
        {
            throw new InvalidOperationException(
                "Equipment with this identifier already exists.");
        }

        equipment.UpdateInformation(
            normalizedIdentifier,
            name,
            description);

        await _equipmentRepository.UpdateAsync(equipment);
    }
}