using System.Collections.ObjectModel;
using Prism.Commands;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.ViewModels;

public sealed class WorkScheduleSettingsViewModel : SettingsSectionViewModelBase
{
    private readonly Func<Task> _saveAsync;
    private readonly ILocalizationService _loc;
    private WorkScheduleMode _selectedMode = WorkScheduleMode.Weekly;
    private DateOnly _anchorDate = DateOnly.FromDateTime(DateTime.Today);
    private int _weekCycleCount = 1;
    private int _workDays = 5;
    private int _restDays = 2;
    private int _anchorWorkDayIndex = 1;

    public WorkScheduleSettingsViewModel(Func<Task> saveAsync, ILocalizationService localizationService)
    {
        _saveAsync = saveAsync;
        _loc = localizationService;
        SaveCommand = new DelegateCommand(() => _ = SaveCurrentSectionAsync());
        WeeklyCycleItems = new ObservableCollection<WeeklyCycleItemViewModel>
        {
            new(1)
        };

        _loc.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]")
            {
                RaisePropertyChanged(nameof(TodayDescription));
            }
        };
    }

    public string TodayDescription => string.Format(_loc["WorkSchedule.TodayDescription"], _anchorDate.Month, _anchorDate.Day);

    public WorkScheduleMode SelectedMode
    {
        get => _selectedMode;
        set
        {
            if (SetProperty(ref _selectedMode, value))
            {
                RaisePropertyChanged(nameof(IsWeeklyMode));
                RaisePropertyChanged(nameof(IsDailyMode));
            }
        }
    }

    public bool IsWeeklyMode
    {
        get => SelectedMode == WorkScheduleMode.Weekly;
        set
        {
            if (value)
            {
                SelectedMode = WorkScheduleMode.Weekly;
            }
        }
    }

    public bool IsDailyMode
    {
        get => SelectedMode == WorkScheduleMode.Daily;
        set
        {
            if (value)
            {
                SelectedMode = WorkScheduleMode.Daily;
            }
        }
    }

    public DateOnly AnchorDate
    {
        get => _anchorDate;
        set
        {
            if (SetProperty(ref _anchorDate, value))
            {
                RaisePropertyChanged(nameof(TodayDescription));
            }
        }
    }

    public int AnchorWorkDayIndexMax => Math.Max(1, WorkDays + RestDays);

    public int WeekCycleCount
    {
        get => _weekCycleCount;
        set
        {
            var normalizedValue = Math.Max(1, value);
            if (SetProperty(ref _weekCycleCount, normalizedValue))
            {
                EnsureWeeklyCycleItems();
            }
        }
    }

    public ObservableCollection<WeeklyCycleItemViewModel> WeeklyCycleItems { get; }

    public int WorkDays
    {
        get => _workDays;
        set
        {
            if (SetProperty(ref _workDays, value))
            {
                RaisePropertyChanged(nameof(AnchorWorkDayIndexMax));
                ClampAnchorWorkDayIndex();
            }
        }
    }

    public int RestDays
    {
        get => _restDays;
        set
        {
            if (SetProperty(ref _restDays, value))
            {
                RaisePropertyChanged(nameof(AnchorWorkDayIndexMax));
                ClampAnchorWorkDayIndex();
            }
        }
    }

    public int AnchorWorkDayIndex
    {
        get => _anchorWorkDayIndex;
        set => SetProperty(ref _anchorWorkDayIndex, ClampAnchorWorkDayIndex(value));
    }

    public void LoadFrom(WorkScheduleConfig workScheduleConfig)
    {
        SelectedMode = workScheduleConfig.Mode;
        WeekCycleCount = workScheduleConfig.WeekCycleCount;
        WorkDays = workScheduleConfig.WorkDays;
        RestDays = workScheduleConfig.RestDays;
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (SelectedMode == WorkScheduleMode.Weekly)
        {
            var ordered = workScheduleConfig.WeeklyCycles.OrderBy(i => i.WeekIndex).ToList();
            if (ordered.Count == WeekCycleCount && WeekCycleCount > 0)
            {
                var anchorMon = workScheduleConfig.AnchorDate.AddDays(-(((int)workScheduleConfig.AnchorDate.DayOfWeek + 6) % 7));
                var todayMon = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
                var weekDiff = (todayMon.DayNumber - anchorMon.DayNumber) / 7;
                var shift = ((weekDiff % WeekCycleCount) + WeekCycleCount) % WeekCycleCount;
                if (shift > 0)
                    ordered = ordered.Skip(shift).Concat(ordered.Take(shift)).ToList();
            }

            WeeklyCycleItems.Clear();
            for (int i = 0; i < ordered.Count; i++)
            {
                var item = WeeklyCycleItemViewModel.FromModel(ordered[i]);
                item.WeekIndex = i + 1;
                WeeklyCycleItems.Add(item);
            }
            AnchorWorkDayIndex = 1;
        }
        else
        {
            var cycleLen = WorkDays + RestDays;
            if (cycleLen > 0)
            {
                var dayDiff = today.DayNumber - workScheduleConfig.AnchorDate.DayNumber;
                var anchorPos = workScheduleConfig.AnchorWorkDayIndex - 1;
                var todayPos = ((anchorPos + dayDiff) % cycleLen + cycleLen) % cycleLen + 1;
                AnchorWorkDayIndex = todayPos;
            }

            WeeklyCycleItems.Clear();
            foreach (var item in workScheduleConfig.WeeklyCycles.OrderBy(i => i.WeekIndex))
                WeeklyCycleItems.Add(WeeklyCycleItemViewModel.FromModel(item));
        }

        AnchorDate = today;
        EnsureWeeklyCycleItems();
        ClampAnchorWorkDayIndex();
    }

    public WorkScheduleConfig ToModel()
    {
        return new WorkScheduleConfig
        {
            Mode = SelectedMode,
            AnchorDate = DateOnly.FromDateTime(DateTime.Today),
            WeekCycleCount = WeekCycleCount,
            WeeklyCycles = WeeklyCycleItems.OrderBy(i => i.WeekIndex).Select(i => i.ToModel()).ToList(),
            WorkDays = WorkDays,
            RestDays = RestDays,
            AnchorWorkDayIndex = AnchorWorkDayIndex
        };
    }

    public DelegateCommand SaveCommand { get; }

    public Task ShowLoadFailedFeedbackAsync()
    {
        return ShowSaveFeedbackAsync(_loc["Settings.LoadFailed"], true);
    }

    private async Task SaveCurrentSectionAsync()
    {
        EnsureWeeklyCycleItems();
        try { await _saveAsync(); }
        catch { await ShowSaveFeedbackAsync("保存失败", true); return; }
        await ShowSaveFeedbackAsync("保存成功", false);
    }

    private void EnsureWeeklyCycleItems()
    {
        while (WeeklyCycleItems.Count < WeekCycleCount)
        {
            WeeklyCycleItems.Add(new WeeklyCycleItemViewModel(WeeklyCycleItems.Count + 1));
        }

        while (WeeklyCycleItems.Count > WeekCycleCount)
        {
            WeeklyCycleItems.RemoveAt(WeeklyCycleItems.Count - 1);
        }
    }

    private void ClampAnchorWorkDayIndex()
    {
        var clampedValue = ClampAnchorWorkDayIndex(_anchorWorkDayIndex);
        if (clampedValue != _anchorWorkDayIndex)
        {
            SetProperty(ref _anchorWorkDayIndex, clampedValue, nameof(AnchorWorkDayIndex));
        }
    }

    private int ClampAnchorWorkDayIndex(int value)
    {
        var maxDayIndex = Math.Max(1, WorkDays + RestDays);
        return Math.Clamp(value, 1, maxDayIndex);
    }
}
