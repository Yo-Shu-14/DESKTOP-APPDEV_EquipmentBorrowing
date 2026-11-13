using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Application.Interfaces;

public interface IBorrowingRepository
{
    Task<IReadOnlyList<Borrowing>> GetActiveByStudentIdAsync(int studentId);

    Task<Borrowing?> GetByIdAsync(Guid borrowingId);

    Task AddAsync(Borrowing borrowing);

    Task UpdateAsync(Borrowing borrowing);

    Task<IReadOnlyList<Borrowing>> GetAllActiveAsync();

    Task AddBorrowingWithEquipmentUpdateAsync(
        Borrowing borrowing,
        Equipment equipment);

    Task ReturnBorrowingWithEquipmentUpdateAsync(
        Borrowing borrowing,
        Equipment equipment);

    Task AddBorrowingsWithEquipmentUpdatesAsync(
        IReadOnlyList<Borrowing> borrowings,
        IReadOnlyList<Equipment> equipment);

    Task<IReadOnlyList<Borrowing>> GetAllByStudentIdAsync(int studentId);
    Task<IReadOnlyList<Borrowing>> GetAllByEquipmentIdAsync(Guid equipmentId);
    Task<IReadOnlyList<Borrowing>> GetAllAsync();
}