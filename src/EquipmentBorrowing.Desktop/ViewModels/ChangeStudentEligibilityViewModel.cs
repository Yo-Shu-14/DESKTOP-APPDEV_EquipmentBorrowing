using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Desktop.ApiClient;
using System.Collections.ObjectModel;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class ChangeStudentEligibilityViewModel : ObservableObject
{
    private readonly EquipmentBorrowingApiClient _api;

    public ObservableCollection<StudentDto> Students { get; } = new();

    [ObservableProperty]
    private StudentDto? selectedStudent;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isError;

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    public ChangeStudentEligibilityViewModel(EquipmentBorrowingApiClient api)
    {
        _api = api;
    }

    [RelayCommand]
    private async Task LoadStudentsAsync()
    {
        try
        {
            IsLoading = true;
            IsError = false;
            FeedbackMessage = string.Empty;

            var students = await _api.GetStudentsAsync();

            Students.Clear();

            foreach (var student in students)
                Students.Add(student);
        }
        catch (ApiConnectionException)
        {
            IsError = true;
            FeedbackMessage = "Cannot reach the API server. Please ensure the API is running.";
        }
        catch (ApiServerException ex)
        {
            IsError = true;
            FeedbackMessage = $"Server error: {ex.Message}";
        }
        catch
        {
            IsError = true;
            FeedbackMessage = "Unable to load students. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private Task PermitStudentAsync()
    {
        return ChangeEligibilityAsync(true);
    }

    [RelayCommand]
    private Task RestrictStudentAsync()
    {
        return ChangeEligibilityAsync(false);
    }

    private async Task ChangeEligibilityAsync(bool isAllowed)
    {
        if (SelectedStudent is null)
        {
            IsError = true;
            FeedbackMessage = "Please select a student.";
            return;
        }

        try
        {
            IsLoading = true;
            IsError = false;
            FeedbackMessage = string.Empty;

            int studentId = SelectedStudent.Id;

            var updatedStudent = await _api.ChangeEligibilityAsync(studentId, isAllowed);

            await LoadStudentsAsync();

            if (!IsError)
            {
                SelectedStudent = Students.FirstOrDefault(s => s.Id == updatedStudent.Id);

                IsError = false;
                FeedbackMessage = isAllowed
                    ? "Student is now permitted to borrow."
                    : "Student is now restricted from borrowing.";
            }
        }
        catch (ApiConnectionException)
        {
            IsError = true;
            FeedbackMessage = "Cannot reach the API server. Please ensure the API is running.";
        }
        catch (ApiServerException ex)
        {
            IsError = true;
            FeedbackMessage = $"Server error: {ex.Message}";
        }
        catch (Exception ex)
        {
            IsError = true;
            FeedbackMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
