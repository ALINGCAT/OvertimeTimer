using System.Collections.ObjectModel;

namespace OvertimeTimer.App.DesignTime;

public sealed class DesignMonthPickerViewModel
{
    public ObservableCollection<int> YearOptions { get; } = new(new[] { 2025, 2026, 2027 });

    public ObservableCollection<int> MonthOptions { get; } = new(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });

    public int SelectedYear => 2026;

    public int SelectedMonth => 6;

    public object TodayCommand { get; } = new();

    public object ConfirmCommand { get; } = new();
}
