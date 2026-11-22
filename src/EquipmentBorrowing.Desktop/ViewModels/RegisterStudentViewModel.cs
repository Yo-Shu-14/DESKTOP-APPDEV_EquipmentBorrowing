using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Desktop.ApiClient;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class RegisterStudentViewModel : ObservableObject
{
    private readonly EquipmentBorrowingApiClient _api;

    [ObservableProperty]
    private string institutionalNumber = string.Empty;

    [ObservableProperty]
    private string studentName = string.Empty;

    [ObservableProperty]
    private string contactInformation = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private bool isError;

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    public RegisterStudentViewModel(EquipmentBorrowingApiClient api)
    {
        _api = api;
    }

    [RelayCommand]
    private async Task RegisterStudentAsync()
    {
        try
        {
            IsLoading = true;
            IsError = false;
            FeedbackMessage = string.Empty;

            await _api.CreateStudentAsync(new CreateStudentRequest(
                InstitutionalNumber,
                StudentName,
                string.IsNullOrWhiteSpace(ContactInformation) ? null : ContactInformation));

            FeedbackMessage = "Student registered successfully.";

            InstitutionalNumber = string.Empty;
            StudentName = string.Empty;
            ContactInformation = string.Empty;
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
