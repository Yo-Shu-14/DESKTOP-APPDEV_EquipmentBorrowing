using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Desktop.ApiClient;
using System.Collections.ObjectModel;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class ActiveBorrowingsViewModel : ObservableObject
{
    private readonly EquipmentBorrowingApiClient _api;

    public ObservableCollection<BorrowingDto> ActiveBorrowings { get; } = new();

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private bool isError;

    public ActiveBorrowingsViewModel(EquipmentBorrowingApiClient api)
    {
        _api = api;
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

            var borrowings = await _api.GetBorrowingsAsync(status: "Active");

            ActiveBorrowings.Clear();
            foreach (var borrowing in borrowings)
            {
                ActiveBorrowings.Add(borrowing);
            }

            IsEmpty = ActiveBorrowings.Count == 0;
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
            FeedbackMessage = "Unable to load active borrowings. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ReturnAsync(BorrowingDto borrowing)
    {
        try
        {
            await _api.ReturnBorrowingAsync(borrowing.Id);
            await LoadBorrowingsAsync();

            if (!IsError)
            {
                FeedbackMessage = "Equipment returned successfully.";
            }
        }
        catch (ApiConnectionException)
        {
            FeedbackMessage = "Cannot reach the API server. Please ensure the API is running.";
        }
        catch (ApiServerException ex)
        {
            FeedbackMessage = $"Server error: {ex.Message}";
        }
        catch (Exception ex)
        {
            FeedbackMessage = ex.Message;
        }
    }
}
