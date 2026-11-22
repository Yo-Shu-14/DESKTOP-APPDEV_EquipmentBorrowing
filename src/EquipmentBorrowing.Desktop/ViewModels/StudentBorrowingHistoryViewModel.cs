using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Desktop.ApiClient;
using System.Collections.ObjectModel;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class StudentBorrowingHistoryViewModel : ObservableObject
{
    private readonly EquipmentBorrowingApiClient _api;

    public ObservableCollection<StudentDto> StudentList { get; } = new();

    public ObservableCollection<BorrowingDto> BorrowingHistory { get; } = new();

    [ObservableProperty]
    private StudentDto? selectedStudent;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private bool isError;

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    public StudentBorrowingHistoryViewModel(EquipmentBorrowingApiClient api)
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

            if (StudentList.Count == 0)
            {
                var students = await _api.GetStudentsAsync();

                foreach (var student in students)
                {
                    StudentList.Add(student);
                }
            }

            if (SelectedStudent == null)
            {
                SelectedStudent = StudentList.FirstOrDefault();
            }

            if (SelectedStudent == null)
            {
                IsEmpty = true;
                FeedbackMessage = "No students registered yet.";
                BorrowingHistory.Clear();
                return;
            }

            var history = await _api.GetStudentBorrowingsAsync(SelectedStudent.Id);

            BorrowingHistory.Clear();

            foreach (var borrowing in history)
            {
                BorrowingHistory.Add(borrowing);
            }

            IsEmpty = BorrowingHistory.Count == 0;

            if (IsEmpty)
            {
                FeedbackMessage = "No borrowing history found for this student.";
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
            FeedbackMessage = "Unable to load borrowing history. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
