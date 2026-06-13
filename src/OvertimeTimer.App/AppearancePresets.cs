using OvertimeTimer.App.Models;

namespace OvertimeTimer.App;

public static class AppearancePresets
{
    public sealed record Preset(string NameKey, AppearanceConfig Config, string PreviewFontFamily, double PreviewFontSize, double PreviewLineHeight,
        string PreviewBackgroundColor, string PreviewTextColor, string PreviewLinkColor, string PreviewCodeBackgroundColor, string PreviewCodeFontFamily)
    {
        public string DisplayName => Localization.LocalizationService.Instance[NameKey];

        public bool IsMatch(AppearanceConfig config)
        {
            if (Config is null) return config is null;
            return Config.WindowBackgroundColor == config.WindowBackgroundColor
                && Config.CalendarWorkdayColor == config.CalendarWorkdayColor
                && Config.CalendarRestDayColor == config.CalendarRestDayColor
                && Config.CalendarTodayColor == config.CalendarTodayColor
                && Config.CalendarOutOfMonthColor == config.CalendarOutOfMonthColor
                && Config.CalendarOvertimeDotColor == config.CalendarOvertimeDotColor
                && Config.CalendarDiaryDotColor == config.CalendarDiaryDotColor
                && Config.CalendarHolidayColor == config.CalendarHolidayColor
                && Config.CalendarAdjustWorkdayColor == config.CalendarAdjustWorkdayColor
                && Config.CalendarLeaveColor == config.CalendarLeaveColor
                && Config.CardBackgroundColor == config.CardBackgroundColor
                && Config.CardBorderColor == config.CardBorderColor;
        }
    }

    public static Preset Custom { get; } = new("Appearance.Preset.Custom", null!,
        "", 0, 0, "", "", "", "", "");

    public static IReadOnlyList<Preset> All { get; } = new List<Preset>
    {
        new("Appearance.Preset.WarmBrown",
            new() { WindowBackgroundColor = "#FFFFF7ED", CalendarWorkdayColor = "#FF431407", CalendarRestDayColor = "#FF9A3412", CalendarTodayColor = "#FF16A34A", CalendarOutOfMonthColor = "#FFFED7AA", CalendarOvertimeDotColor = "#FFDC2626", CalendarDiaryDotColor = "#FF16A34A", CalendarHolidayColor = "#FF7C3AED", CalendarAdjustWorkdayColor = "#FFB91C1C", CalendarLeaveColor = "#FF0891B2", CardBackgroundColor = "#FFFFFBEB", CardBorderColor = "#FFFDE68A", CalendarDayBorderColor = "#FFFDE68A" },
            "Microsoft YaHei UI", 14, 12, "#FFFFFBEB", "#FF431407", "#FFEA580C", "#FFFED7AA", "Consolas"),
        new("Appearance.Preset.SageGreen",
            new() { WindowBackgroundColor = "#FFC7EDCC", CalendarWorkdayColor = "#FF1B2817", CalendarRestDayColor = "#FF5B8C5A", CalendarTodayColor = "#FF16A34A", CalendarOutOfMonthColor = "#FFA8D8A8", CalendarOvertimeDotColor = "#FFDC2626", CalendarDiaryDotColor = "#FF16A34A", CalendarHolidayColor = "#FF9333EA", CalendarAdjustWorkdayColor = "#FFB91C1C", CalendarLeaveColor = "#FF0D9488", CardBackgroundColor = "#FFD4F0D4", CardBorderColor = "#FF8BC48B", CalendarDayBorderColor = "#FF8BC48B" },
            "Microsoft YaHei UI", 14, 12, "#FFD4F0D4", "#FF1B2817", "#FF059669", "#FFA8D8A8", "Consolas"),
        new("Appearance.Preset.OceanBlue",
            new() { WindowBackgroundColor = "#FFF0F9FF", CalendarWorkdayColor = "#FF0C4A6E", CalendarRestDayColor = "#FF7DD3FC", CalendarTodayColor = "#FF22C55E", CalendarOutOfMonthColor = "#FFE0F2FE", CalendarOvertimeDotColor = "#FFDC2626", CalendarDiaryDotColor = "#FF16A34A", CalendarHolidayColor = "#FF8B5CF6", CalendarAdjustWorkdayColor = "#FFB91C1C", CalendarLeaveColor = "#FFF59E0B", CardBackgroundColor = "#FFFFFFFF", CardBorderColor = "#FFBAE6FD", CalendarDayBorderColor = "#FFBAE6FD" },
            "Microsoft YaHei UI", 14, 12, "#FFFFFFFF", "#FF0C4A6E", "#FF0284C7", "#FFE0F2FE", "Consolas"),
            new("Appearance.Preset.CherryBlossom",
            new() { WindowBackgroundColor = "#FFFFF0F5", CalendarWorkdayColor = "#FF831843", CalendarRestDayColor = "#FFE5C5D5", CalendarTodayColor = "#FF22C55E", CalendarOutOfMonthColor = "#FFFCE7F3", CalendarOvertimeDotColor = "#FFDC2626", CalendarDiaryDotColor = "#FF16A34A", CalendarHolidayColor = "#FFA855F7", CalendarAdjustWorkdayColor = "#FFB91C1C", CalendarLeaveColor = "#FF4F46E5", CardBackgroundColor = "#FFFFFFFF", CardBorderColor = "#FFFCE7F3", CalendarDayBorderColor = "#FFFCE7F3" },
            "Microsoft YaHei UI", 14, 12, "#FFFFFFFF", "#FF831843", "#FFDB2777", "#FFFCE7F3", "Consolas"),
    };
}
