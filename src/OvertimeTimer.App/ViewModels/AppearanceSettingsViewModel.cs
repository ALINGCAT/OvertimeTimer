using System.Collections.ObjectModel;
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
        ChooseColorCommand = new DelegateCommand<string>(ChooseColor);

        foreach (var preset in AppearancePresets.All)
            AvailablePresets.Add(preset);
        AvailablePresets.Add(AppearancePresets.Custom);

        PropertyChanged += (_, _) =>
        {
            if (!_matchingPreset) MatchPreset();
            ScheduleAutoSave(() => SaveCurrentSectionAsync());
        };
    }

    public ObservableCollection<AppearancePresets.Preset> AvailablePresets { get; } = new();

    private PreviewSettingsViewModel? _previewSection;

    public void SetPreviewSection(PreviewSettingsViewModel previewSection) => _previewSection = previewSection;

    public event Action<AppearancePresets.Preset>? PresetSelected;

    private AppearancePresets.Preset? _selectedPreset;
    public AppearancePresets.Preset? SelectedPreset
    {
        get => _selectedPreset;
        set
        {
            if (value is null || !SetProperty(ref _selectedPreset, value)) return;

            WindowBackgroundColor = value.Config.WindowBackgroundColor;
            CalendarWorkdayColor = value.Config.CalendarWorkdayColor;
            CalendarRestDayColor = value.Config.CalendarRestDayColor;
            CalendarTodayColor = value.Config.CalendarTodayColor;
            CalendarOutOfMonthColor = value.Config.CalendarOutOfMonthColor;
            CalendarOvertimeDotColor = value.Config.CalendarOvertimeDotColor;
            CalendarDiaryDotColor = value.Config.CalendarDiaryDotColor;
            CalendarHolidayColor = value.Config.CalendarHolidayColor;
            CalendarAdjustWorkdayColor = value.Config.CalendarAdjustWorkdayColor;
            CalendarLeaveColor = value.Config.CalendarLeaveColor;
            CardBackgroundColor = value.Config.CardBackgroundColor;
            CardBorderColor = value.Config.CardBorderColor;
            CalendarDayBorderColor = value.Config.CalendarDayBorderColor;

            _previewSection?.ApplyPreset(value);
            PresetSelected?.Invoke(value);
        }
    }

    private string _windowBackgroundColor = "#FFF0F9FF";
    public string WindowBackgroundColor { get => _windowBackgroundColor; set => SetProperty(ref _windowBackgroundColor, value); }
    private string _calendarWorkdayColor = "#FF0C4A6E";
    public string CalendarWorkdayColor { get => _calendarWorkdayColor; set => SetProperty(ref _calendarWorkdayColor, value); }
    private string _calendarRestDayColor = "#FF7DD3FC";
    public string CalendarRestDayColor { get => _calendarRestDayColor; set => SetProperty(ref _calendarRestDayColor, value); }
    private string _calendarTodayColor = "#FF22C55E";
    public string CalendarTodayColor { get => _calendarTodayColor; set => SetProperty(ref _calendarTodayColor, value); }
    private string _calendarOutOfMonthColor = "#FFE0F2FE";
    public string CalendarOutOfMonthColor { get => _calendarOutOfMonthColor; set => SetProperty(ref _calendarOutOfMonthColor, value); }
    private string _calendarOvertimeDotColor = "#FFDC2626";
    public string CalendarOvertimeDotColor { get => _calendarOvertimeDotColor; set => SetProperty(ref _calendarOvertimeDotColor, value); }
    private string _calendarDiaryDotColor = "#FF16A34A";
    public string CalendarDiaryDotColor { get => _calendarDiaryDotColor; set => SetProperty(ref _calendarDiaryDotColor, value); }
    private string _calendarHolidayColor = "#FF8B5CF6";
    public string CalendarHolidayColor { get => _calendarHolidayColor; set => SetProperty(ref _calendarHolidayColor, value); }
    private string _calendarAdjustWorkdayColor = "#FFB91C1C";
    public string CalendarAdjustWorkdayColor { get => _calendarAdjustWorkdayColor; set => SetProperty(ref _calendarAdjustWorkdayColor, value); }
    private string _calendarLeaveColor = "#FF0284C7";
    public string CalendarLeaveColor { get => _calendarLeaveColor; set => SetProperty(ref _calendarLeaveColor, value); }
    private string _calendarDayBorderColor = "#FFBAE6FD";
    public string CalendarDayBorderColor { get => _calendarDayBorderColor; set => SetProperty(ref _calendarDayBorderColor, value); }
    private string _cardBackgroundColor = "#FFFFFFFF";
    public string CardBackgroundColor { get => _cardBackgroundColor; set => SetProperty(ref _cardBackgroundColor, value); }
    private string _cardBorderColor = "#FFBAE6FD";
    public string CardBorderColor { get => _cardBorderColor; set => SetProperty(ref _cardBorderColor, value); }

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
        CardBackgroundColor = appearanceConfig.CardBackgroundColor;
        CardBorderColor = appearanceConfig.CardBorderColor;
        CalendarLeaveColor = appearanceConfig.CalendarLeaveColor;
        CalendarDayBorderColor = appearanceConfig.CalendarDayBorderColor;

        MatchPreset();
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
            CalendarAdjustWorkdayColor = CalendarAdjustWorkdayColor,
            CardBackgroundColor = CardBackgroundColor,
            CardBorderColor = CardBorderColor,
            CalendarLeaveColor = CalendarLeaveColor,
            CalendarDayBorderColor = CalendarDayBorderColor
        };
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
            nameof(CardBackgroundColor) => CardBackgroundColor,
            nameof(CardBorderColor) => CardBorderColor,
            nameof(CalendarLeaveColor) => CalendarLeaveColor,
            nameof(CalendarDayBorderColor) => CalendarDayBorderColor,
            _ => "#FFFFFFFF"
        };

        var selectedColor = _colorSelectionService.ChooseColor(currentColor);
        if (string.IsNullOrWhiteSpace(selectedColor)) return;

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
            case nameof(CardBackgroundColor): CardBackgroundColor = selectedColor; break;
            case nameof(CardBorderColor): CardBorderColor = selectedColor; break;
            case nameof(CalendarLeaveColor): CalendarLeaveColor = selectedColor; break;
            case nameof(CalendarDayBorderColor): CalendarDayBorderColor = selectedColor; break;
        }
    }

    private async Task SaveCurrentSectionAsync()
    {
        if (!TryNormalizeAppearanceColors(out _)) return;
        try
        {
            _appearanceSettingsService.Apply(ToModel());
            await _saveAsync();
        }
        catch { }
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
            ("Appearance.CalendarAdjustWorkday", () => CalendarAdjustWorkdayColor, v => CalendarAdjustWorkdayColor = v),
            ("Appearance.CardBackground", () => CardBackgroundColor, v => CardBackgroundColor = v),
            ("Appearance.CardBorder", () => CardBorderColor, v => CardBorderColor = v),
            ("Appearance.CalendarLeave", () => CalendarLeaveColor, v => CalendarLeaveColor = v),
            ("Appearance.CalendarDayBorder", () => CalendarDayBorderColor, v => CalendarDayBorderColor = v)
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

    private bool _matchingPreset;

    private void MatchPreset()
    {
        _matchingPreset = true;
        var config = ToModel();
        foreach (var preset in AppearancePresets.All)
        {
            if (preset.IsMatch(config))
            {
                _selectedPreset = preset;
                RaisePropertyChanged(nameof(SelectedPreset));
                _matchingPreset = false;
                return;
            }
        }

        if (_selectedPreset != AppearancePresets.Custom)
        {
            _selectedPreset = AppearancePresets.Custom;
            RaisePropertyChanged(nameof(SelectedPreset));
        }
        _matchingPreset = false;
    }
}
