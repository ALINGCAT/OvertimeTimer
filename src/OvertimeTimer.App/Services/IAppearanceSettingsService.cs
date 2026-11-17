using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public interface IAppearanceSettingsService
{
    bool IsDarkTheme { get; }

    void Apply(AppearanceConfig appearanceConfig);

    void ApplyPreviewColors(string backgroundColor, string textColor, string linkColor, string codeBackgroundColor);

    void ApplyPreviewTypography(string fontFamily, double fontSize, string codeFontFamily);

    event Action? PreviewSettingsChanged;

    event Action? ThemeChanged;
}
