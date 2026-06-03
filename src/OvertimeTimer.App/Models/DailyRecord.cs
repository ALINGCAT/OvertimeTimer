namespace OvertimeTimer.App.Models;

public sealed class DailyRecord
{
    public DateOnly Date { get; set; }

    public int OvertimeHours { get; set; }

    public int OvertimeMinutes { get; set; }

    public string DiaryMarkdown { get; set; } = string.Empty;

    public DateTime LastModified { get; set; }
}
