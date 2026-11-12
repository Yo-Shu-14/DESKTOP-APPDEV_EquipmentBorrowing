using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentViewModel : ObservableObject
{
    private readonly ViewEquipmentCatalogService _viewEquipmentCatalogService;
    private readonly BorrowEquipmentService _borrowEquipmentService;
    private readonly IStudentRepository _studentRepository;

    public ObservableCollection<Equipment> EquipmentList { get; } = new();
    public ObservableCollection<Student> StudentList { get; } = new();

    [ObservableProperty]
    private Student? selectedStudent;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isEmpty;

    [ObservableProperty]
    private bool isError;


    //
    public Equipment? SelectedEquipment { get; set; }
    public DateTimeOffset? ExpectedReturnDate { get; set; } = DateTimeOffset.Now.AddDays(7);

    [ObservableProperty]
    public string feedbackMessage = string.Empty;


    public EquipmentViewModel(
    ViewEquipmentCatalogService viewEquipmentCatalogService,
    BorrowEquipmentService borrowEquipmentService,
    IStudentRepository studentRepository)
    {
        _viewEquipmentCatalogService = viewEquipmentCatalogService;
        _borrowEquipmentService = borrowEquipmentService;
        _studentRepository = studentRepository;
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

            var equipment = await _viewEquipmentCatalogService
                .ViewEquipmentCatalogAsync();

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
                var students = await _studentRepository.GetAllAsync();

                foreach (var student in students)
                {
                    StudentList.Add(student);
                }

                SelectedStudent = StudentList.FirstOrDefault();
            }
        }
        catch (Exception)
        {
            IsError = true;
            IsEmpty = false;
            FeedbackMessage = "Unable to load equipment. Please check your connection and try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }



    // borrow com
    [RelayCommand]
    private async Task BorrowAsync(Equipment equipment)
    {
        SelectedEquipment = equipment;

        if (StudentList.Count == 0)
        {
            var students = await _studentRepository.GetAllAsync();
            foreach (var s in students)
                StudentList.Add(s);
            SelectedStudent = StudentList.FirstOrDefault();
        }

        if (ExpectedReturnDate == null)
        {
            FeedbackMessage = "Please select an expected return date!";
            return;
        }

        if (ExpectedReturnDate.Value.Date < DateTimeOffset.Now.Date)
        {
            FeedbackMessage = "Expected return date cannot be in the past.";
            return;
        }

        try
        {
            await _borrowEquipmentService.BorrowEquipmentAsync(
                SelectedStudent.Id,
                SelectedEquipment.EquipmentId,
                3,
                ExpectedReturnDate.Value.DateTime
                );

            FeedbackMessage = "Successful Borrowing";

            await LoadEquipmentAsync();

        }
        catch (Exception ex) {
            FeedbackMessage = ex.Message;
        }
           

        
    }
}

