namespace OvertimeTimer.Core.Models;

public sealed class MonthlySummary
{
    public DateOnly Month { get; set; }

    public int TotalOvertimeMinutes { get; set; }
}
