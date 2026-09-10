using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Domain;


namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class ActiveBorrowingsViewModel : ObservableObject
{
    public ObservableCollection<BorrowingItem> ActiveBorrowings { get; }


    private readonly IBorrowingRepository _borrowingRepository;
    private readonly ReturnEquipmentService _returnEquipmentService;




    public ActiveBorrowingsViewModel(IBorrowingRepository borrowingRepository, ReturnEquipmentService returnEquipmentService)
    {
        _borrowingRepository = borrowingRepository;
        _returnEquipmentService= returnEquipmentService;
    }



    [RelayCommand]
    private async Task LoadBorrowingsAsync() { 
    
    }



    [RelayCommand]
    private async Task ReturnAsync(Borrowing borrowing)
    {
        await _returnEquipmentService.ReturnEquipmentAsync(
            borrowing.BorrowingId);

        await LoadBorrowingsAsync();
    }
}

public class BorrowingItem
{
    public int Id { get; set; }

    public string EquipmentName { get; set; } = "";

    public string BorrowerName { get; set; } = "";

    public DateTime BorrowedAt { get; set; }
}