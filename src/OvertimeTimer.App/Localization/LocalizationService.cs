using System.ComponentModel;
using System.IO;
using System.Text.Json;
using Prism.Mvvm;

namespace OvertimeTimer.App.Localization;

public sealed class LocalizationService : BindableBase, ILocalizationService
{
    public static LocalizationService Instance { get; internal set; } = new();

    private readonly Dictionary<string, string> _strings = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<LanguageItem> _availableLanguages = new()
    {
        new() { Code = "zh-CN", Name = "中文" },
        new() { Code = "en-US", Name = "English" }
    };

    private string _currentLanguage = "zh-CN";

    internal LocalizationService()
    {
        LoadDefaultStrings();
    }

    public string this[string key]
    {
        get
        {
            if (_strings.TryGetValue(key, out var value))
            {
                return value;
            }

            return key;
        }
    }

    public string CurrentLanguage
    {
        get => _currentLanguage;
        private set => SetProperty(ref _currentLanguage, value);
    }

    public IReadOnlyList<LanguageItem> AvailableLanguages => _availableLanguages.AsReadOnly();

    public async Task SetLanguageAsync(string languageCode)
    {
        if (_currentLanguage == languageCode)
        {
            return;
        }

        await LoadLanguageFileAsync(languageCode);
        CurrentLanguage = languageCode;
        RaisePropertyChanged("Item[]");
    }

    public void Load()
    {
        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            "Localization",
            "Resources",
            $"{_currentLanguage}.json");

        if (!File.Exists(filePath))
        {
            return;
        }

        var json = File.ReadAllText(filePath);
        var entries = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        if (entries is null)
        {
            return;
        }

