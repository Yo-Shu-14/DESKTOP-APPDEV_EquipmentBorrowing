using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Desktop.ApiClient;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class RegisterEquipmentViewModel : ObservableObject
{
    private readonly EquipmentBorrowingApiClient _api;

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

    public RegisterEquipmentViewModel(EquipmentBorrowingApiClient api)
    {
        _api = api;
    }

    [RelayCommand]
    private async Task RegisterEquipmentAsync()
    {
        try
        {
            IsLoading = true;
            IsError = false;
            FeedbackMessage = string.Empty;

            await _api.CreateEquipmentAsync(new CreateEquipmentRequest(
                EquipmentIdentifier,
                EquipmentName,
                string.IsNullOrWhiteSpace(Description) ? null : Description));

            FeedbackMessage = "Equipment registered successfully.";

            EquipmentIdentifier = string.Empty;
            EquipmentName = string.Empty;
            Description = string.Empty;
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
