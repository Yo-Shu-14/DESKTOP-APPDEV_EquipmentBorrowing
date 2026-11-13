using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Desktop.Views;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private object? currentView;

    private readonly EquipmentViewModel _equipmentViewModel;
    private readonly ActiveBorrowingsViewModel _activeBorrowingsViewModel;
    private readonly StudentBorrowingHistoryViewModel _studentBorrowingHistoryViewModel;
    private readonly EquipmentBorrowingHistoryViewModel _equipmentBorrowingHistoryViewModel;
    private readonly SearchBorrowingsViewModel _searchBorrowingsViewModel;
    private readonly RegisterStudentViewModel _registerStudentViewModel;
    private readonly UpdateStudentInformationViewModel _updateStudentInformationViewModel;
    private readonly ChangeStudentEligibilityViewModel _changeStudentEligibilityViewModel;
    private readonly RegisterEquipmentViewModel _registerEquipmentViewModel;
    private readonly UpdateEquipmentInformationViewModel _updateEquipmentInformationViewModel;

    public MainWindowViewModel(
        EquipmentViewModel equipmentViewModel,
        ActiveBorrowingsViewModel activeBorrowingsViewModel,
        StudentBorrowingHistoryViewModel studentBorrowingHistoryViewModel,
        EquipmentBorrowingHistoryViewModel equipmentBorrowingHistoryViewModel,
        SearchBorrowingsViewModel searchBorrowingsViewModel,
        RegisterStudentViewModel registerStudentViewModel,
        UpdateStudentInformationViewModel updateStudentInformationViewModel,
        ChangeStudentEligibilityViewModel changeStudentEligibilityViewModel,
        RegisterEquipmentViewModel registerEquipmentViewModel,
        UpdateEquipmentInformationViewModel updateEquipmentInformationViewModel)
    {
        _equipmentViewModel = equipmentViewModel;
        _activeBorrowingsViewModel = activeBorrowingsViewModel;
        _studentBorrowingHistoryViewModel = studentBorrowingHistoryViewModel;
        _equipmentBorrowingHistoryViewModel = equipmentBorrowingHistoryViewModel;
        _searchBorrowingsViewModel = searchBorrowingsViewModel;
        _registerStudentViewModel = registerStudentViewModel;
        _updateStudentInformationViewModel = updateStudentInformationViewModel;
        _changeStudentEligibilityViewModel = changeStudentEligibilityViewModel;
        _registerEquipmentViewModel = registerEquipmentViewModel;
        _updateEquipmentInformationViewModel = updateEquipmentInformationViewModel;

        CurrentView = new EquipmentView
        {
            DataContext = _equipmentViewModel
        };
    }

    [RelayCommand]
    private async Task ShowEquipment()
    {
        await _equipmentViewModel.LoadEquipmentCommand.ExecuteAsync(null);

        CurrentView = new EquipmentView
        {
            DataContext = _equipmentViewModel
        };
    }

    [RelayCommand]
    private async Task ShowActiveBorrowings()
    {
        await _activeBorrowingsViewModel.LoadBorrowingsCommand.ExecuteAsync(null);

        CurrentView = new ActiveBorrowingsView
        {
            DataContext = _activeBorrowingsViewModel
        };
    }

    [RelayCommand]
    private async Task ShowStudentHistory()
    {
        CurrentView = new StudentBorrowingHistoryView
        {
            DataContext = _studentBorrowingHistoryViewModel
        };

        await _studentBorrowingHistoryViewModel.LoadHistoryCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task ShowEquipmentHistory()
    {
        CurrentView = new EquipmentBorrowingHistoryView
        {
            DataContext = _equipmentBorrowingHistoryViewModel
        };

        await _equipmentBorrowingHistoryViewModel.LoadHistoryCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task ShowSearchBorrowings()
    {
        CurrentView = new SearchBorrowingsView
        {
            DataContext = _searchBorrowingsViewModel
        };

        await _searchBorrowingsViewModel.LoadBorrowingsCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private Task ShowRegisterStudent()
    {
        CurrentView = new RegisterStudentView
        {
            DataContext = _registerStudentViewModel
        };

        return Task.CompletedTask;
    }
    [RelayCommand]
    private async Task ShowUpdateStudentInformation()
    {
        CurrentView = new UpdateStudentInformationView
        {
            DataContext = _updateStudentInformationViewModel
        };

        await _updateStudentInformationViewModel.LoadStudentsCommand.ExecuteAsync(null);
    }
    [RelayCommand]
    private async Task ShowChangeStudentEligibility()
    {
        CurrentView = new ChangeStudentEligibilityView
        {
            DataContext = _changeStudentEligibilityViewModel
        };

        await _changeStudentEligibilityViewModel.LoadStudentsCommand.ExecuteAsync(null);
    }
    [RelayCommand]
    private void ShowRegisterEquipment()
    {
        CurrentView = new RegisterEquipmentView
        {
            DataContext = _registerEquipmentViewModel
        };
    }
    [RelayCommand]
    private async Task ShowUpdateEquipment()
    {
        await _updateEquipmentInformationViewModel.LoadEquipmentCommand.ExecuteAsync(null);

        CurrentView = new UpdateEquipmentInformationView
        {
            DataContext = _updateEquipmentInformationViewModel
        };
    }
}