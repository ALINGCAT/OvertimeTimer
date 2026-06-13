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
        ChooseColorCommand = new DelegateCommand<string>(ChooseColor);

        foreach (var family in Fonts.SystemFontFamilies.OrderBy(f => f.Source))
            AvailableFontFamilies.Add(family.Source);
        foreach (var family in Fonts.SystemFontFamilies.Where(IsMonospace).OrderBy(f => f.Source))
            AvailableCodeFontFamilies.Add(family.Source);

        PropertyChanged += (_, _) => ScheduleAutoSave(() => SaveCurrentSectionAsync());
    }

    public void ApplyPreset(AppearancePresets.Preset preset)
    {
        PreviewFontFamily = preset.PreviewFontFamily;
        PreviewFontSize = preset.PreviewFontSize;
        PreviewLineHeight = preset.PreviewLineHeight;
        PreviewBackgroundColor = preset.PreviewBackgroundColor;
        PreviewTextColor = preset.PreviewTextColor;
        PreviewLinkColor = preset.PreviewLinkColor;
        PreviewCodeBackgroundColor = preset.PreviewCodeBackgroundColor;
        PreviewCodeFontFamily = preset.PreviewCodeFontFamily;

        _appearanceSettingsService.ApplyPreviewSettings(
            PreviewFontFamily, PreviewFontSize, PreviewLineHeight,
            PreviewBackgroundColor, PreviewTextColor, PreviewLinkColor,
            PreviewCodeBackgroundColor, PreviewCodeFontFamily);
    }

    private string _previewBackgroundColor = "#FFFFFFFF";
    public string PreviewBackgroundColor { get => _previewBackgroundColor; set => SetProperty(ref _previewBackgroundColor, value); }
    private string _previewTextColor = "#FF0C4A6E";
    public string PreviewTextColor { get => _previewTextColor; set => SetProperty(ref _previewTextColor, value); }
    private string _previewLinkColor = "#FF0284C7";
    public string PreviewLinkColor { get => _previewLinkColor; set => SetProperty(ref _previewLinkColor, value); }
    private string _previewCodeBackgroundColor = "#FFE0F2FE";
    public string PreviewCodeBackgroundColor { get => _previewCodeBackgroundColor; set => SetProperty(ref _previewCodeBackgroundColor, value); }
    private string _previewCodeFontFamily = "Consolas";
    public string PreviewCodeFontFamily { get => _previewCodeFontFamily; set => SetProperty(ref _previewCodeFontFamily, value); }
    private string _previewFontFamily = "Microsoft YaHei UI";
    public string PreviewFontFamily { get => _previewFontFamily; set => SetProperty(ref _previewFontFamily, value); }
    private double _previewFontSize = 14;
    public double PreviewFontSize { get => _previewFontSize; set => SetProperty(ref _previewFontSize, value); }
    private double _previewLineHeight = 12;
    public double PreviewLineHeight { get => _previewLineHeight; set => SetProperty(ref _previewLineHeight, value); }

    public ObservableCollection<string> AvailableFontFamilies { get; } = new();
    public ObservableCollection<string> AvailableCodeFontFamilies { get; } = new();
    public DelegateCommand<string> ChooseColorCommand { get; }

    public void LoadFrom(string fontFamily, double fontSize, double lineHeight,
        string backgroundColor, string textColor, string linkColor, string codeBackgroundColor, string codeFontFamily)
    {
        PreviewFontFamily = fontFamily; PreviewFontSize = fontSize; PreviewLineHeight = lineHeight;
        PreviewBackgroundColor = backgroundColor; PreviewTextColor = textColor; PreviewLinkColor = linkColor;
        PreviewCodeBackgroundColor = codeBackgroundColor; PreviewCodeFontFamily = codeFontFamily;
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
        var c = _colorSelectionService.ChooseColor(currentColor);
        if (string.IsNullOrWhiteSpace(c)) return;
        switch (propertyName)
        {
            case nameof(PreviewBackgroundColor): PreviewBackgroundColor = c; break;
            case nameof(PreviewTextColor): PreviewTextColor = c; break;
            case nameof(PreviewLinkColor): PreviewLinkColor = c; break;
            case nameof(PreviewCodeBackgroundColor): PreviewCodeBackgroundColor = c; break;
        }
    }

    private async Task SaveCurrentSectionAsync()
    {
        if (!TryNormalizeColors(out _)) return;
        try { await _saveAsync(); }
        catch { }
    }

    private bool TryNormalizeColors(out string msg)
    {
        var fields = new (string Lk, Func<string> G, Action<string> S)[]
        {
            ("Appearance.PreviewBackground", () => PreviewBackgroundColor, v => PreviewBackgroundColor = v),
            ("Appearance.PreviewText", () => PreviewTextColor, v => PreviewTextColor = v),
            ("Appearance.PreviewLink", () => PreviewLinkColor, v => PreviewLinkColor = v),
            ("Appearance.PreviewCodeBackground", () => PreviewCodeBackgroundColor, v => PreviewCodeBackgroundColor = v)
        };
        foreach (var (lk, g, s) in fields)
        {
            if (!_appearanceSettingsService.TryNormalizeColor(g(), out var nc)) { msg = string.Format(_loc["Settings.AppearanceColorFormatError"], _loc[lk]); return false; }
            s(nc);
        }
        msg = string.Empty; return true;
    }
    private static bool IsMonospace(System.Windows.Media.FontFamily f) { var n = f.Source.ToLowerInvariant(); return n.Contains("consol") || n.Contains("courier") || n.Contains("mono") || n.Contains("code") || n.Contains("hack") || n.Contains("cascadia") || n.Contains("fira") || n.Contains("jetbrains") || n.Contains("source"); }
}
