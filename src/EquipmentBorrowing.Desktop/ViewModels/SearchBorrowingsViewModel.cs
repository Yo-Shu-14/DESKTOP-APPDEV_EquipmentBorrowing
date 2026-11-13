using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class SearchBorrowingsViewModel : ObservableObject
{
    private readonly SearchBorrowingsService _searchService;

    private List<Borrowing> _allBorrowings = new();

    public ObservableCollection<Borrowing> Borrowings { get; } = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedStatus = "All";

    [ObservableProperty]
    private string selectedSortField = "Date Borrowed";

    [ObservableProperty]
    private string selectedSortDirection = "Newest First";

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private bool isError;

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    public SearchBorrowingsViewModel(SearchBorrowingsService searchService)
    {
        _searchService = searchService;
    }
    public List<string> StatusOptions { get; } =
    new() { "All", "Active", "Returned" };

    public List<string> SortFieldOptions { get; } =
        new() { "Date Borrowed", "Student Name", "Equipment Name" };

    public List<string> SortDirectionOptions { get; } =
        new() { "Newest First", "Oldest First", "A-Z", "Z-A" };

    [RelayCommand]
    private async Task LoadBorrowingsAsync()
    {
        try
        {
            IsLoading = true;
            IsError = false;
            IsEmpty = false;
            FeedbackMessage = string.Empty;

            var results = await _searchService.GetAllBorrowingsAsync();

            _allBorrowings = results.ToList();

            ApplyFilters();
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

    private void ApplyFilters()
    {
        IEnumerable<Borrowing> filtered = _allBorrowings;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            string search = SearchText.Trim().ToLower();

            filtered = filtered.Where(b =>
                b.Student.Name.ToLower().Contains(search) ||
                b.Equipment.Name.ToLower().Contains(search) ||
                b.BorrowingId.ToString().ToLower().Contains(search));
        }

        if (SelectedStatus == "Active")
        {
            filtered = filtered.Where(b => b.Status == BorrowingStatus.Active);
        }
        else if (SelectedStatus == "Returned")
        {
            filtered = filtered.Where(b => b.Status == BorrowingStatus.Returned);
        }

        filtered = SelectedSortField switch
        {
            "Student Name" => SelectedSortDirection == "A-Z"
                ? filtered.OrderBy(b => b.Student.Name)
                : filtered.OrderByDescending(b => b.Student.Name),

            "Equipment Name" => SelectedSortDirection == "A-Z"
                ? filtered.OrderBy(b => b.Equipment.Name)
                : filtered.OrderByDescending(b => b.Equipment.Name),

            _ => SelectedSortDirection == "Newest First"
                ? filtered.OrderByDescending(b => b.DateBorrowed)
                : filtered.OrderBy(b => b.DateBorrowed)
        };

        Borrowings.Clear();

        foreach (var borrowing in filtered)
        {
            Borrowings.Add(borrowing);
        }

        IsEmpty = Borrowings.Count == 0;

        FeedbackMessage = IsEmpty
            ? "No borrowing records found."
            : $"{Borrowings.Count} borrowing record(s) found.";
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnSelectedStatusChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnSelectedSortFieldChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnSelectedSortDirectionChanged(string value)
    {
        ApplyFilters();
    }

    [RelayCommand]
    private void ResetFilters()
    {
        SearchText = string.Empty;
        SelectedStatus = "All";
        SelectedSortField = "Date Borrowed";
        SelectedSortDirection = "Newest First";

        ApplyFilters();
    }
}