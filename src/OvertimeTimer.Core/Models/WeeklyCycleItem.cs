namespace OvertimeTimer.Core.Models;

public sealed class WeeklyCycleItem
{
    public int WeekIndex { get; set; }

    public bool MondayWork { get; set; } = true;

    public bool TuesdayWork { get; set; } = true;

    public bool WednesdayWork { get; set; } = true;

    public bool ThursdayWork { get; set; } = true;

    public bool FridayWork { get; set; } = true;

    public bool SaturdayWork { get; set; }

    public bool SundayWork { get; set; }
}
