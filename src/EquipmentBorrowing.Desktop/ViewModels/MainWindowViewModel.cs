using System;
using System.Collections.Generic;
using System.Text;
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

    public MainWindowViewModel(
        EquipmentViewModel equipmentViewModel,
        ActiveBorrowingsViewModel activeBorrowingsViewModel,
        StudentBorrowingHistoryViewModel studentBorrowingHistoryViewModel,
        EquipmentBorrowingHistoryViewModel equipmentBorrowingHistoryViewModel,
        SearchBorrowingsViewModel searchBorrowingsViewModel)
        {
            _equipmentBorrowingHistoryViewModel = equipmentBorrowingHistoryViewModel;
            _equipmentViewModel = equipmentViewModel;
            _activeBorrowingsViewModel = activeBorrowingsViewModel;
            _studentBorrowingHistoryViewModel = studentBorrowingHistoryViewModel;
            _searchBorrowingsViewModel = searchBorrowingsViewModel;

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

}