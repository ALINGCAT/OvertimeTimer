namespace OvertimeTimer.Core.Models;

public sealed class WorkScheduleConfig
{
    public WorkScheduleMode Mode { get; set; } = WorkScheduleMode.Weekly;

    public DateOnly AnchorDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public int WeekCycleCount { get; set; } = 1;

    public int CurrentCycleWeekIndex { get; set; } = 1;

    public List<WeeklyCycleItem> WeeklyCycles { get; set; } = new();

    public int WorkDays { get; set; } = 5;

    public int RestDays { get; set; } = 2;

    public int AnchorWorkDayIndex { get; set; } = 1;
}
