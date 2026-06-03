using System.Collections.ObjectModel;
using Prism.Commands;
using OvertimeTimer.App.Services;
using OvertimeTimer.Core.Models;

namespace OvertimeTimer.App.ViewModels;

public sealed class AppearanceSettingsViewModel : SettingsSectionViewModelBase
{
    private const string SaveSuccessMessage = "外观设置已保存。";

    private readonly IAppearanceSettingsService _appearanceSettingsService;
    private readonly IColorSelectionService _colorSelectionService;
    private readonly Func<Task> _saveAsync;
    private readonly ObservableCollection<AppearanceColorItemViewModel> _appearanceColorItems = new();
    private string _windowBackgroundColor = "#FFEAF3FF";
    private string _calendarWorkdayColor = "#FF0F172A";
    private string _calendarRestDayColor = "#FF94A3B8";
    private string _calendarTodayColor = "#FF22C55E";
    private string _calendarOutOfMonthColor = "#FFE2E8F0";
    private string _calendarOvertimeDotColor = "#FFDC2626";
    private string _calendarDiaryDotColor = "#FF16A34A";

    public AppearanceSettingsViewModel(
        IAppearanceSettingsService appearanceSettingsService,
        IColorSelectionService colorSelectionService,
        Func<Task> saveAsync)
    {
        _appearanceSettingsService = appearanceSettingsService;
        _colorSelectionService = colorSelectionService;
        _saveAsync = saveAsync;
        AppearanceColorItems = _appearanceColorItems;
        InitializeAppearanceColorItems();
        SaveCommand = new DelegateCommand(() => _ = SaveCurrentSectionAsync());
    }

    public string WindowBackgroundColor
    {
        get => _windowBackgroundColor;
        set
        {
            if (SetProperty(ref _windowBackgroundColor, value))
            {
                UpdateAppearanceColorItem(nameof(WindowBackgroundColor), value);
            }
        }
    }

    public string CalendarWorkdayColor
    {
        get => _calendarWorkdayColor;
        set
        {
            if (SetProperty(ref _calendarWorkdayColor, value))
            {
                UpdateAppearanceColorItem(nameof(CalendarWorkdayColor), value);
            }
        }
    }

    public string CalendarRestDayColor
    {
        get => _calendarRestDayColor;
        set
        {
            if (SetProperty(ref _calendarRestDayColor, value))
            {
                UpdateAppearanceColorItem(nameof(CalendarRestDayColor), value);
            }
        }
    }

    public string CalendarTodayColor
    {
        get => _calendarTodayColor;
        set
        {
            if (SetProperty(ref _calendarTodayColor, value))
            {
                UpdateAppearanceColorItem(nameof(CalendarTodayColor), value);
            }
        }
    }

    public string CalendarOutOfMonthColor
    {
        get => _calendarOutOfMonthColor;
        set
        {
            if (SetProperty(ref _calendarOutOfMonthColor, value))
            {
                UpdateAppearanceColorItem(nameof(CalendarOutOfMonthColor), value);
            }
        }
    }

    public string CalendarOvertimeDotColor
    {
        get => _calendarOvertimeDotColor;
        set
        {
            if (SetProperty(ref _calendarOvertimeDotColor, value))
            {
                UpdateAppearanceColorItem(nameof(CalendarOvertimeDotColor), value);
            }
        }
    }

    public string CalendarDiaryDotColor
    {
        get => _calendarDiaryDotColor;
        set
        {
            if (SetProperty(ref _calendarDiaryDotColor, value))
            {
                UpdateAppearanceColorItem(nameof(CalendarDiaryDotColor), value);
            }
        }
    }

    public ObservableCollection<AppearanceColorItemViewModel> AppearanceColorItems { get; }

    public DelegateCommand SaveCommand { get; }

