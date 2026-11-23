namespace EquipmentBorrowing.Domain;

public class Student
{
    public int Id { get; private set; }
    public string InstitutionalNumber { get; private set; }
    public string Name { get; private set; }
    public string? ContactInformation { get; private set; }
    public bool IsAllowedToBorrow { get; private set; }

    // Existing constructor for seed data and tests
    public Student(
        int Id,
        string Name,
        bool IsAllowedToBorrow,
        string InstitutionalNumber = "",
        string? ContactInformation = null)
    {
        this.Id = Id;
        this.Name = Name;
        this.IsAllowedToBorrow = IsAllowedToBorrow;
        this.InstitutionalNumber = InstitutionalNumber;
        this.ContactInformation = ContactInformation;
    }

    // Constructor for registering new students
    public Student(
        string institutionalNumber,
        string name,
        string? contactInformation = null)
    {
        Id = 0;
        InstitutionalNumber = institutionalNumber.Trim().ToUpperInvariant();
        Name = name.Trim();
        ContactInformation = string.IsNullOrWhiteSpace(contactInformation)
            ? null
            : contactInformation.Trim();
        IsAllowedToBorrow = true;
    }
    public void UpdateInformation(
    string institutionalNumber,
    string name,
    string? contactInformation)
    {
        InstitutionalNumber = institutionalNumber.Trim().ToUpperInvariant();
        Name = name.Trim();
        ContactInformation = string.IsNullOrWhiteSpace(contactInformation)
            ? null
            : contactInformation.Trim();
    }
    public void ChangeBorrowingEligibility(bool isAllowed)
    {
        IsAllowedToBorrow = isAllowed;
    }
}

//MJ Jade G. Piquero - 2024300423
//John Carlo Magno - 2024301174