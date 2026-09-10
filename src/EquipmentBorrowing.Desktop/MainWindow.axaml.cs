using Avalonia.Controls;
using EquipmentBorrowing.Desktop.ViewModels;

namespace EquipmentBorrowing.Desktop;

public partial class MainWindow : Window
{
    public MainWindow(EquipmentViewModel equipmentViewModel)
    {
        InitializeComponent();

        DataContext = equipmentViewModel;
    }
}