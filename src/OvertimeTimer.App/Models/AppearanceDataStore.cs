namespace OvertimeTimer.App.Models;

public sealed class AppearanceDataStore
{
    public AppearanceConfig AppearanceConfig { get; set; } = new();
    public string PreviewFontFamily { get; set; } = "Microsoft YaHei UI";
    public double PreviewFontSize { get; set; } = 14;
    public double PreviewLineHeight { get; set; } = 12;
    public string PreviewBackgroundColor { get; set; } = "#FFFFFBEB";
    public string PreviewTextColor { get; set; } = "#FF431407";
    public string PreviewLinkColor { get; set; } = "#FFEA580C";
    public string PreviewCodeBackgroundColor { get; set; } = "#FFFED7AA";
    public string PreviewCodeFontFamily { get; set; } = "Consolas";
}
