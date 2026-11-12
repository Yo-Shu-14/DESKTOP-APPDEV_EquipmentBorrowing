using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class ActiveBorrowingsViewModel : ObservableObject
{
    public ObservableCollection<Borrowing> ActiveBorrowings { get; } = new();

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private bool isError;

    private readonly IBorrowingRepository _borrowingRepository;
    private readonly ReturnEquipmentService _returnEquipmentService;




    public ActiveBorrowingsViewModel(IBorrowingRepository borrowingRepository, ReturnEquipmentService returnEquipmentService)
    {
        _borrowingRepository = borrowingRepository;
        _returnEquipmentService= returnEquipmentService;
    }


    [RelayCommand]
    private async Task LoadBorrowingsAsync()
    {
        try
        {
            IsLoading = true;
            IsEmpty = false;
            IsError = false;
            FeedbackMessage = string.Empty;

            var borrowings = await _borrowingRepository.GetAllActiveAsync();

            ActiveBorrowings.Clear();

            foreach (var borrowing in borrowings)
            {
                ActiveBorrowings.Add(borrowing);
            }

            IsEmpty = ActiveBorrowings.Count == 0;

            if (IsEmpty)
            {
                FeedbackMessage = "No active borrowings found.";
            }
        }
        catch (Exception)
        {
            IsError = true;
            IsEmpty = false;
            FeedbackMessage = "Unable to load active borrowings. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }



    [RelayCommand]
    private async Task ReturnAsync(Borrowing borrowing)
    {
        try
        {
            await _returnEquipmentService.ReturnEquipmentAsync(
                borrowing.BorrowingId);

            await LoadBorrowingsAsync();

            if (!IsError)
            {
                FeedbackMessage = "Equipment returned successfully.";
            }
        }
        catch (Exception ex)
        {
            FeedbackMessage = ex.Message;
        }
    }
}

