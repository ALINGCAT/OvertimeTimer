namespace OvertimeTimer.Core.Models;

public sealed class AppDataStore
{
    public WorkScheduleConfig WorkScheduleConfig { get; set; } = new();

    public DiaryStorageConfig DiaryStorageConfig { get; set; } = new();

    public List<DailyRecord> DailyRecords { get; set; } = new();

    public DateOnly? LastSelectedDate { get; set; }
}
