namespace OvertimeTimer.App.Models;

public sealed class SettingsDataStore
{
    public WorkScheduleConfig WorkScheduleConfig { get; set; } = new();

    public DiaryStorageConfig DiaryStorageConfig { get; set; } = new();

    public AppearanceConfig AppearanceConfig { get; set; } = new();

    public string PreviewFontFamily { get; set; } = "Microsoft YaHei UI";

    public double PreviewFontSize { get; set; } = 14;

    public double PreviewLineHeight { get; set; } = 12;

    public string PreviewBackgroundColor { get; set; } = "#FFF5F0E1";

    public string PreviewTextColor { get; set; } = "#FF0F172A";

    public string PreviewLinkColor { get; set; } = "#FF3B82F6";

    public string PreviewCodeBackgroundColor { get; set; } = "#FFF3F4F6";

    public string PreviewCodeFontFamily { get; set; } = "Consolas";

    public DateOnly? LastSelectedDate { get; set; }
}
