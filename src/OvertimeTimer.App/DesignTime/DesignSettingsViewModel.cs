using System.Collections.ObjectModel;

namespace OvertimeTimer.App.DesignTime;

public sealed class DesignSettingsViewModel
{
    public string CurrentSectionTitle { get; } = "通用设置";

    public string CurrentSectionDescription { get; } = "配置日记文件存储、分组方式和界面语言。";

    public string DiaryRootPath { get; } = @"D:\QuickAccess\Documents\GitHub\OvertimeTimer\dailies";

    public bool IsFlatStorageMode { get; } = false;

    public bool IsByYearStorageMode { get; } = true;

    public bool IsByMonthStorageMode { get; } = false;

    public string DiaryStorageModeDescription { get; } = "按年份创建子目录，例如 2026\\2026-06-03.md。";

    public string DiaryStoragePathStatus { get; } = "当前目录：D:\\QuickAccess\\Documents\\GitHub\\OvertimeTimer\\dailies";

    public bool IsGeneralSectionSelected { get; } = true;

    public bool IsWorkScheduleSectionSelected { get; } = false;

    public bool IsAppearanceSectionSelected { get; } = false;

    public bool IsWeeklyMode { get; } = true;

    public bool IsDailyMode { get; } = false;

    public string SaveFeedbackMessage { get; } = "已应用并保存配置";

    public bool HasSaveFeedback { get; } = true;

    public bool IsSaveFeedbackError { get; } = false;

    public int WeekCycleCount { get; } = 1;

    public int CurrentCycleWeekIndex { get; } = 1;

    public int WorkDays { get; } = 5;

    public int RestDays { get; } = 2;

    public int AnchorWorkDayIndexMax { get; } = 7;

    public int AnchorWorkDayIndex { get; } = 1;

    public string TodayDescription { get; } = "今天是6月3日";

    public ObservableCollection<DesignWeeklyCycleItemViewModel> WeeklyCycleItems { get; } = new()
    {
        new DesignWeeklyCycleItemViewModel(1)
    };

    public object ShowGeneralSectionCommand { get; } = new object();

    public object ShowWorkScheduleSectionCommand { get; } = new object();

    public object ShowAppearanceSectionCommand { get; } = new object();

    public object ChooseDiaryRootPathCommand { get; } = new object();

    public object SaveCommand { get; } = new object();
}
