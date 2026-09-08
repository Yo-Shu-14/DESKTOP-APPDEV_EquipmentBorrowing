using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Desktop.ViewModels;

public class EquipmentViewModel
{
    public ObservableCollection<EquipmentItem> EquipmentList { get; }

    public ICommand BorrowCommand { get; }

    public EquipmentViewModel()
    {
        EquipmentList = new ObservableCollection<EquipmentItem>
        {
            new EquipmentItem
            {
                Id = 1,
                Name = "Laptop",
                IsAvailable = true
            },

            new EquipmentItem
            {
                Id = 2,
                Name = "Projector",
                IsAvailable = true
            },

            new EquipmentItem
            {
                Id = 3,
                Name = "Camera",
                IsAvailable = false
            }
        };

        BorrowCommand = new RelayCommand(Borrow);
    }

    private void Borrow(object? parameter)
    {
        if (parameter is not EquipmentItem equipment)
            return;

        equipment.IsAvailable = false;
    }
}

public class EquipmentItem
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public bool IsAvailable { get; set; }
}