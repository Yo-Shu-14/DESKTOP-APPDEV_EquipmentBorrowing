using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure;

public class BorrowEquipmentFailedCase
{
    public static async Task Run()
    {
        var student = new Student(
    1,
    "John D Natuto",
    true);

        var equipmentId = Guid.NewGuid();

        var equipment = new Equipment(
            equipmentId,
            "Mouse",
            "Laboratory mouse",
            false);

        var studentRepository = new InMemoryStudentRepository(
            new[] { student });

        var equipmentRepository = new InMemoryEquipmentRepository(
            new[] { equipment });

        var borrowingRepository = new InMemoryBorrowingRepository();

        var borrowEquipmentService = new BorrowEquipmentService(
            studentRepository,
            equipmentRepository,
            borrowingRepository);

        try
        {
            await borrowEquipmentService.BorrowEquipmentAsync(
                student.Id,
                equipment.EquipmentId,
                3,
                DateTime.Now.AddDays(7));

            Console.WriteLine("Equipment borrowed successfully.");
            Console.WriteLine($"Borrowed by: {student.Name}");
            Console.WriteLine($"Equipment: {equipment.Name}");

        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Borrowing failed: {ex.Message}");
        }
    }
}