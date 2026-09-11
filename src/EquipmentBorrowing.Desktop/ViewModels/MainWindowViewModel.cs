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



    public MainWindowViewModel(
        EquipmentViewModel equipmentViewModel,
        ActiveBorrowingsViewModel activeBorrowingsViewModel)
    {
        _equipmentViewModel = equipmentViewModel;
        _activeBorrowingsViewModel = activeBorrowingsViewModel;

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

}