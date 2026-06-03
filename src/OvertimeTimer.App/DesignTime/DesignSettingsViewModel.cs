using System.Collections.ObjectModel;

namespace OvertimeTimer.App.DesignTime;

public sealed class DesignSettingsViewModel
{
    public string CurrentSectionTitle { get; } = "工作日规则";

    public string CurrentSectionDescription { get; } = "配置按周或按天的工作日循环规则。";

    public string TodayDescription { get; } = "今天是6月3日";

    public string DiaryRootPath { get; } = @"D:\QuickAccess\Documents\GitHub\OvertimeTimer\dailies";

    public bool IsFlatStorageMode { get; } = false;

    public bool IsByYearStorageMode { get; } = true;

    public bool IsByMonthStorageMode { get; } = false;

    public string DiaryStorageModeDescription { get; } = "按年份创建子目录，例如 2026\\2026-06-03.md。";

    public string DiaryStoragePathStatus { get; } = "当前目录：D:\\QuickAccess\\Documents\\GitHub\\OvertimeTimer\\dailies";

    public bool IsWorkScheduleSectionSelected { get; } = true;

    public bool IsStorageSectionSelected { get; } = false;

    public bool IsLanguageSectionSelected { get; } = false;

    public bool IsWeeklyMode { get; } = true;

    public bool IsDailyMode { get; } = false;

    public string WorkScheduleSaveFeedbackMessage { get; } = "工作日规则设置已保存。";

    public bool HasWorkScheduleSaveFeedback { get; } = true;

    public bool IsWorkScheduleSaveFeedbackError { get; } = false;

    public string StorageSaveFeedbackMessage { get; } = "日记根目录不能为空。";

    public bool HasStorageSaveFeedback { get; } = true;

    public bool IsStorageSaveFeedbackError { get; } = true;

    public int WeekCycleCount { get; } = 1;

    public int CurrentCycleWeekIndex { get; } = 1;

    public int WorkDays { get; } = 5;

    public int RestDays { get; } = 2;

    public int AnchorWorkDayIndexMax { get; } = 7;

    public int AnchorWorkDayIndex { get; } = 1;

    public ObservableCollection<DesignWeeklyCycleItemViewModel> WeeklyCycleItems { get; } = new()
    {
        new DesignWeeklyCycleItemViewModel(1)
    };

    public object ShowWorkScheduleSectionCommand { get; } = new object();

    public object ShowStorageSectionCommand { get; } = new object();

    public object ShowLanguageSectionCommand { get; } = new object();

    public object ChooseDiaryRootPathCommand { get; } = new object();

    public object SaveCommand { get; } = new object();
}
