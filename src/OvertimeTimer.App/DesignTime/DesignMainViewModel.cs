using System.Collections.ObjectModel;
using Prism.Commands;

namespace OvertimeTimer.App.DesignTime;

public sealed class DesignMainViewModel
{
    public string YearMonthLabel { get; } = "2026 年 06 月";

    public string SelectedDateLabel { get; } = "当前选择的日期: 2026-06-02 周二";

    public string MonthlyOvertimeSummary { get; } = "12 小时 30 分钟";

    public ObservableCollection<DesignCalendarDayViewModel> CalendarDays { get; } = new()
    {
        new DesignCalendarDayViewModel(27, false, false, true),
        new DesignCalendarDayViewModel(28, false, false, false),
        new DesignCalendarDayViewModel(29, false, false, false),
        new DesignCalendarDayViewModel(30, false, false, false),
        new DesignCalendarDayViewModel(31, false, false, false),
        new DesignCalendarDayViewModel(1, true, true, false),
        new DesignCalendarDayViewModel(2, true, false, false)
    };

    public DesignDayRecordViewModel SelectedDayRecord { get; } = new();

    public DelegateCommand TodayCommand { get; } = new(() => { });

    public DelegateCommand<DesignCalendarDayViewModel> SelectDayCommand { get; } = new(_ => { });

    public DelegateCommand OpenMonthPickerCommand { get; } = new(() => { });
}
