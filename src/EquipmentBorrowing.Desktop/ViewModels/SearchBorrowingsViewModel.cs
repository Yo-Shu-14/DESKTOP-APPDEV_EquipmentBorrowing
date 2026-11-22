using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Desktop.ApiClient;
using System.Collections.ObjectModel;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class SearchBorrowingsViewModel : ObservableObject
{
    private readonly EquipmentBorrowingApiClient _api;

    public ObservableCollection<BorrowingDto> Borrowings { get; } = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedStatus = "All";

    [ObservableProperty]
    private string selectedSortField = "Checkout Date";

    [ObservableProperty]
    private string selectedSortDirection = "Descending";

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private bool isError;

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    public List<string> StatusOptions { get; } = new() { "All", "Active", "Returned" };

    public List<string> SortFieldOptions { get; } = new() { "Checkout Date", "Expected Return Date" };

    public List<string> SortDirectionOptions { get; } = new() { "Descending", "Ascending" };

    public SearchBorrowingsViewModel(EquipmentBorrowingApiClient api)
    {
        _api = api;
    }

    [RelayCommand]
    private async Task LoadBorrowingsAsync()
    {
        try
        {
            IsLoading = true;
            IsError = false;
            IsEmpty = false;
            FeedbackMessage = string.Empty;

            string? apiStatus = SelectedStatus == "All" ? null : SelectedStatus;
            string apiSort = SelectedSortField == "Expected Return Date" ? "expectedReturnDate" : "borrowedDate";
            string apiDirection = SelectedSortDirection == "Ascending" ? "asc" : "desc";
            string? apiSearch = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText.Trim();

            var results = await _api.GetBorrowingsAsync(
                search: apiSearch,
                status: apiStatus,
                sort: apiSort,
                direction: apiDirection);

            Borrowings.Clear();

            foreach (var borrowing in results)
            {
                Borrowings.Add(borrowing);
            }

            IsEmpty = Borrowings.Count == 0;

            FeedbackMessage = IsEmpty
                ? "No borrowing records found."
                : $"{Borrowings.Count} borrowing record(s) found.";
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
            FeedbackMessage = "Unable to load borrowing records. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = LoadBorrowingsAsync();
    }

    partial void OnSelectedStatusChanged(string value)
    {
        _ = LoadBorrowingsAsync();
    }

    partial void OnSelectedSortFieldChanged(string value)
    {
        _ = LoadBorrowingsAsync();
    }

    partial void OnSelectedSortDirectionChanged(string value)
    {
        _ = LoadBorrowingsAsync();
    }

    [RelayCommand]
    private async Task ResetFiltersAsync()
    {
        SearchText = string.Empty;
        SelectedStatus = "All";
        SelectedSortField = "Checkout Date";
        SelectedSortDirection = "Descending";

        await LoadBorrowingsAsync();
    }
}
