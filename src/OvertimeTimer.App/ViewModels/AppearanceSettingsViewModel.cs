using Prism.Commands;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Services;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.ViewModels;

public sealed class AppearanceSettingsViewModel : SettingsSectionViewModelBase
{
    private readonly IAppearanceSettingsService _appearanceSettingsService;
    private readonly IColorSelectionService _colorSelectionService;
    private readonly Func<Task> _saveAsync;
    private readonly ILocalizationService _loc;

    public AppearanceSettingsViewModel(
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
    }

    private string _windowBackgroundColor = "#FFEAF3FF";
    public string WindowBackgroundColor
    {
        get => _windowBackgroundColor;
        set => SetProperty(ref _windowBackgroundColor, value);
    }

    private string _calendarWorkdayColor = "#FF0F172A";
    public string CalendarWorkdayColor
    {
        get => _calendarWorkdayColor;
        set => SetProperty(ref _calendarWorkdayColor, value);
    }

    private string _calendarRestDayColor = "#FF94A3B8";
    public string CalendarRestDayColor
    {
        get => _calendarRestDayColor;
        set => SetProperty(ref _calendarRestDayColor, value);
    }

    private string _calendarTodayColor = "#FF22C55E";
    public string CalendarTodayColor
    {
        get => _calendarTodayColor;
        set => SetProperty(ref _calendarTodayColor, value);
    }

    private string _calendarOutOfMonthColor = "#FFE2E8F0";
    public string CalendarOutOfMonthColor
    {
        get => _calendarOutOfMonthColor;
        set => SetProperty(ref _calendarOutOfMonthColor, value);
    }

    private string _calendarOvertimeDotColor = "#FFDC2626";
    public string CalendarOvertimeDotColor
    {
        get => _calendarOvertimeDotColor;
        set => SetProperty(ref _calendarOvertimeDotColor, value);
    }

    private string _calendarDiaryDotColor = "#FF16A34A";
    public string CalendarDiaryDotColor
    {
        get => _calendarDiaryDotColor;
        set => SetProperty(ref _calendarDiaryDotColor, value);
    }

    private string _previewBackgroundColor = "#FFF5F0E1";
    public string PreviewBackgroundColor
    {
        get => _previewBackgroundColor;
        set => SetProperty(ref _previewBackgroundColor, value);
    }

    private string _calendarHolidayColor = "#FF8B5CF6";
    public string CalendarHolidayColor
    {
        get => _calendarHolidayColor;
        set => SetProperty(ref _calendarHolidayColor, value);
    }

    private string _calendarAdjustWorkdayColor = "#FFB91C1C";
    public string CalendarAdjustWorkdayColor
    {
        get => _calendarAdjustWorkdayColor;
        set => SetProperty(ref _calendarAdjustWorkdayColor, value);
    }

    public DelegateCommand SaveCommand { get; }

    public DelegateCommand<string> ChooseColorCommand { get; }

    public void LoadFrom(AppearanceConfig appearanceConfig)
    {
        WindowBackgroundColor = appearanceConfig.WindowBackgroundColor;
        CalendarWorkdayColor = appearanceConfig.CalendarWorkdayColor;
        CalendarRestDayColor = appearanceConfig.CalendarRestDayColor;
        CalendarTodayColor = appearanceConfig.CalendarTodayColor;
        CalendarOutOfMonthColor = appearanceConfig.CalendarOutOfMonthColor;
        CalendarOvertimeDotColor = appearanceConfig.CalendarOvertimeDotColor;
        CalendarDiaryDotColor = appearanceConfig.CalendarDiaryDotColor;
        CalendarHolidayColor = appearanceConfig.CalendarHolidayColor;
        CalendarAdjustWorkdayColor = appearanceConfig.CalendarAdjustWorkdayColor;
    }

