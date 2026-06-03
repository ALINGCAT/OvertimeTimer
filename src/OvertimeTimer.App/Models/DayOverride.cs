namespace OvertimeTimer.App.Models;

public sealed class DayOverride
{
    public DateOnly Date { get; set; }

    public bool IsHoliday { get; set; }
}
