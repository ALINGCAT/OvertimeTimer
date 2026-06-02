namespace OvertimeTimer.App.Services;

public interface IMonthSelectionDialogService
{
    MonthSelectionResult? Show(DateOnly currentMonth);
}
