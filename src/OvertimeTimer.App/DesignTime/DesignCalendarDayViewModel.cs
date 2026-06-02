namespace OvertimeTimer.App.DesignTime;

public sealed class DesignCalendarDayViewModel
{
    public DesignCalendarDayViewModel(int dayNumber, bool isInCurrentMonth, bool hasOvertime, bool hasDiary)
    {
        DayNumber = dayNumber;
        IsInCurrentMonth = isInCurrentMonth;
        HasOvertime = hasOvertime;
        HasDiary = hasDiary;
    }

    public int DayNumber { get; }

    public bool IsInCurrentMonth { get; }

    public bool HasOvertime { get; }

    public bool HasDiary { get; }

    public bool IsSelected { get; } = false;

    public bool IsRestDayOvertimeHighlighted { get; } = false;
}
