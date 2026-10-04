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
            true);

        var student2 = new Student(
            2,
            "emji jid",
            false);

        var equipment1 = new Equipment(
            Guid.NewGuid(),
            "Laptop",
            "Lab Laptop",
            true);

        var equipment2 = new Equipment(
            Guid.NewGuid(),
            "Projector",
            "Epson Projector",
            true);

        var equipment3 = new Equipment(
            Guid.NewGuid(),
            "Camera",
            "Digital Camera",
            true);

        var equipment4 = new Equipment(
            Guid.NewGuid(),
            "Microphone",
            "Wireless Microphone",
            true);

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