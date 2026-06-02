using System.Collections.ObjectModel;

namespace OvertimeTimer.App.DesignTime;

public sealed class DesignMainViewModel
{
    public string YearMonthLabel { get; } = "2026 年 06 月";

    public string StatusText { get; } = "已加载 2026-06";

    public string MonthlyOvertimeSummary { get; } = "12 小时 30 分钟";

    public ObservableCollection<DesignCalendarDayViewModel> CalendarDays { get; } = new()
    {
        new(27, false, false, true),
        new(28, false, false, false),
        new(29, false, false, false),
        new(30, false, false, false),
        new(31, false, false, false),
        new(1, true, true, false),
        new(2, true, false, false)
    };

    public DesignDayRecordViewModel SelectedDayRecord { get; } = new();

    public object PreviousMonthCommand { get; } = new object();

    public object NextMonthCommand { get; } = new object();

    public object TodayCommand { get; } = new object();

    public object SelectDayCommand { get; } = new object();
}
