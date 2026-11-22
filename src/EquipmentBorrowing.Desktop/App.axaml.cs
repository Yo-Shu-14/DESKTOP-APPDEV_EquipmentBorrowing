using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using EquipmentBorrowing.Desktop.ApiClient;
using EquipmentBorrowing.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace EquipmentBorrowing.Desktop;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        // Force application to use Light Theme
        RequestedThemeVariant = ThemeVariant.Light;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();

            // ─── API Client ────────────────────────────────────────────────────────
            var apiBaseUrl = Environment.GetEnvironmentVariable("EQUIPMENT_BORROWING_API_URL")
                ?? "http://localhost:5000/";

            services.AddHttpClient<EquipmentBorrowingApiClient>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // ─── ViewModels ────────────────────────────────────────────────────────
            services.AddSingleton<EquipmentViewModel>();
            services.AddTransient<ActiveBorrowingsViewModel>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<StudentBorrowingHistoryViewModel>();
            services.AddTransient<EquipmentBorrowingHistoryViewModel>();
            services.AddTransient<SearchBorrowingsViewModel>();
            services.AddTransient<RegisterStudentViewModel>();
            services.AddTransient<UpdateStudentInformationViewModel>();
            services.AddTransient<ChangeStudentEligibilityViewModel>();
            services.AddTransient<RegisterEquipmentViewModel>();
            services.AddTransient<UpdateEquipmentInformationViewModel>();

            var serviceProvider = services.BuildServiceProvider();

            var mainWindowViewModel =
                serviceProvider.GetRequiredService<MainWindowViewModel>();

            desktop.MainWindow = new MainWindow(mainWindowViewModel);

            var equipmentViewModel =
                serviceProvider.GetRequiredService<EquipmentViewModel>();

            await equipmentViewModel.LoadEquipmentCommand.ExecuteAsync(null);
        }

        base.OnFrameworkInitializationCompleted();
    }
}
