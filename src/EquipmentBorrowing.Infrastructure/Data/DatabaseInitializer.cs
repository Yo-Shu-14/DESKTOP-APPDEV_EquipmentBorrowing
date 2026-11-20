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

        // 1. Initial Students (Eligible, Ineligible, and with/without history)
        var student1 = new Student("2024-0001", "Jang Kaloy", "0917-123-4567");
        student1.ChangeBorrowingEligibility(true);

        var student2 = new Student("2024-0002", "Emji Jid", "0918-765-4321");
        student2.ChangeBorrowingEligibility(false); // Ineligible student for testing UC-14 / restriction

        var student3 = new Student("2024-0003", "Maria Santos", null);
        student3.ChangeBorrowingEligibility(true);

        var student4 = new Student("2024-0004", "Alex Rivera", null);
        student4.ChangeBorrowingEligibility(true); // Student with no history for UC-07 empty-state testing

        db.Students.AddRange(student1, student2, student3, student4);
        await db.SaveChangesAsync();

        // 2. Initial Equipment (Available, Unavailable, and with/without history)
        var eqLaptop = new Equipment("EQ-001", "Lab Laptop Dell XPS", "15-inch development laptop");
        var eqProjector = new Equipment("EQ-002", "Epson Projector", "HDMI/VGA 1080p projector");
        var eqCamera = new Equipment("EQ-003", "Sony Alpha Camera", "Mirrorless digital camera with 28-70mm lens");
        var eqMic = new Equipment("EQ-004", "Wireless Microphone", "Dual channel wireless lavalier microphone");
        var eqOscilloscope = new Equipment("EQ-005", "Digital Oscilloscope", "100MHz 2-channel digital storage oscilloscope");
        var eqTripod = new Equipment("EQ-006", "Heavy Duty Tripod", "Adjustable aluminum camera tripod"); // No history for UC-08

        // Mark items with active borrowings as unavailable per BR-10
        eqLaptop.MarkAsUnavailable();
        eqCamera.MarkAsUnavailable();

        db.Equipment.AddRange(eqLaptop, eqProjector, eqCamera, eqMic, eqOscilloscope, eqTripod);
        await db.SaveChangesAsync();

        // 3. Initial Borrowing Records (Active loans and Returned history)
        var now = DateTime.UtcNow;

        // Active borrowing 1: student1 (Jang Kaloy) has eqLaptop
        var active1 = new Borrowing(
            Guid.NewGuid(),
            student1,
            eqLaptop,
            now.AddDays(-2),
            now.AddDays(5),
            BorrowingStatus.Active);

        // Active borrowing 2: student3 (Maria Santos) has eqCamera
        var active2 = new Borrowing(
            Guid.NewGuid(),
            student3,
            eqCamera,
            now.AddDays(-1),
            now.AddDays(6),
            BorrowingStatus.Active);

        // Returned borrowing 1: student1 (Jang Kaloy) previously borrowed eqProjector
        var returned1 = new Borrowing(
            Guid.NewGuid(),
            student1,
            eqProjector,
            now.AddDays(-14),
            now.AddDays(-7),
            BorrowingStatus.Returned);
        returned1.MarkAsReturned();

        // Returned borrowing 2: student3 (Maria Santos) previously borrowed eqMic
        var returned2 = new Borrowing(
            Guid.NewGuid(),
            student3,
            eqMic,
            now.AddDays(-10),
            now.AddDays(-3),
            BorrowingStatus.Returned);
        returned2.MarkAsReturned();

        db.Borrowings.AddRange(active1, active2, returned1, returned2);
        await db.SaveChangesAsync();
    }
}