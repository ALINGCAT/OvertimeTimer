using System.Collections.ObjectModel;
using System.IO;
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
    private readonly ISettingsInteractionService _settingsInteractionService;

    public PreviewSettingsViewModel(
        IAppearanceSettingsService appearanceSettingsService,
        IColorSelectionService colorSelectionService,
        Func<Task> saveAsync,
        ILocalizationService localizationService,
        ISettingsInteractionService settingsInteractionService)
    {
        _appearanceSettingsService = appearanceSettingsService;
        _colorSelectionService = colorSelectionService;
        _saveAsync = saveAsync;
        _loc = localizationService;
        _settingsInteractionService = settingsInteractionService;
        SaveCommand = new DelegateCommand(() => _ = SaveCurrentSectionAsync());
        ChooseColorCommand = new DelegateCommand<string>(ChooseColor);
        ExportCssCommand = new DelegateCommand(ExportCss);
        ImportCssCommand = new DelegateCommand(ImportCss);

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

    public DelegateCommand ExportCssCommand { get; }

    public DelegateCommand ImportCssCommand { get; }

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

    private void ExportCss()
    {
        var css = BuildCssContent();
        var path = _settingsInteractionService.SaveFile("CSS files (*.css)|*.css", "preview.css");
        if (path is null) return;

        try
        {
            File.WriteAllText(path, css);
        }
        catch (Exception)
        {
            _ = ShowSaveFeedbackAsync("导出失败", true);
        }
    }

    private void ImportCss()
    {
        var path = _settingsInteractionService.OpenFile("CSS files (*.css)|*.css");
        if (path is null) return;

        try
        {
            var css = File.ReadAllText(path);
            ParseCssContent(css);
        }
        catch (Exception)
        {
            _ = ShowSaveFeedbackAsync("导入失败，文件格式不正确", true);
        }
    }

    private string BuildCssContent()
    {
        var font = PreviewFontFamily;
        var codeFont = PreviewCodeFontFamily;
        var bg = StripAlphaForCss(PreviewBackgroundColor);
        var fg = StripAlphaForCss(PreviewTextColor);
        var link = StripAlphaForCss(PreviewLinkColor);
        var codeBg = StripAlphaForCss(PreviewCodeBackgroundColor);

        return $@"/* OvertimeTimer Preview CSS */
body {{
    font-family: '{font}';
    font-size: {PreviewFontSize}px;
    line-height: {PreviewLineHeight / PreviewFontSize:F2};
    word-wrap: break-word;
    color: {fg};
    background: {bg};
    padding: 16px;
    margin: 0;
}}
h1, h2 {{ border-bottom: 1px solid #D8DEE4; padding-bottom: 0.3em; }}
h1 {{ font-size: 2em; margin: 24px 0 16px; }}
h2 {{ font-size: 1.5em; margin: 24px 0 16px; }}
h3 {{ font-size: 1.25em; margin: 24px 0 16px; }}
table {{ border-collapse: collapse; width: 100%; margin: 8px 0; }}
th, td {{ border: 1px solid #D1D5DB; padding: 6px 13px; text-align: left; }}
th {{ background: rgba(0,0,0,0.04); font-weight: 600; }}
tr:nth-child(odd) {{ background: rgba(0,0,0,0.02); }}
tr:nth-child(even) {{ background: rgba(0,0,0,0.06); }}
code {{ background: {codeBg}; padding: .2em .4em; border-radius: 6px; font-size: 85%; font-family: '{codeFont}'; }}
pre {{ background: {codeBg}; padding: 16px; border-radius: 6px; overflow-x: auto; line-height: 1.45; }}
pre code {{ background: none; padding: 0; font-size: 100%; }}
blockquote {{ border-left: 4px solid #D8DEE4; color: #656D76; padding: 0 16px; margin: 8px 0; }}
a {{ color: {link}; }}
ul, ol {{ padding-left: 2em; }}
img {{ max-width: 100%; }}
";
    }

    private void ParseCssContent(string css)
    {
        var font = ExtractCssValue(css, "body", "font-family");
        if (!string.IsNullOrWhiteSpace(font))
            PreviewFontFamily = font.Split(',')[0].Trim().Trim('\'', '"');

        var size = ExtractCssValue(css, "body", "font-size");
        if (!string.IsNullOrWhiteSpace(size) && double.TryParse(size.Replace("px", ""), out var sz))
            PreviewFontSize = sz;

        var bg = ExtractCssValue(css, "body", "background");
        if (!string.IsNullOrWhiteSpace(bg))
            PreviewBackgroundColor = AddAlphaPrefix(bg);

        var fg = ExtractCssValue(css, "body", "color");
        if (!string.IsNullOrWhiteSpace(fg))
            PreviewTextColor = AddAlphaPrefix(fg);

        var link = ExtractCssValue(css, "a", "color");
        if (!string.IsNullOrWhiteSpace(link))
            PreviewLinkColor = AddAlphaPrefix(link);

        var codeBg = ExtractCssValue(css, "code", "background");
        if (!string.IsNullOrWhiteSpace(codeBg))
            PreviewCodeBackgroundColor = AddAlphaPrefix(codeBg);

        var codeFont = ExtractCssValue(css, "code", "font-family");
        if (!string.IsNullOrWhiteSpace(codeFont))
            PreviewCodeFontFamily = codeFont.Split(',')[0].Trim().Trim('\'', '"');
    }

    private static string? ExtractCssValue(string css, string selector, string property)
    {
        var start = css.IndexOf(selector, StringComparison.Ordinal);
        if (start < 0) return null;

        var propStart = css.IndexOf(property, start, StringComparison.Ordinal);
        if (propStart < 0) return null;

        var colon = css.IndexOf(':', propStart);
        if (colon < 0) return null;

        var semi = css.IndexOf(';', colon);
        if (semi < 0) semi = css.IndexOf('}', colon);

        var value = css.Substring(colon + 1, semi - colon - 1).Trim();
        return value;
    }

    private static string StripAlphaForCss(string color)
    {
        if (color.Length >= 9 && color.StartsWith("#"))
            return "#" + color.Substring(3);
        return color;
    }

    private static string AddAlphaPrefix(string cssColor)
    {
        if (cssColor.StartsWith("#") && cssColor.Length == 7)
            return "#FF" + cssColor.Substring(1);
        if (cssColor.StartsWith("#") && cssColor.Length == 4)
            return "#FF" + cssColor.Substring(1);
        return cssColor;
    }
}
