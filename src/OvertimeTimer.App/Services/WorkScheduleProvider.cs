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

    private readonly string _overridesFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OvertimeTimer",
        "overrides.json");

    private readonly Dictionary<DateOnly, DayOverride> _overrides = new();

    public WorkScheduleConfig Config { get; private set; } = new();

    public IReadOnlyList<DayOverride> Overrides => _overrides.Values.ToList().AsReadOnly();

    public event Action? ConfigChanged;

    public void Load()
    {
        if (!File.Exists(_settingsFilePath))
        {
            Config = new WorkScheduleConfig();
        }
        else
        {
            var json = File.ReadAllText(_settingsFilePath);
            var settingsDataStore = JsonSerializer.Deserialize<SettingsDataStore>(json, SerializerOptions);
            Config = settingsDataStore?.WorkScheduleConfig ?? new WorkScheduleConfig();
        }

        LoadOverridesFromFile();
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
        LoadOverridesFromFile();
        ConfigChanged?.Invoke();
    }

    public DayOverride? GetOverride(DateOnly date)
    {
        return _overrides.TryGetValue(date, out var o) ? o : null;
    }

    public void AddOverride(DateOnly date, OverrideType type)
    {
        _overrides[date] = new DayOverride { Date = date, Type = type };
        _ = SaveOverridesAsync();
    }

    public void RemoveOverride(DateOnly date)
    {
        _overrides.Remove(date);
        _ = SaveOverridesAsync();
    }

    public bool IsRestDay(DateOnly date)
    {
        var o = GetOverride(date);
        if (o is not null)
            return o.Type is OverrideType.Holiday or OverrideType.Leave;

        return !IsWorkDay(date);
    }

    public bool IsWorkDay(DateOnly date)
    {
        var o = GetOverride(date);
        if (o is not null)
            return o.Type == OverrideType.AdjustWorkday;

        if (Config.Mode == WorkScheduleMode.Daily)
            return IsWorkDayByDaily(date);

        return IsWorkDayByWeekly(date);
    }

    private void LoadOverridesFromFile()
    {
        _overrides.Clear();
        if (!File.Exists(_overridesFilePath))
            return;

        try
        {
            var json = File.ReadAllText(_overridesFilePath);
            var list = JsonSerializer.Deserialize<List<DayOverride>>(json, SerializerOptions);
            if (list is null)
                return;

            foreach (var o in list)
            {
                _overrides[o.Date] = o;
            }
        }
        catch
        {
        }
    }

    private bool IsWorkDayByDaily(DateOnly date)
    {
        var cycleLength = Config.WorkDays + Config.RestDays;
        if (cycleLength <= 0)
            return false;

        var anchorDate = Config.AnchorDate;
        var dayDiff = date.DayNumber - anchorDate.DayNumber;
        var anchorPosition = Config.AnchorWorkDayIndex - 1;
        var position = ((anchorPosition + dayDiff) % cycleLength + cycleLength) % cycleLength;

        return position < Config.WorkDays;
    }

    private bool IsWorkDayByWeekly(DateOnly date)
    {
        if (Config.WeeklyCycles.Count == 0)
            return false;

        var anchorMonday = Config.AnchorDate.AddDays(-(((int)Config.AnchorDate.DayOfWeek + 6) % 7));
        var targetMonday = date.AddDays(-(((int)date.DayOfWeek + 6) % 7));
        var weekDiff = (targetMonday.DayNumber - anchorMonday.DayNumber) / 7;
        var targetCycleWeekIndex = (weekDiff % Config.WeekCycleCount + Config.WeekCycleCount) % Config.WeekCycleCount;

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

    private async Task SaveOverridesAsync()
    {
        try
        {
            var list = _overrides.Values.ToList();
            Directory.CreateDirectory(Path.GetDirectoryName(_overridesFilePath)!);
            var temporaryFilePath = $"{_overridesFilePath}.tmp";

            await using (var stream = new FileStream(
                             temporaryFilePath,
                             FileMode.Create,
                             FileAccess.Write,
                             FileShare.None,
                             4096,
                             FileOptions.Asynchronous))
            {
                await JsonSerializer.SerializeAsync(stream, list, SerializerOptions);
                await stream.FlushAsync();
            }

            if (File.Exists(_overridesFilePath))
            {
                File.Replace(temporaryFilePath, _overridesFilePath, null);
            }
            else
            {
                File.Move(temporaryFilePath, _overridesFilePath);
            }
        }
        catch
        {
        }
    }
}
