using System.Collections.ObjectModel;
using System.Windows.Media;
using Prism.Commands;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Services;

namespace OvertimeTimer.App.ViewModels;

public sealed class PreviewSettingsViewModel : SettingsSectionViewModelBase
{
    private readonly IAppearanceSettingsService _appearanceSettingsService;
    private readonly IColorSelectionService _colorSelectionService;
    private readonly Func<Task> _saveAsync;
    private readonly ILocalizationService _loc;

    public PreviewSettingsViewModel(
        IAppearanceSettingsService appearanceSettingsService,
        IColorSelectionService colorSelectionService,
        Func<Task> saveAsync,
        ILocalizationService localizationService)
    {
        _appearanceSettingsService = appearanceSettingsService;
        _colorSelectionService = colorSelectionService;
        _saveAsync = saveAsync;
        _loc = localizationService;
        SaveCommand = new DelegateCommand(() => _ = SaveCurrentSectionAsync());
        ChooseColorCommand = new DelegateCommand<string>(ChooseColor);

        foreach (var family in Fonts.SystemFontFamilies.OrderBy(f => f.Source))
        {
            AvailableFontFamilies.Add(family.Source);
        }

        foreach (var family in Fonts.SystemFontFamilies
                     .Where(f => IsMonospace(f))
                     .OrderBy(f => f.Source))
        {
            AvailableCodeFontFamilies.Add(family.Source);
        }
    }

    private string _previewBackgroundColor = "#FFF5F0E1";
    public string PreviewBackgroundColor
    {
        get => _previewBackgroundColor;
        set => SetProperty(ref _previewBackgroundColor, value);
    }

    private string _previewTextColor = "#FF0F172A";
    public string PreviewTextColor
    {
        get => _previewTextColor;
        set => SetProperty(ref _previewTextColor, value);
    }

    private string _previewLinkColor = "#FF3B82F6";
    public string PreviewLinkColor
    {
        get => _previewLinkColor;
        set => SetProperty(ref _previewLinkColor, value);
    }

    private string _previewCodeBackgroundColor = "#FFF3F4F6";
    public string PreviewCodeBackgroundColor
    {
        get => _previewCodeBackgroundColor;
        set => SetProperty(ref _previewCodeBackgroundColor, value);
    }

    private string _previewCodeFontFamily = "Consolas";
    public string PreviewCodeFontFamily
    {
        get => _previewCodeFontFamily;
        set => SetProperty(ref _previewCodeFontFamily, value);
    }

    private string _previewFontFamily = "Microsoft YaHei UI";
    public string PreviewFontFamily
    {
        get => _previewFontFamily;
        set => SetProperty(ref _previewFontFamily, value);
    }

    private double _previewFontSize = 14;
    public double PreviewFontSize
    {
        get => _previewFontSize;
        set => SetProperty(ref _previewFontSize, value);
    }

    private double _previewLineHeight = 12;
    public double PreviewLineHeight
    {
        get => _previewLineHeight;
        set => SetProperty(ref _previewLineHeight, value);
    }

    public ObservableCollection<string> AvailableFontFamilies { get; } = new();

    public ObservableCollection<string> AvailableCodeFontFamilies { get; } = new();

    public DelegateCommand SaveCommand { get; }

    public DelegateCommand<string> ChooseColorCommand { get; }

    public void LoadFrom(string fontFamily, double fontSize, double lineHeight,
        string backgroundColor, string textColor, string linkColor,
        string codeBackgroundColor, string codeFontFamily)
    {
        PreviewFontFamily = fontFamily;
        PreviewFontSize = fontSize;
        PreviewLineHeight = lineHeight;
        PreviewBackgroundColor = backgroundColor;
        PreviewTextColor = textColor;
        PreviewLinkColor = linkColor;
        PreviewCodeBackgroundColor = codeBackgroundColor;
        PreviewCodeFontFamily = codeFontFamily;
    }

    private void ChooseColor(string propertyName)
    {
        var currentColor = propertyName switch
        {
            nameof(PreviewBackgroundColor) => PreviewBackgroundColor,
            nameof(PreviewTextColor) => PreviewTextColor,
            nameof(PreviewLinkColor) => PreviewLinkColor,
            nameof(PreviewCodeBackgroundColor) => PreviewCodeBackgroundColor,
            _ => "#FFFFFFFF"
        };

        var selectedColor = _colorSelectionService.ChooseColor(currentColor);
        if (string.IsNullOrWhiteSpace(selectedColor))
            return;

        switch (propertyName)
        {
            case nameof(PreviewBackgroundColor): PreviewBackgroundColor = selectedColor; break;
            case nameof(PreviewTextColor): PreviewTextColor = selectedColor; break;
            case nameof(PreviewLinkColor): PreviewLinkColor = selectedColor; break;
            case nameof(PreviewCodeBackgroundColor): PreviewCodeBackgroundColor = selectedColor; break;
        }
    }

    private async Task SaveCurrentSectionAsync()
    {
        if (!TryNormalizeColors(out var feedbackMessage))
        {
            _ = ShowSaveFeedbackAsync(feedbackMessage, true);
            return;
        }

        try
        {
            await _saveAsync();
        }
        catch (Exception)
        {
            _ = ShowSaveFeedbackAsync(_loc["Settings.SaveFailed"], true);
            return;
        }

        _ = ShowSaveFeedbackAsync(_loc["Settings.Saved"], false);
    }

    private bool TryNormalizeColors(out string feedbackMessage)
    {
        var fields = new (string LabelKey, Func<string> Getter, Action<string> Setter)[]
        {
            ("Appearance.PreviewBackground", () => PreviewBackgroundColor, v => PreviewBackgroundColor = v),
            ("Appearance.PreviewText", () => PreviewTextColor, v => PreviewTextColor = v),
            ("Appearance.PreviewLink", () => PreviewLinkColor, v => PreviewLinkColor = v),
            ("Appearance.PreviewCodeBackground", () => PreviewCodeBackgroundColor, v => PreviewCodeBackgroundColor = v)
        };

        foreach (var (labelKey, getter, setter) in fields)
        {
            if (!_appearanceSettingsService.TryNormalizeColor(getter(), out var normalizedColor))
            {
                feedbackMessage = string.Format(_loc["Settings.AppearanceColorFormatError"], _loc[labelKey]);
                return false;
            }

            setter(normalizedColor);
        }

        feedbackMessage = string.Empty;
        return true;
    }

    private static bool IsMonospace(System.Windows.Media.FontFamily family)
    {
        var name = family.Source.ToLowerInvariant();
        return name.Contains("consol") || name.Contains("courier") || name.Contains("mono")
               || name.Contains("code") || name.Contains("hack") || name.Contains("cascadia")
               || name.Contains("fira") || name.Contains("jetbrains") || name.Contains("source");
    }
}
