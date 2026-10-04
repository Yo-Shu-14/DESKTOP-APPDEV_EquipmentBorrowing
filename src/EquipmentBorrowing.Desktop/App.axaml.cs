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
            services.AddSingleton<EquipmentViewModel>();
            services.AddTransient<ActiveBorrowingsViewModel>();
            services.AddTransient<MainWindowViewModel>();

            var serviceProvider = services.BuildServiceProvider();

            var db = serviceProvider.GetRequiredService<EquipmentBorrowingDbContext>();

            await DatabaseInitializer.InitializeAsync(db);

            var equipmentViewModel = serviceProvider.GetRequiredService<EquipmentViewModel>();
            await equipmentViewModel.LoadEquipmentCommand.ExecuteAsync(null);

            var activeBorrowingsViewModel = serviceProvider.GetRequiredService<ActiveBorrowingsViewModel>();
            await activeBorrowingsViewModel.LoadBorrowingsCommand.ExecuteAsync(null);

            var mainWindowViewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();



            desktop.MainWindow = new MainWindow( mainWindowViewModel);
            
        }

        base.OnFrameworkInitializationCompleted();
    }
}