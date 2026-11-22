
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class ReturnEquipmentService
{
    private readonly IBorrowingRepository _borrowingRepository;

    public ReturnEquipmentService(
        IBorrowingRepository borrowingRepository)
    {
        _borrowingRepository = borrowingRepository;
    }

    public async Task<Borrowing> ReturnEquipmentAsync(Guid borrowingId)
    {
        var borrowing = await _borrowingRepository.GetByIdAsync(borrowingId);

        if (borrowing is null)
        {
            throw new InvalidOperationException(
                "Borrowing does not exist.");
        }

        if (borrowing.Status == BorrowingStatus.Returned)
        {
            throw new InvalidOperationException(
                "Equipment has already been returned.");
        }

        borrowing.MarkAsReturned();

        await _borrowingRepository.ReturnBorrowingWithEquipmentUpdateAsync(
            borrowing,
            borrowing.Equipment);

        return borrowing;
    }
}
