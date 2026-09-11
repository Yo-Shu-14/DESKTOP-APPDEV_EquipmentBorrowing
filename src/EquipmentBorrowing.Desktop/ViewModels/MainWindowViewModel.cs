using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _currentView = "Equipment";

    public string CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    [RelayCommand]
    private void ShowEquipment()
    {
        CurrentView = "Equipment";
    }

    [RelayCommand]
    private void ShowActiveBorrowings()
    {
        CurrentView = "ActiveBorrowings";
    }
}