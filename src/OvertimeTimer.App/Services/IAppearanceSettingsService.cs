using OvertimeTimer.Core.Models;

namespace OvertimeTimer.App.Services;

public interface IAppearanceSettingsService
{
    bool TryNormalizeColor(string colorText, out string normalizedColor);

    void Apply(AppearanceConfig appearanceConfig);
}
