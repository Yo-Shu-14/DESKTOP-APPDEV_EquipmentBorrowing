using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Desktop.ViewModels;

public class ActiveBorrowingsViewModel
{
    public ObservableCollection<BorrowingItem> ActiveBorrowings { get; }

    public ICommand ReturnCommand { get; }

    public ActiveBorrowingsViewModel()
    {
        ActiveBorrowings = new ObservableCollection<BorrowingItem>
        {
            new BorrowingItem
            {
                Id = 1,
                EquipmentName = "Laptop",
                BorrowerName = "Juan Dela Cruz",
                BorrowedAt = DateTime.Now
            },

            new BorrowingItem
            {
                Id = 2,
                EquipmentName = "Projector",
                BorrowerName = "Maria Santos",
                BorrowedAt = DateTime.Now
            }
        };

        ReturnCommand = new RelayCommand(Return);
    }

    private void Return(object? parameter)
    {
        if (parameter is not BorrowingItem borrowing)
            return;

        ActiveBorrowings.Remove(borrowing);
    }
}

public class BorrowingItem
{
    public int Id { get; set; }

    public string EquipmentName { get; set; } = "";

    public string BorrowerName { get; set; } = "";

    public DateTime BorrowedAt { get; set; }
}