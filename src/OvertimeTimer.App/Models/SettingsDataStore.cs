namespace OvertimeTimer.App.Models;

public sealed class SettingsDataStore
{
    public WorkScheduleConfig WorkScheduleConfig { get; set; } = new();

    public DiaryStorageConfig DiaryStorageConfig { get; set; } = new();

    public AppearanceConfig AppearanceConfig { get; set; } = new();

    public string PreviewFontFamily { get; set; } = "Microsoft YaHei UI";

    public double PreviewFontSize { get; set; } = 8;

    public double PreviewLineHeight { get; set; } = 12;

    public DateOnly? LastSelectedDate { get; set; }
}
