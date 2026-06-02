using System.Collections.ObjectModel;
using Prism.Commands;
using System.Windows;
using OvertimeTimer.App.Services;

namespace OvertimeTimer.App.ViewModels;

public sealed class MonthPickerViewModel : ViewModelBase
{
    private int _selectedYear;
    private int _selectedMonth;

    public event EventHandler<MonthSelectionResult>? RequestClose;

    public MonthPickerViewModel(DateOnly currentMonth)
    {
        _selectedYear = currentMonth.Year;
        _selectedMonth = currentMonth.Month;
        YearOptions = new ObservableCollection<int>(Enumerable.Range(1900, 211));
        MonthOptions = new ObservableCollection<int>(Enumerable.Range(1, 12));
        TodayCommand = new DelegateCommand<Window>(GoToToday);
        ConfirmCommand = new DelegateCommand<Window>(Close);
    }

    public ObservableCollection<int> YearOptions { get; }

    public ObservableCollection<int> MonthOptions { get; }

    public int SelectedYear
    {
        get => _selectedYear;
        set
        {
            if (SetProperty(ref _selectedYear, value))
            {
                RaisePropertyChanged(nameof(SelectedMonthDate));
            }
        }
    }

    public int SelectedMonth
    {
        get => _selectedMonth;
        set
        {
            if (SetProperty(ref _selectedMonth, Math.Clamp(value, 1, 12)))
            {
                RaisePropertyChanged(nameof(SelectedMonthDate));
            }
        }
    }

    public DateOnly SelectedMonthDate => new(_selectedYear, _selectedMonth, 1);

    public DelegateCommand<Window> TodayCommand { get; }

    public DelegateCommand<Window> ConfirmCommand { get; }

    private void GoToToday(Window? window)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        SelectedYear = today.Year;
        SelectedMonth = today.Month;
        RequestClose?.Invoke(this, new MonthSelectionResult(today, true));
    }

    private void Close(Window? window)
    {
        if (window is null)
        {
            return;
        }

        RequestClose?.Invoke(this, new MonthSelectionResult(SelectedMonthDate, false));
    }
}
