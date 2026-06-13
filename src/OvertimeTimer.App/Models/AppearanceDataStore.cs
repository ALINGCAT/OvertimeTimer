namespace OvertimeTimer.App.Models;

public sealed class AppearanceDataStore
{
    public AppearanceConfig AppearanceConfig { get; set; } = new();
    public string PreviewFontFamily { get; set; } = "Microsoft YaHei UI";
    public double PreviewFontSize { get; set; } = 14;
    public double PreviewLineHeight { get; set; } = 12;
    public string PreviewBackgroundColor { get; set; } = "#FFFFFFFF";
    public string PreviewTextColor { get; set; } = "#FF0C4A6E";
    public string PreviewLinkColor { get; set; } = "#FF0284C7";
    public string PreviewCodeBackgroundColor { get; set; } = "#FFE0F2FE";
    public string PreviewCodeFontFamily { get; set; } = "Consolas";
}
