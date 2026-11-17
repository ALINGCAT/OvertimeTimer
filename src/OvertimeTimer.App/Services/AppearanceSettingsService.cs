using System.Windows;
using System.Windows.Media;
using HandyControl.Themes;
using OvertimeTimer.App.Models;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace OvertimeTimer.App.Services;

public sealed class AppearanceSettingsService : IAppearanceSettingsService
{
    public event Action? PreviewSettingsChanged;

    public event Action? ThemeChanged;

    public bool IsDarkTheme { get; private set; }

    public void Apply(AppearanceConfig appearanceConfig)
    {
        var resources = Application.Current?.Resources;
        var accentColor = ParseColor(appearanceConfig.AccentColor) ?? Colors.DodgerBlue;

        try
        {
            ThemeManager.Current.ApplicationTheme = appearanceConfig.IsDarkTheme
                ? ApplicationTheme.Dark
                : ApplicationTheme.Light;

            var accentBrush = new SolidColorBrush(accentColor);
            accentBrush.Freeze();
            ThemeManager.Current.AccentColor = accentBrush;
        }
        catch
        {
            // HandyControl theme switching is best-effort; custom brushes below still apply.
        }

        IsDarkTheme = appearanceConfig.IsDarkTheme;
        WindowThemeHelper.SetDark(appearanceConfig.IsDarkTheme);

        if (resources is not null)
        {
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
            ApplyBrush(resources, "CalendarMarkerDotBrush", appearanceConfig.CalendarMarkerDotColor);
            ApplyBrush(resources, "CalendarDayBorderBrush", appearanceConfig.CalendarDayBorderColor);
            ApplyBrush(resources, "AccentBrush", appearanceConfig.AccentColor);
            ApplyBrush(resources, "AccentSoftBrush", appearanceConfig.AccentSoftColor);
            ApplyBrush(resources, "TextPrimaryBrush", appearanceConfig.TextPrimaryColor);
            ApplyBrush(resources, "TextSecondaryBrush", appearanceConfig.TextSecondaryColor);
            ApplyBrush(resources, "ShellBorderBrush", appearanceConfig.ShellBorderColor);

            resources["TextPrimaryColor"] = appearanceConfig.TextPrimaryColor;
            resources["TextSecondaryColor"] = appearanceConfig.TextSecondaryColor;
            resources["CardBorderColor"] = appearanceConfig.CardBorderColor;
            resources["ThemeAccentColor"] = appearanceConfig.AccentColor;
            resources["IsDarkTheme"] = appearanceConfig.IsDarkTheme;
        }

        ThemeChanged?.Invoke();
    }

    public void ApplyPreviewColors(string backgroundColor, string textColor, string linkColor, string codeBackgroundColor)
    {
        var resources = Application.Current?.Resources;
        if (resources is null)
        {
            return;
        }

        resources["PreviewBackgroundColor"] = backgroundColor;
        ApplyBrush(resources, "PreviewBackgroundBrush", backgroundColor);
        resources["PreviewTextColor"] = textColor;
        resources["PreviewLinkColor"] = linkColor;
        resources["PreviewCodeBackgroundColor"] = codeBackgroundColor;

        PreviewSettingsChanged?.Invoke();
    }

    public void ApplyPreviewTypography(string fontFamily, double fontSize, string codeFontFamily)
    {
        var resources = Application.Current?.Resources;
        if (resources is null)
        {
            return;
        }

        resources["PreviewFontFamily"] = new System.Windows.Media.FontFamily(string.IsNullOrWhiteSpace(fontFamily) ? "Microsoft YaHei UI" : fontFamily);
        resources["PreviewFontSize"] = fontSize;
        resources["PreviewCodeFontFamily"] = string.IsNullOrWhiteSpace(codeFontFamily) ? "Consolas" : codeFontFamily;

        PreviewSettingsChanged?.Invoke();
    }

    private static void ApplyBrush(ResourceDictionary resources, string resourceKey, string colorText)
    {
        var color = ParseColor(colorText);
        if (color is null)
        {
            return;
        }

        var brush = new SolidColorBrush(color.Value);
        brush.Freeze();
        resources[resourceKey] = brush;
    }

    private static Color? ParseColor(string colorText)
    {
        if (string.IsNullOrWhiteSpace(colorText))
        {
            return null;
        }

        try
        {
            return (Color)ColorConverter.ConvertFromString(colorText.Trim());
        }
        catch
        {
            return null;
        }
    }
}
