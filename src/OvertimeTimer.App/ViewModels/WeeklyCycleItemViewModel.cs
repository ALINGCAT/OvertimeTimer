namespace OvertimeTimer.App.ViewModels;

public sealed class WeeklyCycleItemViewModel : ViewModelBase
{
    public WeeklyCycleItemViewModel(int weekIndex)
    {
        WeekIndex = weekIndex;
    }

    public int WeekIndex { get; }

    public bool MondayWork { get; set; } = true;

    public bool TuesdayWork { get; set; } = true;

    public bool WednesdayWork { get; set; } = true;

    public bool ThursdayWork { get; set; } = true;

    public bool FridayWork { get; set; } = true;

    public bool SaturdayWork { get; set; }

    public bool SundayWork { get; set; }
}
