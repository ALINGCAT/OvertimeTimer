using System.Collections.ObjectModel;
using Prism.Commands;
using OvertimeTimer.App.Services;
using OvertimeTimer.Core.Models;

namespace OvertimeTimer.App.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    private readonly IStatusMessageService _statusMessageService;
    private WorkScheduleMode _selectedMode = WorkScheduleMode.Weekly;
    private DateOnly _anchorDate = DateOnly.FromDateTime(DateTime.Today);
    private int _weekCycleCount = 1;
    private int _currentCycleWeekIndex = 1;
    private int _workDays = 5;
    private int _restDays = 2;
    private int _anchorWorkDayIndex = 1;
    private string _statusText = "准备就绪";

    public SettingsViewModel(IStatusMessageService statusMessageService)
    {
        _statusMessageService = statusMessageService;
        WeeklyCycleItems = new ObservableCollection<WeeklyCycleItemViewModel>
        {
            new(1)
        };
        SaveCommand = new DelegateCommand(Save);
    }

    public string SelectedDateDisplay => _anchorDate.ToString("yyyy-MM-dd");

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
                RaisePropertyChanged(nameof(SelectedDateDisplay));
            }
        }
    }

    public int WeekCycleCount
    {
        get => _weekCycleCount;
        set => SetProperty(ref _weekCycleCount, value);
    }

    public int CurrentCycleWeekIndex
    {
        get => _currentCycleWeekIndex;
        set => SetProperty(ref _currentCycleWeekIndex, value);
    }

    public ObservableCollection<WeeklyCycleItemViewModel> WeeklyCycleItems { get; }

    public int WorkDays
    {
        get => _workDays;
        set => SetProperty(ref _workDays, value);
    }

    public int RestDays
    {
        get => _restDays;
        set => SetProperty(ref _restDays, value);
    }

    public int AnchorWorkDayIndex
    {
        get => _anchorWorkDayIndex;
        set => SetProperty(ref _anchorWorkDayIndex, value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    public DelegateCommand SaveCommand { get; }

    private void Save()
    {
        _statusMessageService.Show("设置已保存（待接入持久化）");
        StatusText = "设置已保存";
    }
}