    public void LoadFrom(AppearanceConfig appearanceConfig)
    {
        WindowBackgroundColor = appearanceConfig.WindowBackgroundColor;
        CalendarWorkdayColor = appearanceConfig.CalendarWorkdayColor;
        CalendarRestDayColor = appearanceConfig.CalendarRestDayColor;
        CalendarTodayColor = appearanceConfig.CalendarTodayColor;
        CalendarOutOfMonthColor = appearanceConfig.CalendarOutOfMonthColor;
        CalendarOvertimeDotColor = appearanceConfig.CalendarOvertimeDotColor;
        CalendarDiaryDotColor = appearanceConfig.CalendarDiaryDotColor;
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
            CalendarDiaryDotColor = CalendarDiaryDotColor
        };
    }

    private async Task SaveCurrentSectionAsync()
    {
        if (!TryNormalizeAppearanceColors(out var feedbackMessage))
        {
            await ShowSaveFeedbackAsync(feedbackMessage, true);
            return;
        }

        try
        {
            _appearanceSettingsService.Apply(ToModel());
            await _saveAsync();
        }
        catch (Exception)
        {
            await ShowSaveFeedbackAsync("外观设置保存失败。", true);
            return;
        }

        await ShowSaveFeedbackAsync(SaveSuccessMessage, false);
    }

    private bool TryNormalizeAppearanceColors(out string feedbackMessage)
    {
        var colorFields = new (string Label, Func<string> Getter, Action<string> Setter)[]
        {
            ("窗口背景色", () => WindowBackgroundColor, value => WindowBackgroundColor = value),
            ("工作日文字色", () => CalendarWorkdayColor, value => CalendarWorkdayColor = value),
            ("休息日文字色", () => CalendarRestDayColor, value => CalendarRestDayColor = value),
            ("今日文字色", () => CalendarTodayColor, value => CalendarTodayColor = value),
            ("非本月文字色", () => CalendarOutOfMonthColor, value => CalendarOutOfMonthColor = value),
            ("加班提示点色", () => CalendarOvertimeDotColor, value => CalendarOvertimeDotColor = value),
            ("日记提示点色", () => CalendarDiaryDotColor, value => CalendarDiaryDotColor = value)
        };

        foreach (var colorField in colorFields)
        {
            if (!_appearanceSettingsService.TryNormalizeColor(colorField.Getter(), out var normalizedColor))
            {
                feedbackMessage = $"{colorField.Label}格式无效，请输入如 #FFEAF3FF 的颜色值。";
                return false;
            }

            colorField.Setter(normalizedColor);
        }

        feedbackMessage = string.Empty;
        return true;
    }

    private void InitializeAppearanceColorItems()
    {
        _appearanceColorItems.Clear();
        _appearanceColorItems.Add(new AppearanceColorItemViewModel("窗口背景色", WindowBackgroundColor, ChooseAppearanceColor, OnAppearanceColorTextChanged));
        _appearanceColorItems.Add(new AppearanceColorItemViewModel("工作日文字色", CalendarWorkdayColor, ChooseAppearanceColor, OnAppearanceColorTextChanged));
        _appearanceColorItems.Add(new AppearanceColorItemViewModel("休息日文字色", CalendarRestDayColor, ChooseAppearanceColor, OnAppearanceColorTextChanged));
        _appearanceColorItems.Add(new AppearanceColorItemViewModel("今日文字色", CalendarTodayColor, ChooseAppearanceColor, OnAppearanceColorTextChanged));
        _appearanceColorItems.Add(new AppearanceColorItemViewModel("非本月文字色", CalendarOutOfMonthColor, ChooseAppearanceColor, OnAppearanceColorTextChanged));
        _appearanceColorItems.Add(new AppearanceColorItemViewModel("加班提示点色", CalendarOvertimeDotColor, ChooseAppearanceColor, OnAppearanceColorTextChanged));
        _appearanceColorItems.Add(new AppearanceColorItemViewModel("日记提示点色", CalendarDiaryDotColor, ChooseAppearanceColor, OnAppearanceColorTextChanged));
    }

    private void ChooseAppearanceColor(AppearanceColorItemViewModel colorItem)
    {
        var selectedColor = _colorSelectionService.ChooseColor(colorItem.ColorText);
        if (string.IsNullOrWhiteSpace(selectedColor))
        {
            return;
        }

        SetAppearanceColorByLabel(colorItem.Label, selectedColor);
        colorItem.ColorText = selectedColor;
    }

    private void SetAppearanceColorByLabel(string label, string colorText)
    {
        switch (label)
        {
            case "窗口背景色":
                WindowBackgroundColor = colorText;
                break;
            case "工作日文字色":
                CalendarWorkdayColor = colorText;
                break;
            case "休息日文字色":
                CalendarRestDayColor = colorText;
                break;
            case "今日文字色":
                CalendarTodayColor = colorText;
                break;
            case "非本月文字色":
                CalendarOutOfMonthColor = colorText;
                break;
            case "加班提示点色":
                CalendarOvertimeDotColor = colorText;
                break;
            case "日记提示点色":
                CalendarDiaryDotColor = colorText;
                break;
        }
    }

    private void OnAppearanceColorTextChanged(AppearanceColorItemViewModel colorItem, string colorText)
    {
        SetAppearanceColorByLabel(colorItem.Label, colorText);
    }

    private void UpdateAppearanceColorItem(string propertyName, string colorText)
    {
        var item = propertyName switch
        {
            nameof(WindowBackgroundColor) => AppearanceColorItems.FirstOrDefault(x => x.Label == "窗口背景色"),
            nameof(CalendarWorkdayColor) => AppearanceColorItems.FirstOrDefault(x => x.Label == "工作日文字色"),
            nameof(CalendarRestDayColor) => AppearanceColorItems.FirstOrDefault(x => x.Label == "休息日文字色"),
            nameof(CalendarTodayColor) => AppearanceColorItems.FirstOrDefault(x => x.Label == "今日文字色"),
            nameof(CalendarOutOfMonthColor) => AppearanceColorItems.FirstOrDefault(x => x.Label == "非本月文字色"),
            nameof(CalendarOvertimeDotColor) => AppearanceColorItems.FirstOrDefault(x => x.Label == "加班提示点色"),
            nameof(CalendarDiaryDotColor) => AppearanceColorItems.FirstOrDefault(x => x.Label == "日记提示点色"),
            _ => null
        };

        if (item is not null)
        {
            item.ColorText = colorText;
        }
    }
}
