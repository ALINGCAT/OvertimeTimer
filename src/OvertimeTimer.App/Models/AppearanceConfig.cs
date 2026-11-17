namespace OvertimeTimer.App.Models;

public sealed class AppearanceConfig
{
    public string WindowBackgroundColor { get; set; } = "#FFF3F8FC";

    public string CalendarWorkdayColor { get; set; } = "#FF1F3A56";

    public string CalendarRestDayColor { get; set; } = "#FF7D9AB3";

    public string CalendarTodayColor { get; set; } = "#FF22C55E";

    public string CalendarOutOfMonthColor { get; set; } = "#FFC9D8E4";

    public string CalendarOvertimeDotColor { get; set; } = "#FFDC2626";

    public string CalendarDiaryDotColor { get; set; } = "#FF16A34A";

    public string CalendarHolidayColor { get; set; } = "#FF8B5CF6";

    public string CalendarAdjustWorkdayColor { get; set; } = "#FFB91C1C";

    public string CardBackgroundColor { get; set; } = "#FFFFFFFF";

    public string CardBorderColor { get; set; } = "#FFDCE8F3";

    public string CalendarLeaveColor { get; set; } = "#FF0891B2";

    public string CalendarMarkerDotColor { get; set; } = "#FFE0A526";

    public string CalendarDayBorderColor { get; set; } = "#FFDCE8F3";

    public string AccentColor { get; set; } = "#FF3B82F6";

    public string AccentSoftColor { get; set; } = "#FFEAF3FB";

    public string TextPrimaryColor { get; set; } = "#FF16283A";

    public string TextSecondaryColor { get; set; } = "#FF5F7488";

    public string ShellBorderColor { get; set; } = "#FFDCE8F3";

    public bool IsDarkTheme { get; set; }
}
