using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class RegisterEquipmentViewModel : ObservableObject
{
    private readonly RegisterEquipmentService _registerEquipmentService;

    [ObservableProperty]
    private string equipmentIdentifier = string.Empty;

    [ObservableProperty]
    private string equipmentName = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isError;

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    public RegisterEquipmentViewModel(
        RegisterEquipmentService registerEquipmentService)
    {
        _registerEquipmentService = registerEquipmentService;
    }

    [RelayCommand]
    private async Task RegisterEquipmentAsync()
    {
        try
        {
            IsLoading = true;
            IsError = false;
            FeedbackMessage = string.Empty;

            await _registerEquipmentService.RegisterEquipmentAsync(
                EquipmentIdentifier,
                EquipmentName,
                Description);

            FeedbackMessage = "Equipment registered successfully.";

            EquipmentIdentifier = string.Empty;
            EquipmentName = string.Empty;
            Description = string.Empty;
        }
        catch (Exception ex)
        {
            IsError = true;
            FeedbackMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}