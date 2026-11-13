using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Services;

public class SearchBorrowingsService
{
    private readonly IBorrowingRepository _borrowingRepository;

    public SearchBorrowingsService(
        IBorrowingRepository borrowingRepository)
    {
        _borrowingRepository = borrowingRepository;
    }

    public async Task<IReadOnlyList<Borrowing>> GetAllBorrowingsAsync()
    {
        return await _borrowingRepository.GetAllAsync();
    }
}