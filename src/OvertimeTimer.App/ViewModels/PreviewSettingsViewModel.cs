using System.Collections.ObjectModel;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Services;

namespace OvertimeTimer.App.ViewModels;

public sealed class PreviewSettingsViewModel : SettingsSectionViewModelBase
{
    private readonly IAppearanceSettingsService _appearanceSettingsService;
    private readonly Func<Task> _saveAsync;
    private readonly ILocalizationService _loc;
    private bool _isLoading;

    public PreviewSettingsViewModel(
        IAppearanceSettingsService appearanceSettingsService,
        Func<Task> saveAsync,
        ILocalizationService localizationService)
    {
        _appearanceSettingsService = appearanceSettingsService;
        _saveAsync = saveAsync;
        _loc = localizationService;

        foreach (var family in System.Windows.Media.Fonts.SystemFontFamilies.OrderBy(f => f.Source))
            AvailableFontFamilies.Add(family.Source);
        foreach (var family in System.Windows.Media.Fonts.SystemFontFamilies.Where(IsMonospace).OrderBy(f => f.Source))
            AvailableCodeFontFamilies.Add(family.Source);

        PropertyChanged += (_, args) =>
        {
            if (_isLoading || args.PropertyName is not (nameof(PreviewFontFamily) or nameof(PreviewFontSize) or nameof(PreviewCodeFontFamily)))
            {
                return;
            }

            _appearanceSettingsService.ApplyPreviewTypography(
                PreviewFontFamily, PreviewFontSize, PreviewCodeFontFamily);
            ScheduleAutoSave(SaveCurrentSectionAsync);
        };
    }

    public ObservableCollection<string> AvailableFontFamilies { get; } = new();

    public ObservableCollection<string> AvailableCodeFontFamilies { get; } = new();

    private string _previewFontFamily = "Microsoft YaHei UI";
    public string PreviewFontFamily { get => _previewFontFamily; set => SetProperty(ref _previewFontFamily, value); }

    private double _previewFontSize = 14;
    public double PreviewFontSize { get => _previewFontSize; set => SetProperty(ref _previewFontSize, value); }

    private string _previewCodeFontFamily = "Consolas";
    public string PreviewCodeFontFamily { get => _previewCodeFontFamily; set => SetProperty(ref _previewCodeFontFamily, value); }

    public void LoadFrom(string fontFamily, double fontSize, string codeFontFamily)
    {
        _isLoading = true;
        PreviewFontFamily = fontFamily;
        PreviewFontSize = fontSize;
        PreviewCodeFontFamily = codeFontFamily;
        _isLoading = false;
    }

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

    private static bool IsMonospace(System.Windows.Media.FontFamily f)
    {
        var n = f.Source.ToLowerInvariant();
        return n.Contains("consol") || n.Contains("courier") || n.Contains("mono") || n.Contains("code")
               || n.Contains("hack") || n.Contains("cascadia") || n.Contains("fira") || n.Contains("jetbrains")
               || n.Contains("source");
    }
}
