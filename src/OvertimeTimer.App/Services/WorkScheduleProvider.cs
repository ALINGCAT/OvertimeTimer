using System.IO;
using System.Text.Json;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public sealed class WorkScheduleProvider : IWorkScheduleProvider
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _settingsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OvertimeTimer",
        "settings.json");

    public WorkScheduleConfig Config { get; private set; } = new();

    public event Action? ConfigChanged;

    public void Load()
    {
        if (!File.Exists(_settingsFilePath))
        {
            Config = new WorkScheduleConfig();
            return;
        }

        var json = File.ReadAllText(_settingsFilePath);
        var settingsDataStore = JsonSerializer.Deserialize<SettingsDataStore>(json, SerializerOptions);
        Config = settingsDataStore?.WorkScheduleConfig ?? new WorkScheduleConfig();
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_settingsFilePath))
        {
            Config = new WorkScheduleConfig();
            ConfigChanged?.Invoke();
            return;
        }

        await using var stream = new FileStream(
            _settingsFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            FileOptions.Asynchronous);

        var settingsDataStore = await JsonSerializer.DeserializeAsync<SettingsDataStore>(
            stream,
            SerializerOptions,
            cancellationToken);

        Config = settingsDataStore?.WorkScheduleConfig ?? new WorkScheduleConfig();
        ConfigChanged?.Invoke();
    }

    public bool IsRestDay(DateOnly date)
    {
        return !IsWorkDay(date);
    }

    public bool IsWorkDay(DateOnly date)
    {
        if (Config.Mode == WorkScheduleMode.Daily)
        {
            return IsWorkDayByDaily(date);
        }

        return IsWorkDayByWeekly(date);
    }

    private bool IsWorkDayByDaily(DateOnly date)
    {
        var cycleLength = Config.WorkDays + Config.RestDays;
        if (cycleLength <= 0)
        {
            return false;
        }

        var anchorDate = Config.AnchorDate;
        var dayDiff = date.DayNumber - anchorDate.DayNumber;
        var anchorPosition = Config.AnchorWorkDayIndex - 1;
        var position = ((anchorPosition + dayDiff) % cycleLength + cycleLength) % cycleLength;

        return position < Config.WorkDays;
    }

    private bool IsWorkDayByWeekly(DateOnly date)
    {
        if (Config.WeeklyCycles.Count == 0)
        {
            return false;
        }

        var anchorMonday = Config.AnchorDate.AddDays(-(((int)Config.AnchorDate.DayOfWeek + 6) % 7));
        var targetMonday = date.AddDays(-(((int)date.DayOfWeek + 6) % 7));
        var weekDiff = (targetMonday.DayNumber - anchorMonday.DayNumber) / 7;
        var targetCycleWeekIndex = ((Config.CurrentCycleWeekIndex - 1 + weekDiff) % Config.WeekCycleCount + Config.WeekCycleCount) % Config.WeekCycleCount;

        WeeklyCycleItem? targetWeek = null;
        foreach (var item in Config.WeeklyCycles)
        {
            if (item.WeekIndex - 1 == targetCycleWeekIndex)
            {
                targetWeek = item;
                break;
            }
        }

        targetWeek ??= Config.WeeklyCycles[targetCycleWeekIndex % Config.WeeklyCycles.Count];

        return date.DayOfWeek switch
        {
            DayOfWeek.Monday => targetWeek.MondayWork,
            DayOfWeek.Tuesday => targetWeek.TuesdayWork,
            DayOfWeek.Wednesday => targetWeek.WednesdayWork,
            DayOfWeek.Thursday => targetWeek.ThursdayWork,
            DayOfWeek.Friday => targetWeek.FridayWork,
            DayOfWeek.Saturday => targetWeek.SaturdayWork,
            DayOfWeek.Sunday => targetWeek.SundayWork,
            _ => false
        };
    }
}
