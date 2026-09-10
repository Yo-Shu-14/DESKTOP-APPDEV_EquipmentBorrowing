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

            var equipment = new Equipment(
                Guid.NewGuid(), "Laptop","Lab Laptop" , true
                );


            //repo 
            services.AddSingleton<IStudentRepository>(
                new InMemoryStudentRepository(new[] { studenet })
                );

            services.AddSingleton<IEquipmentRepository>(
                new InMemoryEquipmentRepository(new[] { equipment })
                );

            services.AddSingleton<IBorrowingRepository>(
                new InMemoryBorrowingRepository());


            //services
            services.AddTransient<BorrowEquipmentService>();
            services.AddTransient<ReturnEquipmentService>();
            services.AddTransient<CheckAvailableEquipmentService>();
            services.AddTransient<EquipmentViewModel>();

            var serviceProvider = services.BuildServiceProvider();

            var equipmentViewModel = serviceProvider.GetRequiredService<EquipmentViewModel>();
            await equipmentViewModel.LoadEquipmentCommand.ExecuteAsync(null);
            desktop.MainWindow = new MainWindow(equipmentViewModel);

            base.OnFrameworkInitializationCompleted();
        }

        base.OnFrameworkInitializationCompleted();
    }
}