using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Domain;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class UpdateEquipmentInformationViewModel : ObservableObject
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly UpdateEquipmentInformationService _updateEquipmentInformationService;

    public ObservableCollection<Equipment> EquipmentList { get; } = new();

    [ObservableProperty]
    private Equipment? selectedEquipment;

    [ObservableProperty]
    private string equipmentIdentifier = string.Empty;

    [ObservableProperty]
    private string equipmentName = string.Empty;

    [ObservableProperty]
    private string? description;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isError;

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    public UpdateEquipmentInformationViewModel(
        IEquipmentRepository equipmentRepository,
        UpdateEquipmentInformationService updateEquipmentInformationService)
    {
        _equipmentRepository = equipmentRepository;
        _updateEquipmentInformationService = updateEquipmentInformationService;
    }

    [RelayCommand]
    private async Task LoadEquipmentAsync()
    {
        try
        {
            IsLoading = true;
            IsError = false;
            FeedbackMessage = string.Empty;

            var equipment = await _equipmentRepository.GetAllAsync();

            EquipmentList.Clear();

            foreach (var item in equipment)
                EquipmentList.Add(item);
        }
        catch
        {
            IsError = true;
            FeedbackMessage = "Unable to load equipment. Check the database connection.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedEquipmentChanged(Equipment? value)
    {
        if (value is null)
            return;

        EquipmentIdentifier = value.EquipmentIdentifier;
        EquipmentName = value.Name;
        Description = value.Description;
        FeedbackMessage = string.Empty;
        IsError = false;
    }

    [RelayCommand]
    private async Task UpdateEquipmentAsync()
    {
        if (SelectedEquipment is null)
        {
            IsError = true;
            FeedbackMessage = "Please select equipment.";
            return;
        }

        try
        {
            IsLoading = true;
            IsError = false;
            FeedbackMessage = string.Empty;

            Guid equipmentId = SelectedEquipment.EquipmentId;

            await _updateEquipmentInformationService.UpdateEquipmentInformationAsync(
                equipmentId,
                EquipmentIdentifier,
                EquipmentName,
                Description);

            await LoadEquipmentAsync();

            if (!IsError)
            {
                SelectedEquipment = EquipmentList.FirstOrDefault(
                    e => e.EquipmentId == equipmentId);

                IsError = false;
                FeedbackMessage = "Equipment information updated successfully.";
            }
        }
        catch (ArgumentException ex)
        {
            IsError = true;
            FeedbackMessage = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            IsError = true;
            FeedbackMessage = ex.Message;
        }
        catch
        {
            IsError = true;
            FeedbackMessage =
                "The update could not be confirmed. Refresh the equipment list before retrying.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}