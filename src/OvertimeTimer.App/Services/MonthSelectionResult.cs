namespace OvertimeTimer.App.Services;

public sealed record MonthSelectionResult(DateOnly SelectedMonth, bool UseTodayAsSelectedDate);
