namespace OvertimeTimer.Core.Models;

public sealed class SettingsDataStore
{
    public WorkScheduleConfig WorkScheduleConfig { get; set; } = new();

    public DiaryStorageConfig DiaryStorageConfig { get; set; } = new();

    public AppearanceConfig AppearanceConfig { get; set; } = new();

    public DateOnly? LastSelectedDate { get; set; }
}
