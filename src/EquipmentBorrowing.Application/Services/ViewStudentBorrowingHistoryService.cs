using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class ViewStudentBorrowingHistoryService
{
    private readonly IBorrowingRepository _borrowingRepository;

    public ViewStudentBorrowingHistoryService(
        IBorrowingRepository borrowingRepository)
    {
        _borrowingRepository = borrowingRepository;
    }

    public async Task<IReadOnlyList<Borrowing>> GetStudentHistoryAsync(
        int studentId)
    {
        return await _borrowingRepository.GetAllByStudentIdAsync(studentId);
    }
}