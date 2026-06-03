using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public interface IAppearanceSettingsService
{
    bool TryNormalizeColor(string colorText, out string normalizedColor);

    void Apply(AppearanceConfig appearanceConfig);

    void ApplyPreviewSettings(string fontFamily, double fontSize, double lineHeight);
}
