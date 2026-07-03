using System.Windows;
using System.Windows.Media;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public sealed class AppearanceSettingsService : IAppearanceSettingsService
{
    public event Action? PreviewSettingsChanged;

    public bool TryNormalizeColor(string colorText, out string normalizedColor)
    {
        normalizedColor = string.Empty;
        if (string.IsNullOrWhiteSpace(colorText))
        {
            return false;
        }

        try
        {
            var color = (Color)ColorConverter.ConvertFromString(colorText.Trim());
            normalizedColor = $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Apply(AppearanceConfig appearanceConfig)
    {
        var resources = Application.Current?.Resources;
        if (resources is null)
        {
            return;
        }

        ApplyBrush(resources, "AppWindowBackgroundBrush", appearanceConfig.WindowBackgroundColor);
        ApplyBrush(resources, "CalendarWorkdayBrush", appearanceConfig.CalendarWorkdayColor);
        ApplyBrush(resources, "CalendarRestDayBrush", appearanceConfig.CalendarRestDayColor);
        ApplyBrush(resources, "CalendarTodayBrush", appearanceConfig.CalendarTodayColor);
        ApplyBrush(resources, "CalendarOutOfMonthBrush", appearanceConfig.CalendarOutOfMonthColor);
        ApplyBrush(resources, "CalendarOvertimeDotBrush", appearanceConfig.CalendarOvertimeDotColor);
        ApplyBrush(resources, "CalendarDiaryDotBrush", appearanceConfig.CalendarDiaryDotColor);
        ApplyBrush(resources, "CalendarHolidayBrush", appearanceConfig.CalendarHolidayColor);
        ApplyBrush(resources, "CalendarAdjustWorkdayBrush", appearanceConfig.CalendarAdjustWorkdayColor);
        ApplyBrush(resources, "CardBackgroundBrush", appearanceConfig.CardBackgroundColor);
        ApplyBrush(resources, "CardBorderBrush", appearanceConfig.CardBorderColor);
        ApplyBrush(resources, "CalendarLeaveBrush", appearanceConfig.CalendarLeaveColor);
        ApplyBrush(resources, "CalendarDayBorderBrush", appearanceConfig.CalendarDayBorderColor);
    }

    public void ApplyPreviewSettings(string fontFamily, double fontSize, double lineHeight,
        string backgroundColor, string textColor, string linkColor,
        string codeBackgroundColor, string codeFontFamily)
    {
        var resources = Application.Current?.Resources;
        if (resources is null)
        {
            return;
        }

        resources["PreviewFontFamily"] = new System.Windows.Media.FontFamily(fontFamily);
        resources["PreviewFontSize"] = fontSize;
        resources["PreviewLineHeight"] = lineHeight;
        resources["PreviewBackgroundColor"] = backgroundColor;
        ApplyBrush(resources, "PreviewBackgroundBrush", backgroundColor);
        resources["PreviewTextColor"] = textColor;
        resources["PreviewLinkColor"] = linkColor;
        resources["PreviewCodeBackgroundColor"] = codeBackgroundColor;
        resources["PreviewCodeFontFamily"] = codeFontFamily;

        PreviewSettingsChanged?.Invoke();
    }

    private static void ApplyBrush(ResourceDictionary resources, string resourceKey, string colorText)
    {
        var color = (Color)ColorConverter.ConvertFromString(colorText);
        resources[resourceKey] = new SolidColorBrush(color);
    }
}
