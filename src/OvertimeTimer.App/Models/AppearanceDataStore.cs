namespace OvertimeTimer.App.Models;

public sealed class AppearanceDataStore
{
    public AppearanceConfig AppearanceConfig { get; set; } = new();
    public string PreviewFontFamily { get; set; } = "Microsoft YaHei UI";
    public double PreviewFontSize { get; set; } = 14;
    public string PreviewBackgroundColor { get; set; } = "#FFFFFFFF";
    public string PreviewTextColor { get; set; } = "#FF1F3A56";
    public string PreviewLinkColor { get; set; } = "#FF2563EB";
    public string PreviewCodeBackgroundColor { get; set; } = "#FFEFF6FC";
    public string PreviewCodeFontFamily { get; set; } = "Consolas";
}
