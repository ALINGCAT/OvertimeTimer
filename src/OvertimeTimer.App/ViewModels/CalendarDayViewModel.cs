namespace OvertimeTimer.App.ViewModels;

public sealed class CalendarDayViewModel : ViewModelBase
{
    private bool _isSelected;

    public CalendarDayViewModel(DateOnly date, bool isInCurrentMonth)
    {
        Date = date;
        IsInCurrentMonth = isInCurrentMonth;
        IsToday = date == DateOnly.FromDateTime(DateTime.Today);
    }

    public DateOnly Date { get; }

    public int DayNumber => Date.Day;

    public bool IsInCurrentMonth { get; }

    public bool IsRestDay => Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    public bool HasOvertime { get; set; }

    public bool HasDiary { get; set; }

    public bool IsToday { get; }

    public bool IsRestDayOvertimeHighlighted => IsRestDay && HasOvertime;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
