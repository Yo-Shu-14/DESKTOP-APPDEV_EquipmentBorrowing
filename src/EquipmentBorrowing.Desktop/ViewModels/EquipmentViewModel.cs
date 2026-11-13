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
    private readonly CheckAvailableEquipmentService _checkAvailableEquipmentService;
    private readonly BorrowEquipmentService _borrowEquipmentService;
    private readonly IStudentRepository _studentRepository;

    public ObservableCollection<Equipment> EquipmentList { get; } = new();
    public ObservableCollection<Student> StudentList { get; } = new();

    public ObservableCollection<Equipment> SelectedEquipmentList { get; } = new();

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
    CheckAvailableEquipmentService checkAvailableEquipmentService,
    BorrowEquipmentService borrowEquipmentService,
    IStudentRepository studentRepository)
    {
        _viewEquipmentCatalogService = viewEquipmentCatalogService;
        _checkAvailableEquipmentService = checkAvailableEquipmentService;
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

    [RelayCommand]
    private async Task LoadAvailableEquipmentAsync()
    {
        try
        {
            IsLoading = true;
            IsError = false;
            IsEmpty = false;
            FeedbackMessage = string.Empty;

            var equipment = await _checkAvailableEquipmentService
                .CheckAvailableEquipmentAsync();

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
        catch (Exception)
        {
            IsError = true;
            IsEmpty = false;
            FeedbackMessage = "Unable to find available equipment. Please try again.";
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
                DateTime.SpecifyKind(
                    ExpectedReturnDate.Value.Date,
                    DateTimeKind.Local)
            );
            await LoadEquipmentAsync();
            FeedbackMessage = "Equipment borrowed successfully.";

        }
        catch (Exception ex)
        {
            FeedbackMessage = ex.ToString();
        }



    }

    [RelayCommand]
    private void AddToCheckout(Equipment equipment)
    {
        if (!equipment.IsAvailable)
        {
            FeedbackMessage = "This equipment is not available.";
            return;
        }

        if (SelectedEquipmentList.Any(
            e => e.EquipmentId == equipment.EquipmentId))
        {
            FeedbackMessage = "Equipment is already in your checkout list.";
            return;
        }

        SelectedEquipmentList.Add(equipment);

        FeedbackMessage = $"{equipment.Name} added to checkout.";
    }

    [RelayCommand]
    private void RemoveFromCheckout(Equipment equipment)
    {
        var selectedItem = SelectedEquipmentList.FirstOrDefault(
            e => e.EquipmentId == equipment.EquipmentId);

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
                .Select(e => e.EquipmentId)
                .ToList();

            await _borrowEquipmentService.BorrowMultipleEquipmentAsync(
                SelectedStudent.Id,
                equipmentIds,
                3,
                DateTime.SpecifyKind(
                    ExpectedReturnDate.Value.Date,
                    DateTimeKind.Local));

            SelectedEquipmentList.Clear();

            await LoadEquipmentAsync();

            if (!IsError)
            {
                FeedbackMessage = "All selected equipment borrowed successfully.";
            }
        }
        catch (Exception ex)
        {
            FeedbackMessage = ex.Message;
        }
    }

}

