using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Desktop.ApiClient;
using EquipmentBorrowing.Desktop.ViewModels;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure.Data;
using EquipmentBorrowing.Infrastructure.Repositories;
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

            // Services & Repositories for remaining unmigrated views
            services.AddDatabase();
            services.AddScoped<IStudentRepository, EfStudentRepository>();
            services.AddScoped<IEquipmentRepository, EfEquipmentRepository>();
            services.AddScoped<IBorrowingRepository, EfBorrowingRepository>();
            services.AddTransient<ViewStudentBorrowingHistoryService>();
            services.AddTransient<ViewEquipmentBorrowingHistoryService>();
            services.AddTransient<SearchBorrowingsService>();
            services.AddTransient<RegisterStudentService>();
            services.AddTransient<UpdateStudentInformationService>();
            services.AddTransient<ChangeStudentEligibilityService>();
            services.AddTransient<RegisterEquipmentService>();
            services.AddTransient<UpdateEquipmentInformationService>();

            // ViewModels
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
