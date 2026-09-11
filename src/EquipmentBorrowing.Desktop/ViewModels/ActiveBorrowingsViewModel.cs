using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;


namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class ActiveBorrowingsViewModel : ObservableObject
{
    public ObservableCollection<Borrowing> ActiveBorrowings { get; } = new();


    private readonly IBorrowingRepository _borrowingRepository;
    private readonly ReturnEquipmentService _returnEquipmentService;




    public ActiveBorrowingsViewModel(IBorrowingRepository borrowingRepository, ReturnEquipmentService returnEquipmentService)
    {
        _borrowingRepository = borrowingRepository;
        _returnEquipmentService= returnEquipmentService;
    }



    [RelayCommand]
    private async Task LoadBorrowingsAsync() {
        var borrowings = await _borrowingRepository.GetActiveByStudentIdAsync(1);

        ActiveBorrowings.Clear();

        foreach (var borrowing in borrowings) {
            ActiveBorrowings.Add(borrowing);
        }
        
    }



    [RelayCommand]
    private async Task ReturnAsync(Borrowing borrowing)
    {
        await _returnEquipmentService.ReturnEquipmentAsync(
            borrowing.BorrowingId);

        await LoadBorrowingsAsync();
    }
}

