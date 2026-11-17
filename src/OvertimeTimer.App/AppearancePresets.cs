using System.ComponentModel;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App;

public static class AppearancePresets
{
    public sealed class Preset : INotifyPropertyChanged
    {
        public Preset(
            string id,
            string nameKey,
            bool isDefault,
            AppearanceConfig config,
            string previewBackgroundColor,
            string previewTextColor,
            string previewLinkColor,
            string previewCodeBackgroundColor)
        {
            Id = id;
            NameKey = nameKey;
            IsDefault = isDefault;
            Config = config;
            PreviewBackgroundColor = previewBackgroundColor;
            PreviewTextColor = previewTextColor;
            PreviewLinkColor = previewLinkColor;
            PreviewCodeBackgroundColor = previewCodeBackgroundColor;

            LocalizationService.Instance.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == "Item[]")
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
                }
            };
        }

        public string Id { get; }

        public string NameKey { get; }

        public bool IsDefault { get; }

        public AppearanceConfig Config { get; }

        public string PreviewBackgroundColor { get; }

        public string PreviewTextColor { get; }

        public string PreviewLinkColor { get; }

        public string PreviewCodeBackgroundColor { get; }

        public string DisplayName => LocalizationService.Instance[NameKey];

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    private sealed record LegacyPreset(AppearanceConfig Config, string TargetId);

    public static IReadOnlyList<Preset> All { get; } = new List<Preset>
    {
        new("ocean", "Appearance.Preset.OceanBlue", true,
            new()
            {
                WindowBackgroundColor = "#FFF3F8FC",
                CalendarWorkdayColor = "#FF1F3A56",
                CalendarRestDayColor = "#FF7D9AB3",
                CalendarTodayColor = "#FF22C55E",
                CalendarOutOfMonthColor = "#FFC9D8E4",
                CalendarOvertimeDotColor = "#FFDC2626",
                CalendarDiaryDotColor = "#FF16A34A",
                CalendarHolidayColor = "#FF8B5CF6",
                CalendarAdjustWorkdayColor = "#FFB91C1C",
                CardBackgroundColor = "#FFFFFFFF",
                CardBorderColor = "#FFDCE8F3",
                CalendarLeaveColor = "#FF0891B2",
                CalendarMarkerDotColor = "#FFE0A526",
                CalendarDayBorderColor = "#FFDCE8F3",
                AccentColor = "#FF3B82F6",
                AccentSoftColor = "#FFEAF3FB",
                TextPrimaryColor = "#FF16283A",
                TextSecondaryColor = "#FF5F7488",
                ShellBorderColor = "#FFDCE8F3",
                IsDarkTheme = false
            },
            "#FFFFFFFF", "#FF1F3A56", "#FF2563EB", "#FFEFF6FC"),

        new("caramel", "Appearance.Preset.WarmBrown", false,
            new()
            {
                WindowBackgroundColor = "#FFF9F4EC",
                CalendarWorkdayColor = "#FF43372B",
                CalendarRestDayColor = "#FFAB9680",
                CalendarTodayColor = "#FF22C55E",
                CalendarOutOfMonthColor = "#FFDDD0C0",
                CalendarOvertimeDotColor = "#FFDC2626",
                CalendarDiaryDotColor = "#FF16A34A",
                CalendarHolidayColor = "#FF9A5B8C",
                CalendarAdjustWorkdayColor = "#FFB91C1C",
                CardBackgroundColor = "#FFFFFCF7",
                CardBorderColor = "#FFEDE0CF",
                CalendarLeaveColor = "#FF0891B2",
                CalendarMarkerDotColor = "#FFE0A526",
                CalendarDayBorderColor = "#FFEDE0CF",
                AccentColor = "#FFB45309",
                AccentSoftColor = "#FFF7EBDD",
                TextPrimaryColor = "#FF3F3327",
                TextSecondaryColor = "#FF806F5E",
                ShellBorderColor = "#FFE7D9C7",
                IsDarkTheme = false
            },
            "#FFFFFCF7", "#FF43372B", "#FFB45309", "#FFF5ECE0"),

        new("mint", "Appearance.Preset.SageGreen", false,
            new()
            {
                WindowBackgroundColor = "#FFEFF5EC",
                CalendarWorkdayColor = "#FF2B3D2F",
                CalendarRestDayColor = "#FF89A182",
                CalendarTodayColor = "#FF22C55E",
                CalendarOutOfMonthColor = "#FFC9D8C1",
                CalendarOvertimeDotColor = "#FFDC2626",
                CalendarDiaryDotColor = "#FF16A34A",
                CalendarHolidayColor = "#FF7C6AAE",
                CalendarAdjustWorkdayColor = "#FFB91C1C",
                CardBackgroundColor = "#FFFBFDF8",
                CardBorderColor = "#FFD8E4CE",
                CalendarLeaveColor = "#FF0F766E",
                CalendarMarkerDotColor = "#FFC99A2E",
                CalendarDayBorderColor = "#FFD8E4CE",
                AccentColor = "#FF3F7D4E",
                AccentSoftColor = "#FFE6F0E1",
                TextPrimaryColor = "#FF25352A",
                TextSecondaryColor = "#FF637565",
                ShellBorderColor = "#FFD4E1CB",
                IsDarkTheme = false
            },
            "#FFFBFDF8", "#FF2B3D2F", "#FF3F7D4E", "#FFEAF2E5"),

        new("sakura", "Appearance.Preset.CherryBlossom", false,
            new()
            {
                WindowBackgroundColor = "#FFFDF4F6",
                CalendarWorkdayColor = "#FF4A2D37",
                CalendarRestDayColor = "#FFC39AA7",
                CalendarTodayColor = "#FF22C55E",
                CalendarOutOfMonthColor = "#FFE9D6DC",
                CalendarOvertimeDotColor = "#FFDC2626",
                CalendarDiaryDotColor = "#FF16A34A",
                CalendarHolidayColor = "#FF9A5B8C",
                CalendarAdjustWorkdayColor = "#FFBE123C",
                CardBackgroundColor = "#FFFFFFFF",
                CardBorderColor = "#FFF2DCE2",
                CalendarLeaveColor = "#FF0891B2",
                CalendarMarkerDotColor = "#FFE0A526",
                CalendarDayBorderColor = "#FFF2DCE2",
                AccentColor = "#FFBE185D",
                AccentSoftColor = "#FFFBE8EF",
                TextPrimaryColor = "#FF432A33",
                TextSecondaryColor = "#FF866A73",
                ShellBorderColor = "#FFF0D9DF",
                IsDarkTheme = false
            },
            "#FFFFFFFF", "#FF4A2D37", "#FFBE185D", "#FFFBEBF0"),

        new("lavender", "Appearance.Preset.Lavender", false,
            new()
            {
                WindowBackgroundColor = "#FFF7F5FA",
                CalendarWorkdayColor = "#FF342E45",
                CalendarRestDayColor = "#FFA79ABE",
                CalendarTodayColor = "#FF22C55E",
                CalendarOutOfMonthColor = "#FFDCD5E7",
                CalendarOvertimeDotColor = "#FFDC2626",
                CalendarDiaryDotColor = "#FF16A34A",
                CalendarHolidayColor = "#FF8B5CF6",
                CalendarAdjustWorkdayColor = "#FFB91C1C",
                CardBackgroundColor = "#FFFFFFFF",
                CardBorderColor = "#FFE4DEEE",
                CalendarLeaveColor = "#FF0891B2",
                CalendarMarkerDotColor = "#FFE0A526",
                CalendarDayBorderColor = "#FFE4DEEE",
                AccentColor = "#FF7C3AED",
                AccentSoftColor = "#FFF1EBFA",
                TextPrimaryColor = "#FF2E2940",
                TextSecondaryColor = "#FF706985",
                ShellBorderColor = "#FFE2DBEF",
                IsDarkTheme = false
            },
            "#FFFFFFFF", "#FF342E45", "#FF7C3AED", "#FFF2EDF9"),

        new("graphite", "Appearance.Preset.Graphite", false,
            new()
            {
                WindowBackgroundColor = "#FFF4F5F6",
                CalendarWorkdayColor = "#FF24282E",
                CalendarRestDayColor = "#FF9AA3AD",
                CalendarTodayColor = "#FF22C55E",
                CalendarOutOfMonthColor = "#FFCFD5DB",
                CalendarOvertimeDotColor = "#FFDC2626",
                CalendarDiaryDotColor = "#FF16A34A",
                CalendarHolidayColor = "#FF7A6B99",
                CalendarAdjustWorkdayColor = "#FFB91C1C",
                CardBackgroundColor = "#FFFFFFFF",
                CardBorderColor = "#FFE1E4E8",
                CalendarLeaveColor = "#FF0891B2",
                CalendarMarkerDotColor = "#FFC99A2E",
                CalendarDayBorderColor = "#FFE1E4E8",
                AccentColor = "#FF4B6B8A",
                AccentSoftColor = "#FFEDF1F5",
                TextPrimaryColor = "#FF20242A",
                TextSecondaryColor = "#FF6B727B",
                ShellBorderColor = "#FFDFE2E5",
                IsDarkTheme = false
            },
            "#FFFFFFFF", "#FF24282E", "#FF2563EB", "#FFF1F3F5"),

        new("midnight", "Appearance.Preset.Midnight", false,
            new()
            {
                WindowBackgroundColor = "#FF0F172A",
                CalendarWorkdayColor = "#FFE2E8F0",
                CalendarRestDayColor = "#FF8FA3BF",
                CalendarTodayColor = "#FF34D399",
                CalendarOutOfMonthColor = "#FF52647D",
                CalendarOvertimeDotColor = "#FFF87171",
                CalendarDiaryDotColor = "#FF34D399",
                CalendarHolidayColor = "#FFC4B5FD",
                CalendarAdjustWorkdayColor = "#FFFCA5A5",
                CardBackgroundColor = "#FF16233B",
                CardBorderColor = "#FF2A3A55",
                CalendarLeaveColor = "#FF67E8F9",
                CalendarMarkerDotColor = "#FFFBBF24",
                CalendarDayBorderColor = "#FF2A3A55",
                AccentColor = "#FF2563EB",
                AccentSoftColor = "#FF1E2A44",
                TextPrimaryColor = "#FFF1F5F9",
                TextSecondaryColor = "#FF94A3B8",
                ShellBorderColor = "#FF2A3A55",
                IsDarkTheme = true
            },
            "#FF16233B", "#FFE2E8F0", "#FF7DD3FC", "#FF0B1220"),

        new("mocha", "Appearance.Preset.Mocha", false,
            new()
            {
                WindowBackgroundColor = "#FF1B1714",
                CalendarWorkdayColor = "#FFF2E8DC",
                CalendarRestDayColor = "#FFB9A48C",
                CalendarTodayColor = "#FF4ADE80",
                CalendarOutOfMonthColor = "#FF6E5C49",
                CalendarOvertimeDotColor = "#FFF87171",
                CalendarDiaryDotColor = "#FF4ADE80",
                CalendarHolidayColor = "#FFD8B4FE",
                CalendarAdjustWorkdayColor = "#FFFCA5A5",
                CardBackgroundColor = "#FF26201B",
                CardBorderColor = "#FF3A3028",
                CalendarLeaveColor = "#FF7DD3FC",
                CalendarMarkerDotColor = "#FFFBBF24",
                CalendarDayBorderColor = "#FF3A3028",
                AccentColor = "#FFB45309",
                AccentSoftColor = "#FF3A2A1B",
                TextPrimaryColor = "#FFF6EEE4",
                TextSecondaryColor = "#FFBBA58D",
                ShellBorderColor = "#FF3A3028",
                IsDarkTheme = true
            },
            "#FF26201B", "#FFF2E8DC", "#FFFBBF24", "#FF15110E"),

        new("violet", "Appearance.Preset.Violet", false,
            new()
            {
                WindowBackgroundColor = "#FF15101F",
                CalendarWorkdayColor = "#FFEDE7F6",
                CalendarRestDayColor = "#FFA99BC4",
                CalendarTodayColor = "#FF34D399",
                CalendarOutOfMonthColor = "#FF5E5175",
                CalendarOvertimeDotColor = "#FFF87171",
                CalendarDiaryDotColor = "#FF34D399",
                CalendarHolidayColor = "#FFD8B4FE",
                CalendarAdjustWorkdayColor = "#FFFCA5A5",
                CardBackgroundColor = "#FF1F1830",
                CardBorderColor = "#FF332748",
                CalendarLeaveColor = "#FF67E8F9",
                CalendarMarkerDotColor = "#FFFBBF24",
                CalendarDayBorderColor = "#FF332748",
                AccentColor = "#FF7C3AED",
                AccentSoftColor = "#FF2A1D44",
                TextPrimaryColor = "#FFF3EEFB",
                TextSecondaryColor = "#FFA99BC4",
                ShellBorderColor = "#FF332748",
                IsDarkTheme = true
            },
            "#FF1F1830", "#FFEDE7F6", "#FFC4B5FD", "#FF120E1A")
    };

    private static IReadOnlyList<LegacyPreset> Legacy { get; } = new List<LegacyPreset>
    {
        new(new()
        {
            WindowBackgroundColor = "#FFF0F9FF",
            CalendarWorkdayColor = "#FF0C4A6E",
            CalendarRestDayColor = "#FF7DD3FC",
            CalendarTodayColor = "#FF22C55E",
            CalendarOutOfMonthColor = "#FFE0F2FE",
            CalendarOvertimeDotColor = "#FFDC2626",
            CalendarDiaryDotColor = "#FF16A34A",
            CalendarHolidayColor = "#FF8B5CF6",
            CalendarAdjustWorkdayColor = "#FFB91C1C",
            CardBackgroundColor = "#FFFFFFFF",
            CardBorderColor = "#FFBAE6FD",
            CalendarLeaveColor = "#FFF59E0B",
            CalendarMarkerDotColor = "#FFFFD700",
            CalendarDayBorderColor = "#FFBAE6FD"
        }, "ocean"),

        new(new()
        {
            WindowBackgroundColor = "#FFFFF7ED",
            CalendarWorkdayColor = "#FF431407",
            CalendarRestDayColor = "#FF9A3412",
            CalendarTodayColor = "#FF16A34A",
            CalendarOutOfMonthColor = "#FFFED7AA",
            CalendarOvertimeDotColor = "#FFDC2626",
            CalendarDiaryDotColor = "#FF16A34A",
            CalendarHolidayColor = "#FF7C3AED",
            CalendarAdjustWorkdayColor = "#FFB91C1C",
            CardBackgroundColor = "#FFFFFBEB",
            CardBorderColor = "#FFFDE68A",
            CalendarLeaveColor = "#FF0891B2",
            CalendarMarkerDotColor = "#FFFFD700",
            CalendarDayBorderColor = "#FFFDE68A"
        }, "caramel"),

        new(new()
        {
            WindowBackgroundColor = "#FFC7EDCC",
            CalendarWorkdayColor = "#FF1B2817",
            CalendarRestDayColor = "#FF5B8C5A",
            CalendarTodayColor = "#FF16A34A",
            CalendarOutOfMonthColor = "#FFA8D8A8",
            CalendarOvertimeDotColor = "#FFDC2626",
            CalendarDiaryDotColor = "#FF16A34A",
            CalendarHolidayColor = "#FF9333EA",
            CalendarAdjustWorkdayColor = "#FFB91C1C",
            CardBackgroundColor = "#FFD4F0D4",
            CardBorderColor = "#FF8BC48B",
            CalendarLeaveColor = "#FF0D9488",
            CalendarMarkerDotColor = "#FFFFD700",
            CalendarDayBorderColor = "#FF8BC48B"
        }, "mint"),

        new(new()
        {
            WindowBackgroundColor = "#FFFFF0F5",
            CalendarWorkdayColor = "#FF831843",
            CalendarRestDayColor = "#FFE5C5D5",
            CalendarTodayColor = "#FF22C55E",
            CalendarOutOfMonthColor = "#FFFCE7F3",
            CalendarOvertimeDotColor = "#FFDC2626",
            CalendarDiaryDotColor = "#FF16A34A",
            CalendarHolidayColor = "#FFA855F7",
            CalendarAdjustWorkdayColor = "#FFB91C1C",
            CardBackgroundColor = "#FFFFFFFF",
            CardBorderColor = "#FFFCE7F3",
            CalendarLeaveColor = "#FF4F46E5",
            CalendarMarkerDotColor = "#FFFFD700",
            CalendarDayBorderColor = "#FFFCE7F3"
        }, "sakura")
    };

    public static Preset? FindByVisibleColors(AppearanceConfig config)
        => All.FirstOrDefault(preset => MatchesVisible(preset.Config, config));

    public static AppearanceConfig UpgradeLegacy(AppearanceConfig config)
    {
        var modern = FindByVisibleColors(config);
        if (modern is not null)
        {
            return modern.Config;
        }

        var legacy = Legacy.FirstOrDefault(item => MatchesVisible(item.Config, config));
        if (legacy is null)
        {
            return config;
        }

        return All.FirstOrDefault(preset => preset.Id == legacy.TargetId)?.Config ?? config;
    }

    private static bool MatchesVisible(AppearanceConfig expected, AppearanceConfig actual)
        => expected.WindowBackgroundColor == actual.WindowBackgroundColor
           && expected.CalendarWorkdayColor == actual.CalendarWorkdayColor
           && expected.CalendarRestDayColor == actual.CalendarRestDayColor
           && expected.CalendarTodayColor == actual.CalendarTodayColor
           && expected.CalendarOutOfMonthColor == actual.CalendarOutOfMonthColor
           && expected.CalendarOvertimeDotColor == actual.CalendarOvertimeDotColor
           && expected.CalendarDiaryDotColor == actual.CalendarDiaryDotColor
           && expected.CalendarHolidayColor == actual.CalendarHolidayColor
           && expected.CalendarAdjustWorkdayColor == actual.CalendarAdjustWorkdayColor
           && expected.CardBackgroundColor == actual.CardBackgroundColor
           && expected.CardBorderColor == actual.CardBorderColor
           && expected.CalendarLeaveColor == actual.CalendarLeaveColor
           && expected.CalendarMarkerDotColor == actual.CalendarMarkerDotColor
           && expected.CalendarDayBorderColor == actual.CalendarDayBorderColor
           && expected.IsDarkTheme == actual.IsDarkTheme;
}
