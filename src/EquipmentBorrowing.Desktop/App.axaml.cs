using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
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
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();

            services.AddDatabase();

            services.AddScoped<IStudentRepository, EfStudentRepository>();
            services.AddScoped<IEquipmentRepository, EfEquipmentRepository>();
            services.AddScoped<IBorrowingRepository, EfBorrowingRepository>();

            //services
            services.AddTransient<BorrowEquipmentService>();
            services.AddTransient<ReturnEquipmentService>();
            services.AddTransient<CheckAvailableEquipmentService>();
            services.AddTransient<EquipmentViewModel>();
            services.AddTransient<ActiveBorrowingsViewModel>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<ViewEquipmentCatalogService>();
            services.AddTransient<ViewStudentBorrowingHistoryService>();
            services.AddTransient<StudentBorrowingHistoryViewModel>();
            services.AddTransient<ViewEquipmentBorrowingHistoryService>();
            services.AddTransient<EquipmentBorrowingHistoryViewModel>();
            services.AddTransient<SearchBorrowingsService>();
            services.AddTransient<SearchBorrowingsViewModel>();
            services.AddTransient<RegisterStudentService>();
            services.AddTransient<RegisterStudentViewModel>();
            services.AddTransient<UpdateStudentInformationService>();
            services.AddTransient<UpdateStudentInformationViewModel>();
            services.AddTransient<ChangeStudentEligibilityService>();
            services.AddTransient<ChangeStudentEligibilityViewModel>();
            services.AddTransient<RegisterEquipmentService>();
            services.AddTransient<RegisterEquipmentViewModel>();
            services.AddTransient<UpdateEquipmentInformationService>();
            services.AddTransient<UpdateEquipmentInformationViewModel>();


            var serviceProvider = services.BuildServiceProvider();

            var mainWindowViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();

            desktop.MainWindow = new MainWindow(mainWindowViewModel);

            try
            {
                var db = serviceProvider.GetRequiredService<EquipmentBorrowingDbContext>();
                await DatabaseInitializer.InitializeAsync(db);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DatabaseInitializer Warning]: {ex.Message}");
                // Database may be offline or unreachable; views handle error and retry states per UC-01/UC-02/BR-18
            }

            var equipmentViewModel = serviceProvider.GetRequiredService<EquipmentViewModel>();
            await equipmentViewModel.LoadEquipmentCommand.ExecuteAsync(null);

        }

        base.OnFrameworkInitializationCompleted();
    }
}