using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Desktop.ApiClient;
using System.Collections.ObjectModel;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class UpdateEquipmentInformationViewModel : ObservableObject
{
    private readonly EquipmentBorrowingApiClient _api;

    public ObservableCollection<EquipmentDto> EquipmentList { get; } = new();

    [ObservableProperty]
    private EquipmentDto? selectedEquipment;

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

    public UpdateEquipmentInformationViewModel(EquipmentBorrowingApiClient api)
    {
        _api = api;
    }

    [RelayCommand]
    private async Task LoadEquipmentAsync()
    {
        try
        {
            IsLoading = true;
            IsError = false;
            FeedbackMessage = string.Empty;

            var equipment = await _api.GetEquipmentAsync();

            EquipmentList.Clear();

            foreach (var item in equipment)
                EquipmentList.Add(item);
        }
        catch (ApiConnectionException)
        {
            IsError = true;
            FeedbackMessage = "Cannot reach the API server. Please ensure the API is running.";
        }
        catch (ApiServerException ex)
        {
            IsError = true;
            FeedbackMessage = $"Server error: {ex.Message}";
        }
        catch
        {
            IsError = true;
            FeedbackMessage = "Unable to load equipment. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedEquipmentChanged(EquipmentDto? value)
    {
        if (value is null)
            return;

        EquipmentIdentifier = value.EquipmentCode;
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

            Guid equipmentId = SelectedEquipment.Id;

            var updated = await _api.UpdateEquipmentAsync(
                equipmentId,
                new UpdateEquipmentRequest(
                    EquipmentIdentifier,
                    EquipmentName,
                    Description));

            await LoadEquipmentAsync();

            if (!IsError)
            {
                SelectedEquipment = EquipmentList.FirstOrDefault(
                    e => e.Id == equipmentId);

                IsError = false;
                FeedbackMessage = "Equipment information updated successfully.";
            }
        }
        catch (ApiConnectionException)
        {
            IsError = true;
            FeedbackMessage = "Cannot reach the API server. Please ensure the API is running.";
        }
        catch (ApiServerException ex)
        {
            IsError = true;
            FeedbackMessage = $"Server error: {ex.Message}";
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
