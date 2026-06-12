using Markdig;
using Prism.Commands;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Services;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.ViewModels;

public sealed class DayRecordViewModel : ViewModelBase
{
    private readonly IStatusMessageService _statusMessageService;
    private readonly IRecordStoreService _recordStoreService;
    private readonly IDiaryFileService _diaryFileService;
    private readonly ILocalizationService _loc;
    private readonly IAppearanceSettingsService _appearanceSettingsService;
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    private DateOnly _date = DateOnly.FromDateTime(DateTime.Today);
    private int _overtimeHours;
    private int _overtimeMinutes;
    private string _diaryMarkdown = string.Empty;
    private bool _isDirty;

    public DayRecordViewModel(
        IStatusMessageService statusMessageService,
        IRecordStoreService recordStoreService,
        IDiaryFileService diaryFileService,
        ILocalizationService localizationService,
        IAppearanceSettingsService appearanceSettingsService)
    {
        _statusMessageService = statusMessageService;
        _recordStoreService = recordStoreService;
        _diaryFileService = diaryFileService;
        _loc = localizationService;
        _appearanceSettingsService = appearanceSettingsService;
        SaveCommand = new DelegateCommand(() => _ = SaveAsync());

        _loc.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]")
            {
                RaisePropertyChanged(nameof(OvertimeDisplay));
            }
        };

        _appearanceSettingsService.PreviewSettingsChanged += () => RaisePropertyChanged(nameof(HtmlPreview));
    }

    public DateOnly Date
    {
        get => _date;
        set
        {
            if (SetProperty(ref _date, value))
            {
                RaisePropertyChanged(nameof(DateDisplay));
            }
        }
    }

    public string DateDisplay => Date.ToString("yyyy-MM-dd");

    public int OvertimeHours
    {
        get => _overtimeHours;
        set
        {
            if (SetProperty(ref _overtimeHours, value))
            {
                RaisePropertyChanged(nameof(OvertimeDisplay));
                IsDirty = true;
            }
        }
    }

    public int OvertimeMinutes
    {
        get => _overtimeMinutes;
        set
        {
            if (SetProperty(ref _overtimeMinutes, value))
            {
                RaisePropertyChanged(nameof(OvertimeDisplay));
                IsDirty = true;
            }
        }
    }

    public string OvertimeDisplay => string.Format(_loc["Calendar.OvertimeFormat"], OvertimeHours, OvertimeMinutes);

    public string DiaryMarkdown
    {
        get => _diaryMarkdown;
        set
        {
            if (SetProperty(ref _diaryMarkdown, value))
            {
                RaisePropertyChanged(nameof(HtmlPreview));
                IsDirty = true;
            }
        }
    }

    public string HtmlPreview
    {
        get
        {
            var resources = System.Windows.Application.Current.Resources;
            var bgColor = StripAlpha(resources["PreviewBackgroundColor"] as string ?? "#FFF5F0E1");

            if (string.IsNullOrWhiteSpace(DiaryMarkdown))
                return $"<html><body style='margin:0;background:{bgColor}'></body></html>";

            var body = Markdown.ToHtml(DiaryMarkdown, Pipeline);
            var fontFamily = (resources["PreviewFontFamily"] as System.Windows.Media.FontFamily)?.Source ?? "Microsoft YaHei UI";
            var fontSize = resources["PreviewFontSize"] as double? ?? 14;
            var textColor = StripAlpha(resources["PreviewTextColor"] as string ?? "#0F172A");
            var linkColor = StripAlpha(resources["PreviewLinkColor"] as string ?? "#3B82F6");
            var codeBg = StripAlpha(resources["PreviewCodeBackgroundColor"] as string ?? "#F3F4F6");
            var codeFont = resources["PreviewCodeFontFamily"] as string ?? "Consolas";

            var safeFont = fontFamily.Replace("'", "").Replace("\"", "");
            var safeCodeFont = codeFont.Replace("'", "").Replace("\"", "");

            return $@"<html><head><meta charset='utf-8'><style>
body {{ font-family: '{safeFont}'; font-size: {fontSize}px; line-height: 1.5; word-wrap: break-word;
       color: {textColor}; background: {bgColor}; padding: 16px; margin: 0; }}
h1, h2 {{ border-bottom: 1px solid #D8DEE4; padding-bottom: 0.3em; }}
h1 {{ font-size: 2em; margin: 4px 0 8px; }}
h2 {{ font-size: 1.5em; margin: 4px 0 8px; }}
h3 {{ font-size: 1.25em; margin: 4px 0 8px; }}
h4 {{ font-size: 1.1em; margin: 4px 0 8px; }}
table {{ border-collapse: collapse; width: 100%; margin: 8px 0; }}
th, td {{ border: 1px solid #D1D5DB; padding: 6px 13px; text-align: left; }}
th {{ background: rgba(0,0,0,0.04); font-weight: 600; }}
tr:nth-child(odd) {{ background: rgba(0,0,0,0.02); }}
tr:nth-child(even) {{ background: rgba(0,0,0,0.06); }}
code {{ background: {codeBg}; padding: .2em .4em; border-radius: 6px; font-size: 85%; font-family: '{safeCodeFont}'; }}
pre {{ background: {codeBg}; padding: 16px; border-radius: 6px; overflow-x: auto; line-height: 1.45; }}
pre code {{ background: none; padding: 0; font-size: 100%; }}
blockquote {{ border-left: 4px solid #D8DEE4; color: #656D76; padding: 0 16px; margin: 8px 0; }}
a {{ color: {linkColor}; }}
ul, ol {{ padding-left: 2em; }}
img {{ max-width: 100%; }}
</style></head><body>{body}</body></html>";
        }
    }

    public string BodyHtml
    {
        get
        {
            if (string.IsNullOrWhiteSpace(DiaryMarkdown))
                return "";

            return Markdown.ToHtml(DiaryMarkdown, Pipeline);
        }
    }

    public bool IsDirty
    {
        get => _isDirty;
        private set => SetProperty(ref _isDirty, value);
    }

    public DelegateCommand SaveCommand { get; }

    public event Action? Saved;

    public async Task LoadAsync(DateOnly date)
    {
        Date = date;
        IsDirty = false;

        var record = await _recordStoreService.LoadAsync(date);
        if (record is not null)
        {
            OvertimeHours = record.OvertimeHours;
            OvertimeMinutes = record.OvertimeMinutes;
        }
        else
        {
            OvertimeHours = 0;
            OvertimeMinutes = 0;
        }

        var diary = await _diaryFileService.LoadDiaryAsync(date);
        DiaryMarkdown = diary;

        IsDirty = false;
    }

    private async Task SaveAsync()
    {
        try
        {
            var record = new DailyRecord
            {
                Date = Date,
                OvertimeHours = OvertimeHours,
                OvertimeMinutes = OvertimeMinutes,
                DiaryMarkdown = DiaryMarkdown,
                LastModified = DateTime.Now
            };

            await _recordStoreService.SaveAsync(record);

            if (string.IsNullOrWhiteSpace(DiaryMarkdown))
            {
                await _diaryFileService.DeleteDiaryAsync(Date);
            }
            else
            {
                await _diaryFileService.SaveDiaryAsync(Date, DiaryMarkdown);
            }

            IsDirty = false;
            _statusMessageService.Show(string.Format(_loc["Diary.Saved"], DateDisplay));
            Saved?.Invoke();
        }
        catch (Exception)
        {
            _statusMessageService.Show(_loc["Diary.SaveFailed"]);
        }
    }

    private static string StripAlpha(string? color)
    {
        if (string.IsNullOrEmpty(color) || color.Length < 7 || !color.StartsWith("#"))
            return color ?? "#000";
        return "#" + color.Substring(3, 6);
    }
}
