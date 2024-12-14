using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Kudo.Model;

namespace Kudo;

public class ViewModel : INotifyPropertyChanged

{
    private bool _isToggled;

    public bool IsToggled
    {
        get => _isToggled;
        set
        {
            if (_isToggled != value)
            {
                _isToggled = value;
                OnPropertyChanged(nameof(IsToggled));
            }
        }
    }
    public Customer CustomerNew { get; set; }
    public ObservableCollection<DataModel> Categories { get; set; } 
    public ObservableCollection<DataModel> Subcategories { get; set; }
    
    public ViewModel()
    {
        this.CustomerNew = new Customer();
        DataViewModel();
    }
    
    public void DataViewModel()
    {
        Categories = new ObservableCollection<DataModel>
        {
            new DataModel() { Name = "Продукты", ID = 0 },
            new DataModel() { Name = "Ценности", ID = 1 },
            new DataModel() { Name = "Поездки", ID = 2 },
            new DataModel() { Name = "Рестораны", ID = 3 },
            new DataModel() { Name = "Одежда", ID = 4 },
            new DataModel() { Name = "Мебель", ID = 5 },
            new DataModel() { Name = "Техника", ID = 6 },
        };
            
        Subcategories = new ObservableCollection<DataModel>
        {
            new DataModel() { Name = "Продукты", ID = 0 },
            new DataModel() { Name = "Ценности", ID = 1 },
            new DataModel() { Name = "Поездки", ID = 2 },
            new DataModel() { Name = "Рестораны", ID = 3 },
            new DataModel() { Name = "Одежда", ID = 4 },
            new DataModel() { Name = "Мебель", ID = 5 },
            new DataModel() { Name = "Техника", ID = 6 },
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}