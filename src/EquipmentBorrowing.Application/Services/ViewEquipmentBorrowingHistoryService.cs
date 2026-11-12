using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class ViewEquipmentBorrowingHistoryService
{
    private readonly IBorrowingRepository _borrowingRepository;

    public ViewEquipmentBorrowingHistoryService(
        IBorrowingRepository borrowingRepository)
    {
        _borrowingRepository = borrowingRepository;
    }

    public async Task<IReadOnlyList<Borrowing>> GetEquipmentHistoryAsync(
        Guid equipmentId)
    {
        return await _borrowingRepository
            .GetAllByEquipmentIdAsync(equipmentId);
    }
}