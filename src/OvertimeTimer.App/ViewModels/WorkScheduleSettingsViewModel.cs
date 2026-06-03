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
    private int _currentCycleWeekIndex = 1;
    private int _workDays = 5;
    private int _restDays = 2;
    private int _anchorWorkDayIndex = 1;

    public WorkScheduleSettingsViewModel(Func<Task> saveAsync, ILocalizationService localizationService)
    {
        _saveAsync = saveAsync;
        _loc = localizationService;
        WeeklyCycleItems = new ObservableCollection<WeeklyCycleItemViewModel>
        {
            new(1)
        };
        SaveCommand = new DelegateCommand(() => _ = SaveCurrentSectionAsync());

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
                ClampCurrentCycleWeekIndex();
                EnsureWeeklyCycleItems();
            }
        }
    }

    public int CurrentCycleWeekIndex
    {
        get => _currentCycleWeekIndex;
        set => SetProperty(ref _currentCycleWeekIndex, ClampCurrentCycleWeekIndex(value));
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

    public DelegateCommand SaveCommand { get; }

    public void LoadFrom(WorkScheduleConfig workScheduleConfig)
    {
        SelectedMode = workScheduleConfig.Mode;
        AnchorDate = workScheduleConfig.AnchorDate;
        WeekCycleCount = workScheduleConfig.WeekCycleCount;
        CurrentCycleWeekIndex = workScheduleConfig.CurrentCycleWeekIndex;
        WorkDays = workScheduleConfig.WorkDays;
        RestDays = workScheduleConfig.RestDays;
        AnchorWorkDayIndex = workScheduleConfig.AnchorWorkDayIndex;

        WeeklyCycleItems.Clear();
        foreach (var weeklyCycle in workScheduleConfig.WeeklyCycles.OrderBy(item => item.WeekIndex))
        {
            WeeklyCycleItems.Add(WeeklyCycleItemViewModel.FromModel(weeklyCycle));
        }

        EnsureWeeklyCycleItems();
        ClampCurrentCycleWeekIndex();
        ClampAnchorWorkDayIndex();
    }

    public WorkScheduleConfig ToModel()
    {
        return new WorkScheduleConfig
        {
            Mode = SelectedMode,
            AnchorDate = AnchorDate,
            WeekCycleCount = WeekCycleCount,
            CurrentCycleWeekIndex = CurrentCycleWeekIndex,
            WeeklyCycles = WeeklyCycleItems
                .OrderBy(item => item.WeekIndex)
                .Select(item => item.ToModel())
                .ToList(),
            WorkDays = WorkDays,
            RestDays = RestDays,
            AnchorWorkDayIndex = AnchorWorkDayIndex
        };
    }

    public Task ShowLoadFailedFeedbackAsync()
    {
        return ShowSaveFeedbackAsync(_loc["Settings.LoadFailed"], true);
    }

    private async Task SaveCurrentSectionAsync()
    {
        ClampCurrentCycleWeekIndex();
        EnsureWeeklyCycleItems();

        try
        {
            await _saveAsync();
        }
        catch (Exception)
        {
            await ShowSaveFeedbackAsync(_loc["Settings.SaveFailed"], true);
            return;
        }

        await ShowSaveFeedbackAsync(_loc["Settings.Saved"], false);
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

    private void ClampCurrentCycleWeekIndex()
    {
        var clampedValue = ClampCurrentCycleWeekIndex(_currentCycleWeekIndex);
        if (clampedValue != _currentCycleWeekIndex)
        {
            SetProperty(ref _currentCycleWeekIndex, clampedValue, nameof(CurrentCycleWeekIndex));
        }
    }

    private int ClampCurrentCycleWeekIndex(int value)
    {
        var maxWeekIndex = Math.Max(1, WeekCycleCount);
        return Math.Clamp(value, 1, maxWeekIndex);
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
