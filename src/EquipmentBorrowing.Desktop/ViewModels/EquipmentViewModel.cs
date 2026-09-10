using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class EquipmentViewModel : ObservableObject
{
    private readonly CheckAvailableEquipmentService _checkAvailableEquipmentService;

    public ObservableCollection<Equipment> EquipmentList { get; } = new();

  



    public EquipmentViewModel(CheckAvailableEquipmentService checkAvailableEquipmentService)
    {
        _checkAvailableEquipmentService = checkAvailableEquipmentService;
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

    }
}

