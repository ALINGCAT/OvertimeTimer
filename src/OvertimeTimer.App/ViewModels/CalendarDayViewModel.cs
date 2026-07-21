namespace OvertimeTimer.App.ViewModels;

public sealed class CalendarDayViewModel : ViewModelBase
{
    private bool _isSelected;
    private bool _isRestDay;
    private bool _hasOvertime;
    private bool _hasDiary;
    private bool _isHoliday;
    private bool _isAdjustWorkday;
    private bool _isLeave;
    private bool _hasUnsavedDiary;
    private bool _isMarked;

    public CalendarDayViewModel(DateOnly date, bool isInCurrentMonth)
    {
        Date = date;
        IsInCurrentMonth = isInCurrentMonth;
        IsToday = date == DateOnly.FromDateTime(DateTime.Today);
    }

    public DateOnly Date { get; }

    public int DayNumber => Date.Day;

    public bool IsInCurrentMonth { get; }

    public bool IsRestDay
    {
        get => _isRestDay;
        set => SetProperty(ref _isRestDay, value);
    }

    public bool HasOvertime
    {
        get => _hasOvertime;
        set => SetProperty(ref _hasOvertime, value);
    }

    public bool HasDiary
    {
        get => _hasDiary;
        set
        {
            if (SetProperty(ref _hasDiary, value))
                RaisePropertyChanged(nameof(ShowDiaryDot));
        }
    }

    public bool IsToday { get; }

    public bool IsHoliday
    {
        get => _isHoliday;
        set => SetProperty(ref _isHoliday, value);
    }

    public bool IsAdjustWorkday
    {
        get => _isAdjustWorkday;
        set => SetProperty(ref _isAdjustWorkday, value);
    }

    public bool IsLeave
    {
        get => _isLeave;
        set => SetProperty(ref _isLeave, value);
    }

    public bool HasUnsavedDiary
    {
        get => _hasUnsavedDiary;
        set
        {
            if (SetProperty(ref _hasUnsavedDiary, value))
                RaisePropertyChanged(nameof(ShowDiaryDot));
        }
    }

    public bool IsMarked
    {
        get => _isMarked;
        set => SetProperty(ref _isMarked, value);
    }

    public bool ShowDiaryDot => HasDiary || HasUnsavedDiary;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
