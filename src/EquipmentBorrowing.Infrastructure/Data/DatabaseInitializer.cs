using EquipmentBorrowing.Domain;
using Microsoft.EntityFrameworkCore;

namespace EquipmentBorrowing.Infrastructure.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        EquipmentBorrowingDbContext db)
    {
        await db.Database.MigrateAsync();

        if (await db.Students.AnyAsync())
        {
            return;
        }

        var student1 = new Student(
            1,
            "jang kaloy",
            true,
            "2024-0001");

        var student2 = new Student(
            2,
            "emji jid",
            false,
            "2024-0002");

        var equipment1 = new Equipment(
            "EQ-001",
            "Laptop",
            "Lab Laptop");

        var equipment2 = new Equipment(
            "EQ-002",
            "Projector",
            "Epson Projector");

        var equipment3 = new Equipment(
            "EQ-003",
            "Camera",
            "Digital Camera");

        var equipment4 = new Equipment(
            "EQ-004",
            "Microphone",
            "Wireless Microphone");

        db.Students.AddRange(
            student1,
            student2);

        db.Equipment.AddRange(
            equipment1,
            equipment2,
            equipment3,
            equipment4);

        await db.SaveChangesAsync();
    }
}