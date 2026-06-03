namespace OvertimeTimer.Core.Models;

public sealed class AppearanceConfig
{
    public string WindowBackgroundColor { get; set; } = "#FFEAF3FF";

    public string CalendarWorkdayColor { get; set; } = "#FF0F172A";

    public string CalendarRestDayColor { get; set; } = "#FF94A3B8";

    public string CalendarTodayColor { get; set; } = "#FF22C55E";

    public string CalendarOutOfMonthColor { get; set; } = "#FFE2E8F0";

    public string CalendarOvertimeDotColor { get; set; } = "#FFDC2626";

    public string CalendarDiaryDotColor { get; set; } = "#FF16A34A";
}
