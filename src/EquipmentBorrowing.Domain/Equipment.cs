using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentBorrowing.Domain;

public class Equipment
{
    public Guid EquipmentId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsAvailable { get; private set; }


    public Equipment(Guid equipmentId, string name, string description, bool isAvailable)
    {
        EquipmentId = equipmentId;
        Name = name;
        Description = description;
        IsAvailable = isAvailable;
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