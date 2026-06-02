namespace OvertimeTimer.App.DesignTime;

public sealed class DesignWeeklyCycleItemViewModel
{
    public DesignWeeklyCycleItemViewModel(int weekIndex)
    {
        WeekIndex = weekIndex;
    }

    public int WeekIndex { get; }

    public bool MondayWork { get; } = true;

    public bool TuesdayWork { get; } = true;

    public bool WednesdayWork { get; } = true;

    public bool ThursdayWork { get; } = true;

    public bool FridayWork { get; } = true;

    public bool SaturdayWork { get; } = false;

    public bool SundayWork { get; } = false;
}
