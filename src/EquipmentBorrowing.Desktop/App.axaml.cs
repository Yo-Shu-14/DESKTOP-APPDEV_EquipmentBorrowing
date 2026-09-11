using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Desktop.ViewModels;
using EquipmentBorrowing.Domain;
using EquipmentBorrowing.Infrastructure;
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


            var studenet = new Student(
                1, "jang kaloy", true
                );

            var equipment1 = new Equipment(
                Guid.NewGuid(),
                "Laptop",
                "Lab Laptop",
                true
            );

            var equipment2 = new Equipment(
                Guid.NewGuid(),
                "Projector",
                "Epson Projector",
                true
            );

            var equipment3 = new Equipment(
                Guid.NewGuid(),
                "Camera",
                "Digital Camera",
                true
            );

            var equipment4 = new Equipment(
                Guid.NewGuid(),
                "Microphone",
                "Wireless Microphone",
                true
            );


            //repo 
            services.AddSingleton<IStudentRepository>(
                new InMemoryStudentRepository(new[] { studenet })
                );

            services.AddSingleton<IEquipmentRepository>(
                new InMemoryEquipmentRepository(new[]
                {
                    equipment1,
                    equipment2,
                    equipment3,
                    equipment4
                })
            );

            services.AddSingleton<IBorrowingRepository>(
                new InMemoryBorrowingRepository());


            //services
            services.AddTransient<BorrowEquipmentService>();
            services.AddTransient<ReturnEquipmentService>();
            services.AddTransient<CheckAvailableEquipmentService>();
            services.AddSingleton<EquipmentViewModel>();
            services.AddTransient<ActiveBorrowingsViewModel>();
            services.AddTransient<MainWindowViewModel>();

            var serviceProvider = services.BuildServiceProvider();

            var equipmentViewModel =
            serviceProvider.GetRequiredService<EquipmentViewModel>();

            await equipmentViewModel.LoadEquipmentCommand.ExecuteAsync(null);

            var mainWindowViewModel =
                serviceProvider.GetRequiredService<MainWindowViewModel>();

            desktop.MainWindow = new MainWindow(mainWindowViewModel);

            base.OnFrameworkInitializationCompleted();
        }

        base.OnFrameworkInitializationCompleted();
    }
}