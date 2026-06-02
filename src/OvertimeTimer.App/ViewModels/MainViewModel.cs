using System.Collections.ObjectModel;
using Prism.Commands;
using OvertimeTimer.App.Services;

namespace OvertimeTimer.App.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly DayRecordViewModel _dayRecordViewModel;
    private readonly IMonthSelectionDialogService _monthSelectionDialogService;
    private DateOnly _displayedMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);
    private CalendarDayViewModel? _selectedDay;
    private string _currentMonthLabel = string.Empty;
    private string _yearMonthLabel = string.Empty;
    private string _monthlyOvertimeSummary = "0 小时 0 分钟";
    private string _selectedDateLabel = string.Empty;

    public MainViewModel(IStatusMessageService statusMessageService, IMonthSelectionDialogService monthSelectionDialogService)
    {
        _dayRecordViewModel = new DayRecordViewModel(statusMessageService);
        _monthSelectionDialogService = monthSelectionDialogService;
        CalendarDays = new ObservableCollection<CalendarDayViewModel>();

        TodayCommand = new DelegateCommand(GoToToday);
        SelectDayCommand = new DelegateCommand<CalendarDayViewModel>(SelectDay);
        OpenMonthPickerCommand = new DelegateCommand(OpenMonthPicker);

        LoadMonth(_displayedMonth);
        SyncSelectedDay(_selectedDate);
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
                SelectedDayRecord.Date = value;
                SyncSelectedDay(value);
            }
        }
    }

    public CalendarDayViewModel? SelectedDay
    {
        get => _selectedDay;
        private set => SetProperty(ref _selectedDay, value);
    }

    public DelegateCommand TodayCommand { get; }

    public DelegateCommand<CalendarDayViewModel> SelectDayCommand { get; }

    public DelegateCommand OpenMonthPickerCommand { get; }

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
        _displayedMonth = new DateOnly(month.Year, month.Month, 1);
        LoadMonth(_displayedMonth);
    }

    private void LoadMonth(DateOnly month)
    {
        CalendarDays.Clear();
        YearMonthLabel = month.ToString("yyyy 年 MM 月");

        var firstDay = new DateOnly(month.Year, month.Month, 1);
        var offset = ((int)firstDay.DayOfWeek + 6) % 7;
        var start = firstDay.AddDays(-offset);

        for (var index = 0; index < 42; index++)
        {
            var date = start.AddDays(index);
            CalendarDays.Add(new CalendarDayViewModel(date, date.Month == month.Month));
        }

        MonthlyOvertimeSummary = "0 小时 0 分钟";
        UpdateSelectedDateLabel(SelectedDate);
        SyncSelectedDay(SelectedDate);
    }

    private void SyncSelectedDay(DateOnly date)
    {
        foreach (var day in CalendarDays)
        {
            day.IsSelected = day.Date == date;
        }

        SelectedDay = CalendarDays.FirstOrDefault(day => day.Date == date)
            ?? CalendarDays.FirstOrDefault(day => day.IsInCurrentMonth);
        UpdateSelectedDateLabel(date);
    }

    private void UpdateSelectedDateLabel(DateOnly date)
    {
        var day = CalendarDays.FirstOrDefault(item => item.Date == date);
        var dayOfWeek = day?.Date.DayOfWeek ?? date.DayOfWeek;
        SelectedDateLabel = $"当前选择的日期: {date:yyyy-MM-dd} {GetChineseDayOfWeek(dayOfWeek)}";
    }

    private static string GetChineseDayOfWeek(DayOfWeek dayOfWeek) => dayOfWeek switch
    {
        DayOfWeek.Monday => "周一",
        DayOfWeek.Tuesday => "周二",
        DayOfWeek.Wednesday => "周三",
        DayOfWeek.Thursday => "周四",
        DayOfWeek.Friday => "周五",
        DayOfWeek.Saturday => "周六",
        DayOfWeek.Sunday => "周日",
        _ => string.Empty
    };
}
