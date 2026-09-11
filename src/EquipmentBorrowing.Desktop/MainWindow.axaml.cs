using Avalonia.Controls;
using EquipmentBorrowing.Desktop.ViewModels;

namespace EquipmentBorrowing.Desktop;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}