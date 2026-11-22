using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Desktop.ApiClient;
using System.Collections.ObjectModel;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class UpdateStudentInformationViewModel : ObservableObject
{
    private readonly EquipmentBorrowingApiClient _api;

    public ObservableCollection<StudentDto> Students { get; } = new();

    [ObservableProperty]
    private StudentDto? selectedStudent;

    [ObservableProperty]
    private string institutionalNumber = string.Empty;

    [ObservableProperty]
    private string studentName = string.Empty;

    [ObservableProperty]
    private string? contactInformation;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isError;

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    public UpdateStudentInformationViewModel(EquipmentBorrowingApiClient api)
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

    partial void OnSelectedStudentChanged(StudentDto? value)
    {
        if (value is null)
            return;

        InstitutionalNumber = value.StudentNumber;
        StudentName = value.FullName;
        ContactInformation = value.Contact;
        FeedbackMessage = string.Empty;
        IsError = false;
    }

    [RelayCommand]
    private async Task UpdateStudentAsync()
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

            var updated = await _api.UpdateStudentAsync(
                studentId,
                new UpdateStudentRequest(
                    InstitutionalNumber,
                    StudentName,
                    ContactInformation));

            await LoadStudentsAsync();

            if (!IsError)
            {
                SelectedStudent = Students.FirstOrDefault(s => s.Id == updated.Id);
                IsError = false;
                FeedbackMessage = "Student information updated successfully.";
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
