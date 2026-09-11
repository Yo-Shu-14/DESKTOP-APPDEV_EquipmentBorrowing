using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using Avalonia.Controls.Primitives;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentViewModel : ObservableObject
{
    private readonly CheckAvailableEquipmentService _checkAvailableEquipmentService;
    private readonly BorrowEquipmentService _borrowEquipmentService;

    public ObservableCollection<Equipment> EquipmentList { get; } = new();



    //
    public int SelectedId { get; set; } = 1;
    public Equipment? SelectedEquipment { get; set; }
    public DateTimeOffset? ExpectedReturnDate { get; set; } = DateTimeOffset.Now.AddDays(7);

    [ObservableProperty]
    public string feedbackMessage = string.Empty;


    public EquipmentViewModel(CheckAvailableEquipmentService checkAvailableEquipmentService, BorrowEquipmentService borrowEquipmentService)
    {
        _checkAvailableEquipmentService = checkAvailableEquipmentService;
        _borrowEquipmentService = borrowEquipmentService;
    }

    [RelayCommand]
    private async Task LoadEquipmentAsync() 
    {
        var equipment  = await _checkAvailableEquipmentService.CheckAvailableEquipmentAsync();

        EquipmentList.Clear();

        foreach (var item in equipment)
        {
            EquipmentList.Add(item);
        }

    }


    
    // borrow com
    [RelayCommand]
    private async Task BorrowAsync(Equipment equipment)
    {
        SelectedEquipment = equipment;

        if (ExpectedReturnDate == null)
        {
            FeedbackMessage = "Please select an expected return date!";
            return;
        }

        try
        {
            await _borrowEquipmentService.BorrowEquipmentAsync(
                SelectedId,
                SelectedEquipment.EquipmentId,
                3,
                ExpectedReturnDate.Value.DateTime
                );

            FeedbackMessage = "Successful Borrowing";

            await LoadEquipmentAsync();

        }
        catch (Exception ex) {
            FeedbackMessage = ex.Message;
        }
           

        
    }
}

