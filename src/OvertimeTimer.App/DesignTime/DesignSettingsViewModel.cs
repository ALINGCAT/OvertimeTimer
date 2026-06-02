using System.Collections.ObjectModel;

namespace OvertimeTimer.App.DesignTime;

public sealed class DesignSettingsViewModel
{
    public string SelectedDateDisplay { get; } = "2026-06-02";

    public bool IsWeeklyMode { get; } = true;

    public bool IsDailyMode { get; } = false;

    public int WeekCycleCount { get; } = 1;

    public int CurrentCycleWeekIndex { get; } = 1;

    public int WorkDays { get; } = 5;

    public int RestDays { get; } = 2;

    public int AnchorWorkDayIndex { get; } = 1;

    public ObservableCollection<DesignWeeklyCycleItemViewModel> WeeklyCycleItems { get; } = new()
    {
        new DesignWeeklyCycleItemViewModel(1)
    };

    public object SaveCommand { get; } = new object();
}
