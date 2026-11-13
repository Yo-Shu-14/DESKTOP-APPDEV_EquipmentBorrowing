using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EquipmentBorrowing.Application.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EquipmentBorrowing.Desktop.ViewModels;

public partial class RegisterStudentViewModel : ObservableObject
{
    private readonly RegisterStudentService _registerStudentService;

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

    public RegisterStudentViewModel(
        RegisterStudentService registerStudentService)
    {
        _registerStudentService = registerStudentService;
    }

    [RelayCommand]
    private async Task RegisterStudentAsync()
    {
        try
        {
            IsLoading = true;
            IsError = false;
            FeedbackMessage = string.Empty;

            await _registerStudentService.RegisterStudentAsync(
                InstitutionalNumber,
                StudentName,
                ContactInformation);

            FeedbackMessage = "Student registered successfully.";

            InstitutionalNumber = string.Empty;
            StudentName = string.Empty;
            ContactInformation = string.Empty;
        }
        catch (Exception ex)
        {
            IsError = true;
            FeedbackMessage = ex.ToString();
        }
        finally
        {
            IsLoading = false;
        }
    }
}