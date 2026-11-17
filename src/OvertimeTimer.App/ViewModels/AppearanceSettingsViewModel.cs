using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Models;
using OvertimeTimer.App.Services;

namespace OvertimeTimer.App.ViewModels;

public sealed class AppearanceSettingsViewModel : SettingsSectionViewModelBase
{
    private readonly IAppearanceSettingsService _appearanceSettingsService;
    private readonly Func<Task> _saveAsync;
    private readonly ILocalizationService _loc;

    public AppearanceSettingsViewModel(
        IAppearanceSettingsService appearanceSettingsService,
        Func<Task> saveAsync,
        ILocalizationService localizationService)
    {
        _appearanceSettingsService = appearanceSettingsService;
        _saveAsync = saveAsync;
        _loc = localizationService;
    }

    public IReadOnlyList<AppearancePresets.Preset> AvailablePresets => AppearancePresets.All;

    private AppearancePresets.Preset? _selectedPreset;

    public AppearancePresets.Preset? SelectedPreset
    {
        get => _selectedPreset;
        set
        {
            if (value is null)
            {
                RaisePropertyChanged(nameof(SelectedPreset));
                return;
            }

            if (ReferenceEquals(_selectedPreset, value))
            {
                return;
            }

            _selectedPreset = value;
            _currentConfig = value.Config;
            PreviewBackgroundColor = value.PreviewBackgroundColor;
            PreviewTextColor = value.PreviewTextColor;
            PreviewLinkColor = value.PreviewLinkColor;
            PreviewCodeBackgroundColor = value.PreviewCodeBackgroundColor;

            _appearanceSettingsService.Apply(value.Config);
            _appearanceSettingsService.ApplyPreviewColors(
                PreviewBackgroundColor, PreviewTextColor, PreviewLinkColor, PreviewCodeBackgroundColor);

            RaisePropertyChanged(nameof(SelectedPreset));
            RaisePropertyChanged(nameof(IsCustomTheme));

            _ = SaveCurrentSectionAsync();
        }
    }

    public bool IsCustomTheme => _selectedPreset is null;

    private AppearanceConfig _currentConfig = new();

    public string PreviewBackgroundColor { get; private set; } = "#FFFFFFFF";

    public string PreviewTextColor { get; private set; } = "#FF1F3A56";

    public string PreviewLinkColor { get; private set; } = "#FF2563EB";

    public string PreviewCodeBackgroundColor { get; private set; } = "#FFEFF6FC";

    public void LoadFrom(AppearanceDataStore data)
    {
        var upgradedConfig = AppearancePresets.UpgradeLegacy(data.AppearanceConfig);
        _currentConfig = upgradedConfig;
        _selectedPreset = AppearancePresets.FindByVisibleColors(upgradedConfig);

        PreviewBackgroundColor = data.PreviewBackgroundColor;
        PreviewTextColor = data.PreviewTextColor;
        PreviewLinkColor = data.PreviewLinkColor;
        PreviewCodeBackgroundColor = data.PreviewCodeBackgroundColor;

        RaisePropertyChanged(nameof(SelectedPreset));
        RaisePropertyChanged(nameof(IsCustomTheme));

        _appearanceSettingsService.Apply(upgradedConfig);
        _appearanceSettingsService.ApplyPreviewColors(
            PreviewBackgroundColor, PreviewTextColor, PreviewLinkColor, PreviewCodeBackgroundColor);
    }

    public AppearanceConfig ToModel() => _currentConfig;

    private async Task SaveCurrentSectionAsync()
    {
        try
        {
            await _saveAsync();
        }
        catch
        {
            await ShowSaveFeedbackAsync(_loc["Settings.SaveFailed"], true);
        }
    }
}
