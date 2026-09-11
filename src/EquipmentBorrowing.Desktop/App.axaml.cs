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


            var student1 = new Student(1, "jang kaloy", true);
            var student2 = new Student(2, "emji jid", false);


            var equipment1 = new Equipment(
                Guid.NewGuid(),"Laptop","Lab Laptop",true);

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
                new InMemoryStudentRepository(new[] { student1, student2 })
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
            services.AddTransient<EquipmentViewModel>();
            services.AddTransient<ActiveBorrowingsViewModel>();
            services.AddTransient<MainWindowViewModel>();

            var serviceProvider = services.BuildServiceProvider();

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