using System.Collections.ObjectModel;
using System.Diagnostics;
using Prism.Commands;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Models;
using OvertimeTimer.App.Services;

namespace OvertimeTimer.App.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly DayRecordViewModel _dayRecordViewModel;
    private readonly IMonthSelectionDialogService _monthSelectionDialogService;
    private readonly IRecordStoreService _recordStoreService;
    private readonly IDiaryFileService _diaryFileService;
    private readonly IWorkScheduleProvider _workScheduleProvider;
    private readonly ILocalizationService _loc;
    private DateOnly _displayedMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);
    private CalendarDayViewModel? _selectedDay;
    private string _yearMonthLabel = string.Empty;
    private string _monthlyOvertimeSummary = string.Empty;
    private string _selectedDateLabel = string.Empty;
    private bool _refreshingStats;
    private readonly Dictionary<DateOnly, string> _unsavedDiaryCache = new();
    private readonly Dictionary<DateOnly, string> _diarySnapshot = new();

    public MainViewModel(
        IStatusMessageService statusMessageService,
        IMonthSelectionDialogService monthSelectionDialogService,
        IRecordStoreService recordStoreService,
        IDiaryFileService diaryFileService,
        IWorkScheduleProvider workScheduleProvider,
        ILocalizationService localizationService,
        IAppearanceSettingsService appearanceSettingsService)
    {
        _monthSelectionDialogService = monthSelectionDialogService;
        _recordStoreService = recordStoreService;
        _diaryFileService = diaryFileService;
        _workScheduleProvider = workScheduleProvider;
        _loc = localizationService;
        _dayRecordViewModel = new DayRecordViewModel(statusMessageService, recordStoreService, diaryFileService, localizationService, appearanceSettingsService);
        _dayRecordViewModel.Saved += () =>
        {
            var date = SelectedDayRecord.Date;
            _diarySnapshot[date] = SelectedDayRecord.DiaryMarkdown;
            var day = CalendarDays.FirstOrDefault(d => d.Date == date);
            if (day is not null) day.HasUnsavedDiary = false;
            _unsavedDiaryCache.Remove(date);
            _ = RefreshMonthStatsAsync();
        };

        _dayRecordViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(DayRecordViewModel.IsDirty) && SelectedDayRecord.IsDirty)
            {
                var day = CalendarDays.FirstOrDefault(d => d.Date == SelectedDayRecord.Date);
                if (day is not null) day.HasUnsavedDiary = true;
            }
        };

        _monthlyOvertimeSummary = _loc["Calendar.MonthlyPrefix"] + _loc["Calendar.DefaultOvertimeSummary"];
        CalendarDays = new ObservableCollection<CalendarDayViewModel>();

        TodayCommand = new DelegateCommand(GoToToday);
        SelectDayCommand = new DelegateCommand<CalendarDayViewModel>(SelectDay);
        OpenMonthPickerCommand = new DelegateCommand(OpenMonthPicker);

        HolidayCommand = new DelegateCommand(ToggleHoliday, CanToggleHoliday);
        AdjustWorkdayCommand = new DelegateCommand(ToggleAdjustWorkday, CanToggleAdjustWorkday);
        LeaveCommand = new DelegateCommand(ToggleLeave, CanToggleLeave);

        _workScheduleProvider.Load();
        _workScheduleProvider.ConfigChanged += () => _ = LoadMonthAsync(_displayedMonth);

        _loc.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]")
            {
                _ = LoadMonthAsync(_displayedMonth);
            }
        };

        _ = LoadMonthAsync(_displayedMonth);
    }

    public ObservableCollection<CalendarDayViewModel> CalendarDays { get; }

    public DayRecordViewModel SelectedDayRecord => _dayRecordViewModel;

    public string YearMonthLabel
    {
        get => _yearMonthLabel;
        private set => SetProperty(ref _yearMonthLabel, value);
    }

    public string MonthlyOvertimeSummary
    {
        get => _monthlyOvertimeSummary;
        private set => SetProperty(ref _monthlyOvertimeSummary, value);
    }

    public string SelectedDateLabel
    {
        get => _selectedDateLabel;
        private set => SetProperty(ref _selectedDateLabel, value);
    }

    public DateOnly SelectedDate
    {
        get => _selectedDate;
        private set
        {
            if (SetProperty(ref _selectedDate, value))
            {
                _ = SelectDayAsync(value);
            }
        }
    }

    public CalendarDayViewModel? SelectedDay
    {
        get => _selectedDay;
        private set
        {
            if (SetProperty(ref _selectedDay, value))
            {
                HolidayCommand.RaiseCanExecuteChanged();
                AdjustWorkdayCommand.RaiseCanExecuteChanged();
                LeaveCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public DelegateCommand TodayCommand { get; }

    public DelegateCommand<CalendarDayViewModel> SelectDayCommand { get; }

    public DelegateCommand OpenMonthPickerCommand { get; }

    public DelegateCommand HolidayCommand { get; }

    public DelegateCommand AdjustWorkdayCommand { get; }

    public DelegateCommand LeaveCommand { get; }

    public void GoToToday()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        SetDisplayedMonth(new DateOnly(today.Year, today.Month, 1));
        SelectedDate = today;
    }

    private void OpenMonthPicker()
    {
        var result = _monthSelectionDialogService.Show(_displayedMonth);
        if (result is not null)
        {
            SetDisplayedMonth(result.SelectedMonth);
            if (result.UseTodayAsSelectedDate)
            {
                SelectedDate = DateOnly.FromDateTime(DateTime.Today);
            }
        }
    }

    private void SelectDay(CalendarDayViewModel? day)
    {
        if (day is null)
        {
            return;
        }

        SetDisplayedMonth(new DateOnly(day.Date.Year, day.Date.Month, 1));
        SelectedDate = day.Date;
    }

    private void SetDisplayedMonth(DateOnly month)
    {
        var normalized = new DateOnly(month.Year, month.Month, 1);
        if (normalized == _displayedMonth) return;
        _displayedMonth = normalized;
        _ = LoadMonthAsync(_displayedMonth);
    }

    private bool CanToggleHoliday()
    {
        var date = SelectedDate;
        var existing = _workScheduleProvider.GetOverride(date);
        if (existing is not null)
            return existing.Type == OverrideType.Holiday;

        return true;
    }

    private void ToggleHoliday()
    {
        var date = SelectedDate;
        var existing = _workScheduleProvider.GetOverride(date);

        if (existing is not null && existing.Type == OverrideType.Holiday)
        {
            _workScheduleProvider.RemoveOverride(date);
        }
        else
        {
            _workScheduleProvider.AddOverride(date, OverrideType.Holiday);
        }

        _ = LoadMonthAsync(_displayedMonth);
    }

    private bool CanToggleAdjustWorkday()
    {
        var date = SelectedDate;
        var existing = _workScheduleProvider.GetOverride(date);
        if (existing is not null)
            return existing.Type == OverrideType.AdjustWorkday;

        return _workScheduleProvider.IsRestDay(date);
    }

    private void ToggleAdjustWorkday()
    {
        var date = SelectedDate;
        var existing = _workScheduleProvider.GetOverride(date);

        if (existing is not null && existing.Type == OverrideType.AdjustWorkday)
        {
            _workScheduleProvider.RemoveOverride(date);
        }
        else
        {
            _workScheduleProvider.AddOverride(date, OverrideType.AdjustWorkday);
        }

        _ = LoadMonthAsync(_displayedMonth);
    }

    private bool CanToggleLeave()
    {
        var date = SelectedDate;
        var existing = _workScheduleProvider.GetOverride(date);
        if (existing is not null)
            return existing.Type == OverrideType.Leave;

        return true;
    }

    private void ToggleLeave()
    {
        var date = SelectedDate;
        var existing = _workScheduleProvider.GetOverride(date);

        if (existing is not null && existing.Type == OverrideType.Leave)
        {
            _workScheduleProvider.RemoveOverride(date);
        }
        else
        {
            _workScheduleProvider.AddOverride(date, OverrideType.Leave);
        }

        _ = LoadMonthAsync(_displayedMonth);
    }

    private async Task LoadMonthAsync(DateOnly month)
    {
        CalendarDays.Clear();
        YearMonthLabel = month.ToString(_loc["Calendar.YearMonthFormat"]);

        var firstDay = new DateOnly(month.Year, month.Month, 1);
        var offset = ((int)firstDay.DayOfWeek + 6) % 7;
        var start = firstDay.AddDays(-offset);

        var records = await _recordStoreService.LoadAllAsync();
        var monthStart = firstDay;
        var monthEnd = firstDay.AddMonths(1).AddDays(-1);

        int totalMinutes = 0;

        for (var index = 0; index < 42; index++)
        {
            var date = start.AddDays(index);
            var day = new CalendarDayViewModel(date, date.Month == month.Month)
            {
                IsRestDay = _workScheduleProvider.IsRestDay(date)
            };

            var o = _workScheduleProvider.GetOverride(date);
            if (o is not null)
            {
                day.IsHoliday = o.Type == OverrideType.Holiday;
                day.IsAdjustWorkday = o.Type == OverrideType.AdjustWorkday;
                day.IsLeave = o.Type == OverrideType.Leave;
            }

            var record = records.Find(r => r.Date == date);
            if (record is not null)
            {
                day.HasOvertime = record.OvertimeHours > 0 || record.OvertimeMinutes > 0;
                if (date >= monthStart && date <= monthEnd)
                {
                    totalMinutes += record.OvertimeHours * 60 + record.OvertimeMinutes;
                }
            }

            if (await _diaryFileService.ExistsAsync(date))
            {
                day.HasDiary = true;
            }

            day.HasUnsavedDiary = _unsavedDiaryCache.ContainsKey(date);

            CalendarDays.Add(day);
        }

        var totalHours = totalMinutes / 60;
        var remainingMinutes = totalMinutes % 60;
        MonthlyOvertimeSummary = _loc["Calendar.MonthlyPrefix"] + string.Format(_loc["Calendar.OvertimeFormat"], totalHours, remainingMinutes);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var monthEndCalc = firstDay.AddMonths(1).AddDays(-1);
        var daysRemaining = 0;
        for (var d = today.AddDays(1); d <= monthEndCalc; d = d.AddDays(1))
        {
            if (_workScheduleProvider.IsWorkDay(d))
                daysRemaining++;
        }

        if (daysRemaining > 0)
        {
            MonthlyOvertimeSummary += string.Format(_loc["Calendar.WorkDaysRemaining"], daysRemaining);
        }

        UpdateSelectedDateLabel(SelectedDate);
        await SelectDayAsync(SelectedDate);
    }

    private async Task RefreshMonthStatsAsync()
    {
        if (_refreshingStats) return;
        _refreshingStats = true;
        try
        {
            var records = await _recordStoreService.LoadAllAsync();
            var firstDay = _displayedMonth;
            var monthEnd = firstDay.AddMonths(1).AddDays(-1);
            int totalMinutes = 0;

            foreach (var day in CalendarDays)
            {
                if (day.Date < firstDay || day.Date > monthEnd)
                {
                    day.HasOvertime = false;
                    day.HasDiary = false;
                    continue;
                }

                var record = records.Find(r => r.Date == day.Date);
                day.HasOvertime = record is not null && (record.OvertimeHours > 0 || record.OvertimeMinutes > 0);
                if (record is not null)
                    totalMinutes += record.OvertimeHours * 60 + record.OvertimeMinutes;

                day.HasDiary = await _diaryFileService.ExistsAsync(day.Date);
                day.HasUnsavedDiary = _unsavedDiaryCache.ContainsKey(day.Date);
            }

            var totalHours = totalMinutes / 60;
            var remainingMinutes = totalMinutes % 60;
            MonthlyOvertimeSummary = _loc["Calendar.MonthlyPrefix"] + string.Format(_loc["Calendar.OvertimeFormat"], totalHours, remainingMinutes);

            var today = DateOnly.FromDateTime(DateTime.Today);
            var daysRemaining = 0;
            for (var d = today.AddDays(1); d <= monthEnd; d = d.AddDays(1))
            {
                if (_workScheduleProvider.IsWorkDay(d))
                    daysRemaining++;
            }

            if (daysRemaining > 0)
                MonthlyOvertimeSummary += string.Format(_loc["Calendar.WorkDaysRemaining"], daysRemaining);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[RefreshMonthStats] {ex.Message}");
        }
        finally
        {
            _refreshingStats = false;
        }
    }

    private async Task SelectDayAsync(DateOnly date)
    {
        try
        {
            var prevDate = SelectedDayRecord.Date;
            var prevContent = SelectedDayRecord.DiaryMarkdown;
            if (!string.IsNullOrWhiteSpace(prevContent) && prevContent != _diarySnapshot.GetValueOrDefault(prevDate, ""))
            {
                _unsavedDiaryCache[prevDate] = prevContent;
                var cachedDay = CalendarDays.FirstOrDefault(d => d.Date == prevDate);
                if (cachedDay is not null) cachedDay.HasUnsavedDiary = true;
            }

            SelectedDayRecord.Date = date;

            foreach (var day in CalendarDays)
            {
                day.IsSelected = day.Date == date;
            }

            SelectedDay = CalendarDays.FirstOrDefault(day => day.Date == date)
                ?? CalendarDays.FirstOrDefault(day => day.IsInCurrentMonth);
            UpdateSelectedDateLabel(date);

            await SelectedDayRecord.LoadAsync(date);
            _diarySnapshot[date] = SelectedDayRecord.DiaryMarkdown;

            if (_unsavedDiaryCache.TryGetValue(date, out var cached))
            {
                if (cached != SelectedDayRecord.DiaryMarkdown)
                    SelectedDayRecord.DiaryMarkdown = cached;
                else
                    _unsavedDiaryCache.Remove(date);
            }
            else
            {
                var day = CalendarDays.FirstOrDefault(d => d.Date == date);
                if (day is not null) day.HasUnsavedDiary = false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[SelectDayAsync] {ex.Message}");
        }
    }

    private void UpdateSelectedDateLabel(DateOnly date)
    {
        var dayOfWeekName = GetLocalizedDayOfWeek(date.DayOfWeek);
        var label = string.Format(_loc["Calendar.SelectedDateFormat"], $"{date:yyyy-MM-dd} {dayOfWeekName}");

        var o = _workScheduleProvider.GetOverride(date);
        if (o is not null)
        {
            label += " " + o.Type switch
            {
                OverrideType.Holiday => _loc["Calendar.Holiday"],
                OverrideType.AdjustWorkday => _loc["Calendar.AdjustWorkday"],
                OverrideType.Leave => _loc["Calendar.Leave"],
                _ => ""
            };
        }

        SelectedDateLabel = label;
    }

    private string GetLocalizedDayOfWeek(DayOfWeek dayOfWeek) => dayOfWeek switch
    {
        DayOfWeek.Monday => _loc["Calendar.Monday"],
        DayOfWeek.Tuesday => _loc["Calendar.Tuesday"],
        DayOfWeek.Wednesday => _loc["Calendar.Wednesday"],
        DayOfWeek.Thursday => _loc["Calendar.Thursday"],
        DayOfWeek.Friday => _loc["Calendar.Friday"],
        DayOfWeek.Saturday => _loc["Calendar.Saturday"],
        DayOfWeek.Sunday => _loc["Calendar.Sunday"],
        _ => string.Empty
    };
}