        foreach (var (key, value) in entries)
        {
            _strings[key] = value;
        }
    }

    public async Task LoadAsync()
    {
        await LoadLanguageFileAsync(_currentLanguage);
    }

    private async Task LoadLanguageFileAsync(string languageCode)
    {
        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            "Localization",
            "Resources",
            $"{languageCode}.json");

        if (!File.Exists(filePath))
        {
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var entries = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        if (entries is null)
        {
            return;
        }

        foreach (var (key, value) in entries)
        {
            _strings[key] = value;
        }
    }

    private void LoadDefaultStrings()
    {
        _strings.Clear();
        _strings["Main.Title"] = "加班计时工具";
        _strings["Calendar.Monday"] = "周一";
        _strings["Calendar.Tuesday"] = "周二";
        _strings["Calendar.Wednesday"] = "周三";
        _strings["Calendar.Thursday"] = "周四";
        _strings["Calendar.Friday"] = "周五";
        _strings["Calendar.Saturday"] = "周六";
        _strings["Calendar.Sunday"] = "周日";
        _strings["Calendar.Today"] = "今天";
        _strings["Calendar.YearMonthFormat"] = "yyyy 年 MM 月";
        _strings["Calendar.OvertimeHours"] = "加班时长";
        _strings["Calendar.Hours"] = "小时";
        _strings["Calendar.Minutes"] = "分钟";
        _strings["Calendar.OvertimeFormat"] = "{0}小时{1}分钟";
        _strings["Calendar.DefaultOvertimeSummary"] = "0小时0分钟";
        _strings["Calendar.MonthlyPrefix"] = "本月已加";
        _strings["Calendar.WorkDaysRemaining"] = " 本月工作日剩余{0}天";
        _strings["Calendar.MonthWorkDays"] = "本月的工作日有{0}天";
        _strings["Calendar.PastMonthSummary"] = "本月已工作{0}天 共加班{1}小时{2}分钟";
        _strings["Calendar.Holiday"] = "节假日";
        _strings["Calendar.AdjustWorkday"] = "调休";
        _strings["Calendar.Leave"] = "请假";
        _strings["Calendar.TotalOvertime"] = "总计加班时长：{0}";
        _strings["Calendar.SelectedDateFormat"] = "当前选择的日期: {0}";
        _strings["Calendar.MarkdownDiary"] = "Markdown 日记";
        _strings["Calendar.Preview"] = "预览";
        _strings["Calendar.Save"] = "保存";
        _strings["Calendar.Setting"] = "设置";
        _strings["Calendar.WeekLabelFormat"] = "第 {0} 周";
        _strings["Diary.Saved"] = "{0}已保存";
        _strings["Diary.SaveFailed"] = "保存失败，请重试。";
        _strings["MonthPicker.Title"] = "选择年月";
        _strings["MonthPicker.Year"] = "年";
        _strings["MonthPicker.Month"] = "月";
        _strings["MonthPicker.Today"] = "今天";
        _strings["MonthPicker.Confirm"] = "确定";
        _strings["Settings.Title"] = "设置";
        _strings["Settings.General"] = "通用设置";
        _strings["Settings.WorkSchedule"] = "工作日规则";
        _strings["Settings.Appearance"] = "外观设置";
        _strings["Settings.Preview"] = "预览设置";
        _strings["Settings.GeneralDesc"] = "配置日记文件存储、分组方式和界面语言。";
        _strings["Settings.WorkScheduleDesc"] = "配置按周或按天的工作日循环规则。";
        _strings["Settings.AppearanceDesc"] = "配置窗口背景、月历日期和提示点的颜色。";
        _strings["Settings.PreviewDesc"] = "配置 Markdown 预览的字体、颜色等显示效果。";
        _strings["Settings.Save"] = "应用";
        _strings["Settings.ExportCss"] = "导出样式";
        _strings["Settings.ImportCss"] = "导入样式";
        _strings["Settings.OpenConfigDir"] = "打开配置文件目录";
        _strings["Settings.Weekly"] = "按周算";
        _strings["Settings.Daily"] = "按天算";
        _strings["Settings.CycleCount"] = "周期次数";
        _strings["Settings.CycleHint"] = "本周为第一周";
        _strings["Settings.WorkDays"] = "工作天数";
        _strings["Settings.RestDays"] = "休息天数";
        _strings["Settings.TodayIsCycleDay"] = "今天是循环的第";
        _strings["Settings.DayUnit"] = "天";
        _strings["Settings.StorageRootPath"] = "日记根目录";
        _strings["Settings.StorageChooseDir"] = "选择目录";
        _strings["Settings.StorageFlat"] = "不分文件夹";
        _strings["Settings.StorageByYear"] = "按年分类";
        _strings["Settings.StorageByMonth"] = "按月分类";
        _strings["Settings.StorageFlatDesc"] = "所有日记文件直接放在根目录中。";
        _strings["Settings.StorageByYearDesc"] = "按年份创建子目录，例如 2026\\2026-06-03.md。";
        _strings["Settings.StorageByMonthDesc"] = "按年月创建子目录，例如 2026-06\\2026-06-03.md。";
        _strings["Settings.StoragePathHint"] = "请先选择或输入日记根目录。";
        _strings["Settings.StorageCurrentPath"] = "当前目录：{0}";
        _strings["Settings.StorageRootPathEmpty"] = "日记根目录不能为空。";
        _strings["Settings.StorageSaveCancelled"] = "已取消保存。";
        _strings["Settings.StorageCreateDirFailed"] = "创建目录失败，请检查路径是否有效或是否有权限。";
        _strings["Settings.StorageCreateConfirm"] = "路径不存在：\n{0}\n\n是否要在此路径新建文件夹？";
        _strings["Settings.StoragePathNotExist"] = "路径不存在";
        _strings["Settings.AppearanceColorFormatError"] = "{0}格式无效，请输入如 #FFEAF3FF 的颜色值。";
        _strings["Settings.Saved"] = "已应用并保存配置";
        _strings["Settings.LoadFailed"] = "设置加载失败，已使用默认配置。";
        _strings["Settings.SaveFailed"] = "保存配置失败。";
        _strings["WorkSchedule.TodayDescription"] = "今天是{0}月{1}日";
        _strings["Language.Language"] = "语言";
        _strings["Language.SwitchFailed"] = "语言切换失败。";
        _strings["Appearance.WindowBackground"] = "窗口背景色";
        _strings["Appearance.CalendarWorkday"] = "工作日文字色";
        _strings["Appearance.CalendarRestDay"] = "休息日文字色";
        _strings["Appearance.CalendarToday"] = "今日文字色";
        _strings["Appearance.CalendarOutOfMonth"] = "非本月文字色";
        _strings["Appearance.CalendarOvertimeDot"] = "加班提示点色";
        _strings["Appearance.CalendarDiaryDot"] = "日记提示点色";
        _strings["Appearance.PreviewFont"] = "预览字体";
        _strings["Appearance.FontSize"] = "字号";
        _strings["Appearance.LineHeight"] = "行高";
        _strings["Appearance.PreviewBackground"] = "预览背景色";
        _strings["Appearance.CalendarHoliday"] = "节假日文字色";
        _strings["Appearance.CalendarAdjustWorkday"] = "调休文字色";
        _strings["Appearance.CardBackground"] = "卡片背景色";
        _strings["Appearance.CardBorder"] = "卡片边框色";
        _strings["Appearance.CalendarLeave"] = "请假文字色";
        _strings["Appearance.CalendarDayBorder"] = "月历边框色";
        _strings["Appearance.Presets"] = "外观预设";
        _strings["Appearance.Preset.Custom"] = "自定义";
        _strings["Appearance.Preset.WarmBrown"] = "暖棕";
        _strings["Appearance.PreviewBackground"] = "背景色";
        _strings["Appearance.PreviewText"] = "文字色";
        _strings["Appearance.PreviewLink"] = "链接色";
        _strings["Appearance.PreviewCodeBackground"] = "代码块背景色";
        _strings["Appearance.PreviewCodeFont"] = "代码块字体";
        _strings["Appearance.PreviewSection"] = "预览设置";
        _strings["Appearance.LanguageSection"] = "语言";
        _strings["Appearance.Preset.SkyBlue"] = "淡蓝（默认）";
        _strings["Appearance.Preset.Dark"] = "深色";
        _strings["Appearance.Preset.WarmBrown"] = "暖棕";
        _strings["Appearance.Preset.SageGreen"] = "豆沙绿";
        _strings["Appearance.Preset.LightGray"] = "浅灰";
        _strings["Appearance.Preset.CherryBlossom"] = "樱花粉";
        _strings["Appearance.Preset.OceanBlue"] = "海洋蓝";
        _strings["Appearance.Preset.DarkPurple"] = "暗紫";
    }
}
