using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Desktop.ApiClient;
using System.Collections.ObjectModel;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentViewModel : ObservableObject
{
    private readonly EquipmentBorrowingApiClient _api;

    public ObservableCollection<EquipmentDto> EquipmentList { get; } = new();
    public ObservableCollection<StudentDto> StudentList { get; } = new();
    public ObservableCollection<EquipmentDto> SelectedEquipmentList { get; } = new();

    [ObservableProperty]
    private StudentDto? selectedStudent;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private bool isError;

    public EquipmentDto? SelectedEquipment { get; set; }
    public DateTimeOffset? ExpectedReturnDate { get; set; } = DateTimeOffset.Now.AddDays(7);

    [ObservableProperty]
    public string feedbackMessage = string.Empty;

    public EquipmentViewModel(EquipmentBorrowingApiClient api)
    {
        _api = api;
    }

    [RelayCommand]
    private async Task LoadEquipmentAsync()
    {
        try
        {
            IsError = false;
            IsLoading = true;
            IsEmpty = false;
            FeedbackMessage = string.Empty;

            var equipment = await _api.GetEquipmentAsync();

            EquipmentList.Clear();
            foreach (var item in equipment)
            {
                EquipmentList.Add(item);
            }

            IsEmpty = EquipmentList.Count == 0;
            if (IsEmpty)
            {
                FeedbackMessage = "No equipment registered yet.";
            }

            if (StudentList.Count == 0)
            {
                var students = await _api.GetStudentsAsync();
                foreach (var student in students)
                {
                    StudentList.Add(student);
                }
                SelectedStudent = StudentList.FirstOrDefault();
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
        catch (Exception ex)
        {
            IsError = true;
            IsEmpty = false;
            FeedbackMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LoadAvailableEquipmentAsync()
    {
        try
        {
            IsLoading = true;
            IsError = false;
            IsEmpty = false;
            FeedbackMessage = string.Empty;

            var equipment = await _api.GetEquipmentAsync(availableOnly: true);

            EquipmentList.Clear();
            foreach (var item in equipment)
            {
                EquipmentList.Add(item);
            }

            IsEmpty = EquipmentList.Count == 0;
            if (IsEmpty)
            {
                FeedbackMessage = "No available equipment found.";
            }
        }
        catch (ApiConnectionException)
        {
            IsError = true;
            IsEmpty = false;
            FeedbackMessage = "Cannot reach the API server. Please ensure the API is running.";
        }
        catch (Exception ex)
        {
            IsError = true;
            IsEmpty = false;
            FeedbackMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task BorrowAsync(EquipmentDto equipment)
    {
        SelectedEquipment = equipment;

        if (StudentList.Count == 0)
        {
            var students = await _api.GetStudentsAsync();
            foreach (var s in students)
                StudentList.Add(s);
            SelectedStudent = StudentList.FirstOrDefault();
        }

        if (SelectedStudent == null)
        {
            FeedbackMessage = "Please select a student.";
            return;
        }

        if (ExpectedReturnDate == null)
        {
            FeedbackMessage = "Please select an expected return date!";
            return;
        }

        if (ExpectedReturnDate.Value.Date <= DateTimeOffset.Now.Date)
        {
            FeedbackMessage = "Expected return date must be in the future.";
            return;
        }

        try
        {
            await _api.CreateBorrowingAsync(new CreateBorrowingRequest(
                SelectedStudent.Id,
                SelectedEquipment.Id,
                ExpectedReturnDate.Value.Date.ToString("yyyy-MM-dd")));

            await LoadEquipmentAsync();
            FeedbackMessage = "Equipment borrowed successfully.";
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

    [RelayCommand]
    private void AddToCheckout(EquipmentDto equipment)
    {
        if (!equipment.IsAvailable)
        {
            FeedbackMessage = "This equipment is not available.";
            return;
        }

        if (SelectedEquipmentList.Any(e => e.Id == equipment.Id))
        {
            FeedbackMessage = "Equipment is already in your checkout list.";
            return;
        }

        SelectedEquipmentList.Add(equipment);
        FeedbackMessage = $"{equipment.Name} added to checkout.";
    }

    [RelayCommand]
    private void RemoveFromCheckout(EquipmentDto equipment)
    {
        var selectedItem = SelectedEquipmentList.FirstOrDefault(e => e.Id == equipment.Id);
        if (selectedItem != null)
        {
            SelectedEquipmentList.Remove(selectedItem);
            FeedbackMessage = $"{equipment.Name} removed from checkout.";
        }
    }

    [RelayCommand]
    private async Task CheckoutAsync()
    {
        if (SelectedStudent == null)
        {
            FeedbackMessage = "Please select a student.";
            return;
        }

        if (SelectedEquipmentList.Count < 2)
        {
            FeedbackMessage = "Please select at least two equipment items.";
            return;
        }

        if (ExpectedReturnDate == null)
        {
            FeedbackMessage = "Please select an expected return date.";
            return;
        }

        if (ExpectedReturnDate.Value.Date <= DateTimeOffset.Now.Date)
        {
            FeedbackMessage = "Expected return date must be in the future.";
            return;
        }

        try
        {
            var equipmentIds = SelectedEquipmentList
                .Select(e => e.Id)
                .ToList();

            await _api.CheckoutMultipleAsync(new CheckoutRequest(
                SelectedStudent.Id,
                equipmentIds,
                ExpectedReturnDate.Value.Date.ToString("yyyy-MM-dd")));

            SelectedEquipmentList.Clear();
            await LoadEquipmentAsync();

            if (!IsError)
            {
                FeedbackMessage = "All selected equipment borrowed successfully.";
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
