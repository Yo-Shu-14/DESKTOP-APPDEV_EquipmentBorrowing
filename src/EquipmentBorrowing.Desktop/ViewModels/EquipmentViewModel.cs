using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using System.Collections.ObjectModel;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentViewModel : ObservableObject
{
    private readonly CheckAvailableEquipmentService _checkAvailableEquipmentService;
    private readonly BorrowEquipmentService _borrowEquipmentService;
    private readonly IStudentRepository _studentRepository;

    public ObservableCollection<Equipment> EquipmentList { get; } = new();
    public ObservableCollection<Student> StudentList { get; } = new();

    [ObservableProperty]
    private Student? selectedStudent;



    //
    public int SelectedId { get; set; } = 1;
    public Equipment? SelectedEquipment { get; set; }
    public DateTimeOffset? ExpectedReturnDate { get; set; } = DateTimeOffset.Now.AddDays(7);

    [ObservableProperty]
    public string feedbackMessage = string.Empty;


    public EquipmentViewModel(CheckAvailableEquipmentService checkAvailableEquipmentService, BorrowEquipmentService borrowEquipmentService, IStudentRepository studentRepository)
    {
        _checkAvailableEquipmentService = checkAvailableEquipmentService;
        _borrowEquipmentService = borrowEquipmentService;
        _studentRepository = studentRepository;
    }



    [RelayCommand]
    private async Task LoadEquipmentAsync() 
    {
        var equipment  = await _checkAvailableEquipmentService.CheckAvailableEquipmentAsync();
        EquipmentList.Clear();
        foreach (var item in equipment)
        {
            EquipmentList.Add(item);
        }

        if (StudentList.Count == 0)
        {
            var students = await _studentRepository.GetAllAsync();
            foreach (var s in students)
                StudentList.Add(s);
            SelectedStudent = StudentList.FirstOrDefault();
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
                SelectedId,
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