    private void ChooseColor(string propertyName)
    {
        var currentColor = propertyName switch
        {
            nameof(WindowBackgroundColor) => WindowBackgroundColor,
            nameof(CalendarWorkdayColor) => CalendarWorkdayColor,
            nameof(CalendarRestDayColor) => CalendarRestDayColor,
            nameof(CalendarTodayColor) => CalendarTodayColor,
            nameof(CalendarOutOfMonthColor) => CalendarOutOfMonthColor,
            nameof(CalendarOvertimeDotColor) => CalendarOvertimeDotColor,
            nameof(CalendarDiaryDotColor) => CalendarDiaryDotColor,
            nameof(CalendarHolidayColor) => CalendarHolidayColor,
            nameof(CalendarAdjustWorkdayColor) => CalendarAdjustWorkdayColor,
            _ => "#FFFFFFFF"
        };

        var selectedColor = _colorSelectionService.ChooseColor(currentColor);
        if (string.IsNullOrWhiteSpace(selectedColor))
        {
            return;
        }

        switch (propertyName)
        {
            case nameof(WindowBackgroundColor): WindowBackgroundColor = selectedColor; break;
            case nameof(CalendarWorkdayColor): CalendarWorkdayColor = selectedColor; break;
            case nameof(CalendarRestDayColor): CalendarRestDayColor = selectedColor; break;
            case nameof(CalendarTodayColor): CalendarTodayColor = selectedColor; break;
            case nameof(CalendarOutOfMonthColor): CalendarOutOfMonthColor = selectedColor; break;
            case nameof(CalendarOvertimeDotColor): CalendarOvertimeDotColor = selectedColor; break;
            case nameof(CalendarDiaryDotColor): CalendarDiaryDotColor = selectedColor; break;
            case nameof(CalendarHolidayColor): CalendarHolidayColor = selectedColor; break;
            case nameof(CalendarAdjustWorkdayColor): CalendarAdjustWorkdayColor = selectedColor; break;
        }
    }

    public AppearanceConfig ToModel()
    {
        return new AppearanceConfig
        {
            WindowBackgroundColor = WindowBackgroundColor,
            CalendarWorkdayColor = CalendarWorkdayColor,
            CalendarRestDayColor = CalendarRestDayColor,
            CalendarTodayColor = CalendarTodayColor,
            CalendarOutOfMonthColor = CalendarOutOfMonthColor,
            CalendarOvertimeDotColor = CalendarOvertimeDotColor,
            CalendarDiaryDotColor = CalendarDiaryDotColor,
            CalendarHolidayColor = CalendarHolidayColor,
            CalendarAdjustWorkdayColor = CalendarAdjustWorkdayColor
        };
    }

    private async Task SaveCurrentSectionAsync()
    {
        if (!TryNormalizeAppearanceColors(out var feedbackMessage))
        {
            _ = ShowSaveFeedbackAsync(feedbackMessage, true);
            return;
        }

        try
        {
            _appearanceSettingsService.Apply(ToModel());
            await _saveAsync();
        }
        catch (Exception)
        {
            _ = ShowSaveFeedbackAsync(_loc["Settings.SaveFailed"], true);
            return;
        }

        _ = ShowSaveFeedbackAsync(_loc["Settings.Saved"], false);
    }

    private bool TryNormalizeAppearanceColors(out string feedbackMessage)
    {
        var fields = new (string LabelKey, Func<string> Getter, Action<string> Setter)[]
        {
            ("Appearance.WindowBackground", () => WindowBackgroundColor, v => WindowBackgroundColor = v),
            ("Appearance.CalendarWorkday", () => CalendarWorkdayColor, v => CalendarWorkdayColor = v),
            ("Appearance.CalendarRestDay", () => CalendarRestDayColor, v => CalendarRestDayColor = v),
            ("Appearance.CalendarToday", () => CalendarTodayColor, v => CalendarTodayColor = v),
            ("Appearance.CalendarOutOfMonth", () => CalendarOutOfMonthColor, v => CalendarOutOfMonthColor = v),
            ("Appearance.CalendarOvertimeDot", () => CalendarOvertimeDotColor, v => CalendarOvertimeDotColor = v),
            ("Appearance.CalendarDiaryDot", () => CalendarDiaryDotColor, v => CalendarDiaryDotColor = v),
            ("Appearance.CalendarHoliday", () => CalendarHolidayColor, v => CalendarHolidayColor = v),
            ("Appearance.CalendarAdjustWorkday", () => CalendarAdjustWorkdayColor, v => CalendarAdjustWorkdayColor = v)
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
}
