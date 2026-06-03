using System.Windows;
using System.Windows.Media;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using OvertimeTimer.Core.Models;

namespace OvertimeTimer.App.Services;

public sealed class AppearanceSettingsService : IAppearanceSettingsService
{
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
    }

    private static void ApplyBrush(ResourceDictionary resources, string resourceKey, string colorText)
    {
        var color = (Color)ColorConverter.ConvertFromString(colorText);
        if (resources[resourceKey] is SolidColorBrush brush)
        {
            brush.Color = color;
            return;
        }

        resources[resourceKey] = new SolidColorBrush(color);
    }
}
