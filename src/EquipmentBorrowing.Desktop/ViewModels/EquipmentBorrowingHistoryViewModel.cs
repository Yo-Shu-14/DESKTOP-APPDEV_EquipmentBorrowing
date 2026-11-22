using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Desktop.ApiClient;
using System.Collections.ObjectModel;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentBorrowingHistoryViewModel : ObservableObject
{
    private readonly EquipmentBorrowingApiClient _api;

    public ObservableCollection<EquipmentDto> EquipmentList { get; } = new();

    public ObservableCollection<BorrowingDto> BorrowingHistory { get; } = new();

    [ObservableProperty]
    private EquipmentDto? selectedEquipment;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private bool isError;

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    public EquipmentBorrowingHistoryViewModel(EquipmentBorrowingApiClient api)
    {
        _api = api;
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
                var equipment = await _api.GetEquipmentAsync();

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

            var history = await _api.GetEquipmentBorrowingsAsync(
                SelectedEquipment.Id);

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
        catch (ApiConnectionException)
        {
            IsError = true;
            IsEmpty = false;
            FeedbackMessage = "Cannot reach the API server. Please ensure the API is running.";
        }
        catch (ApiServerException ex)
        {
            IsError = true;
            IsEmpty = false;
            FeedbackMessage = $"Server error: {ex.Message}";
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
