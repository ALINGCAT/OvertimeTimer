using OvertimeTimer.App.Views;

namespace OvertimeTimer.App.Services;

public sealed class MonthSelectionDialogService : IMonthSelectionDialogService
{
    public MonthSelectionResult? Show(DateOnly currentMonth)
    {
        var window = new MonthPickerWindow(currentMonth)
        {
            Owner = System.Windows.Application.Current?.MainWindow
        };

        if (window.ShowDialog() == true)
        {
            return new MonthSelectionResult(window.SelectedMonth, window.UseTodayAsSelectedDate);
        }

        return null;
    }
}
