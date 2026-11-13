using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class UpdateStudentInformationViewModel : ObservableObject
{
    private readonly IStudentRepository _studentRepository;
    private readonly UpdateStudentInformationService _updateStudentInformationService;

    public ObservableCollection<Student> Students { get; } = new();

    [ObservableProperty]
    private Student? selectedStudent;

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

    public UpdateStudentInformationViewModel(
        IStudentRepository studentRepository,
        UpdateStudentInformationService updateStudentInformationService)
    {
        _studentRepository = studentRepository;
        _updateStudentInformationService = updateStudentInformationService;
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
            FeedbackMessage = "Unable to load students. Check the database connection.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedStudentChanged(Student? value)
    {
        if (value is null)
            return;

        InstitutionalNumber = value.InstitutionalNumber;
        StudentName = value.Name;
        ContactInformation = value.ContactInformation;
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

            var updatedStudent =
                await _updateStudentInformationService.UpdateStudentAsync(
                    SelectedStudent.Id,
                    InstitutionalNumber,
                    StudentName,
                    ContactInformation);

            await LoadStudentsAsync();

            if (!IsError)
            {
                SelectedStudent = Students.FirstOrDefault(
                    s => s.Id == updatedStudent.Id);

                IsError = false;
                FeedbackMessage = "Student information updated successfully.";
            }
        }
        catch (ArgumentException ex)
        {
            IsError = true;
            FeedbackMessage = ex.Message;
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
                "The update could not be confirmed. Refresh the student list before retrying.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}