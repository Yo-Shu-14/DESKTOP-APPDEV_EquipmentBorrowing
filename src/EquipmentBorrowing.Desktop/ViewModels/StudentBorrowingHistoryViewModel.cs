using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class StudentBorrowingHistoryViewModel : ObservableObject
{
    private readonly ViewStudentBorrowingHistoryService _historyService;
    private readonly IStudentRepository _studentRepository;

    public ObservableCollection<Student> StudentList { get; } = new();

    public ObservableCollection<Borrowing> BorrowingHistory { get; } = new();

    [ObservableProperty]
    private Student? selectedStudent;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private bool isError;

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    public StudentBorrowingHistoryViewModel(
        ViewStudentBorrowingHistoryService historyService,
        IStudentRepository studentRepository)
    {
        _historyService = historyService;
        _studentRepository = studentRepository;
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
                var students = await _studentRepository.GetAllAsync();

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

            var history = await _historyService.GetStudentHistoryAsync(
                SelectedStudent.Id);

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