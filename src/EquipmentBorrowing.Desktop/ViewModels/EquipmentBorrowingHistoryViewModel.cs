using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentBorrowingHistoryViewModel : ObservableObject
{
    private readonly ViewEquipmentBorrowingHistoryService _historyService;
    private readonly IEquipmentRepository _equipmentRepository;

    public ObservableCollection<Equipment> EquipmentList { get; } = new();

    public ObservableCollection<Borrowing> BorrowingHistory { get; } = new();

    [ObservableProperty]
    private Equipment? selectedEquipment;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private bool isError;

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    public EquipmentBorrowingHistoryViewModel(
        ViewEquipmentBorrowingHistoryService historyService,
        IEquipmentRepository equipmentRepository)
    {
        _historyService = historyService;
        _equipmentRepository = equipmentRepository;
    }

    [RelayCommand]
    private async Task LoadHistoryAsync()
    {
        try
        {
            IsLoading = true;
            IsError = false;
            IsEmpty = false;
            FeedbackMessage = string.Empty;

            if (EquipmentList.Count == 0)
            {
                var equipment = await _equipmentRepository.GetAllAsync();

                foreach (var item in equipment)
                {
                    EquipmentList.Add(item);
                }
            }

            if (SelectedEquipment == null)
            {
                SelectedEquipment = EquipmentList.FirstOrDefault();
            }

            if (SelectedEquipment == null)
            {
                IsEmpty = true;
                FeedbackMessage = "No equipment registered yet.";
                BorrowingHistory.Clear();
                return;
            }

            var history = await _historyService.GetEquipmentHistoryAsync(
                SelectedEquipment.EquipmentId);

            BorrowingHistory.Clear();

            foreach (var borrowing in history)
            {
                BorrowingHistory.Add(borrowing);
            }

            IsEmpty = BorrowingHistory.Count == 0;

            if (IsEmpty)
            {
                FeedbackMessage = "No borrowing history found for this equipment.";
            }
        }
        catch (Exception)
        {
            IsError = true;
            IsEmpty = false;
            FeedbackMessage = "Unable to load equipment borrowing history. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}