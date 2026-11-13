namespace EquipmentBorrowing.Domain;

public class Equipment
{
    public Guid EquipmentId { get; private set; }
    public string EquipmentIdentifier { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsAvailable { get; private set; }

    // Existing constructor for seed data and tests
    public Equipment(
        Guid equipmentId,
        string name,
        string description,
        bool isAvailable)
    {
        EquipmentId = equipmentId;
        EquipmentIdentifier = $"LEGACY-{equipmentId:N}";
        Name = name;
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
        IsAvailable = isAvailable;
    }

    // Constructor for registering new equipment
    public Equipment(
        string equipmentIdentifier,
        string name,
        string? description = null)
    {
        EquipmentId = Guid.NewGuid();
        EquipmentIdentifier = equipmentIdentifier.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
        IsAvailable = true;
    }

    public void MarkAsAvailable()
    {
        IsAvailable = true;
    }

    public void MarkAsUnavailable()
    {
        IsAvailable = false;
    }
}