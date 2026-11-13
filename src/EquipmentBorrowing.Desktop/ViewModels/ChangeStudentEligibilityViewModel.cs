using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class ChangeStudentEligibilityViewModel : ObservableObject
{
    private readonly IStudentRepository _studentRepository;
    private readonly ChangeStudentEligibilityService _eligibilityService;

    public ObservableCollection<Student> Students { get; } = new();

    [ObservableProperty]
    private Student? selectedStudent;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isError;

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    public ChangeStudentEligibilityViewModel(
        IStudentRepository studentRepository,
        ChangeStudentEligibilityService eligibilityService)
    {
        _studentRepository = studentRepository;
        _eligibilityService = eligibilityService;
    }

    [RelayCommand]
    private async Task LoadStudentsAsync()
    {
        try
        {
            IsLoading = true;
            IsError = false;
            FeedbackMessage = string.Empty;

            var students = await _studentRepository.GetAllAsync();

            Students.Clear();

            foreach (var student in students)
                Students.Add(student);
        }
        catch
        {
            IsError = true;
            FeedbackMessage =
                "Unable to load students. Check the database connection.";
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

            var updatedStudent =
                await _eligibilityService.ChangeEligibilityAsync(
                    studentId,
                    isAllowed);

            await LoadStudentsAsync();

            if (!IsError)
            {
                SelectedStudent = Students.FirstOrDefault(
                    s => s.Id == updatedStudent.Id);

                IsError = false;
                FeedbackMessage = isAllowed
                    ? "Student is now permitted to borrow."
                    : "Student is now restricted from borrowing.";
            }
        }
        catch (InvalidOperationException ex)
        {
            IsError = true;
            FeedbackMessage = ex.Message;
        }
        catch
        {
            IsError = true;
            FeedbackMessage =
                "The change could not be confirmed. Refresh the student list before retrying.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}