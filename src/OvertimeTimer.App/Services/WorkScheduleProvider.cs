using System.IO;
using System.Text.Json;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public sealed class WorkScheduleProvider : IWorkScheduleProvider
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly ISettingsStoreService _store;
    private readonly string _overridesFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OvertimeTimer", "overrides.json");
    private readonly Dictionary<DateOnly, DayOverride> _overrides = new();

    public WorkScheduleConfig Config { get; private set; } = new();
    public IReadOnlyList<DayOverride> Overrides => _overrides.Values.ToList().AsReadOnly();
    public event Action? ConfigChanged;

    public WorkScheduleProvider(ISettingsStoreService store) { _store = store; }

    public void Load()
    {
        Config = Task.Run(() => _store.LoadWorkScheduleAsync()).Result;
        LoadOverridesFromFile();
    }

    public async Task LoadAsync(CancellationToken ct = default)
    {
        Config = await _store.LoadWorkScheduleAsync(ct);
        LoadOverridesFromFile();
        ConfigChanged?.Invoke();
    }

    public DayOverride? GetOverride(DateOnly date) => _overrides.TryGetValue(date, out var o) ? o : null;

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
        if (o is not null) return o.Type is OverrideType.Holiday or OverrideType.Leave;
        return !IsWorkDay(date);
    }

    public bool IsWorkDay(DateOnly date)
    {
        var o = GetOverride(date);
        if (o is not null) return o.Type == OverrideType.AdjustWorkday;
        return Config.Mode == WorkScheduleMode.Daily ? IsWorkDayByDaily(date) : IsWorkDayByWeekly(date);
    }

    private void LoadOverridesFromFile()
    {
        _overrides.Clear();
        if (!File.Exists(_overridesFilePath)) return;
        try
        {
            var json = File.ReadAllText(_overridesFilePath);
            var list = JsonSerializer.Deserialize<List<DayOverride>>(json, SerializerOptions);
            if (list is null) return;
            foreach (var o in list) _overrides[o.Date] = o;
        }
        catch { }
    }

    private async Task SaveOverridesAsync()
    {
        try
        {
            var list = _overrides.Values.ToList();
            Directory.CreateDirectory(Path.GetDirectoryName(_overridesFilePath)!);
            var tmp = _overridesFilePath + ".tmp";
            await File.WriteAllTextAsync(tmp, JsonSerializer.Serialize(list, SerializerOptions));
            if (File.Exists(_overridesFilePath)) File.Replace(tmp, _overridesFilePath, null);
            else File.Move(tmp, _overridesFilePath);
        }
        catch { }
    }

    private bool IsWorkDayByDaily(DateOnly date)
    {
        var cl = Config.WorkDays + Config.RestDays;
        if (cl <= 0) return false;
        var diff = date.DayNumber - Config.AnchorDate.DayNumber;
        var pos = ((Config.AnchorWorkDayIndex - 1 + diff) % cl + cl) % cl;
        return pos < Config.WorkDays;
    }

    private bool IsWorkDayByWeekly(DateOnly date)
    {
        if (Config.WeeklyCycles.Count == 0) return false;
        var am = Config.AnchorDate.AddDays(-(((int)Config.AnchorDate.DayOfWeek + 6) % 7));
        var tm = date.AddDays(-(((int)date.DayOfWeek + 6) % 7));
        var wd = (tm.DayNumber - am.DayNumber) / 7;
        var idx = (wd % Config.WeekCycleCount + Config.WeekCycleCount) % Config.WeekCycleCount;
        var tw = Config.WeeklyCycles.Find(c => c.WeekIndex - 1 == idx) ?? Config.WeeklyCycles[idx % Config.WeeklyCycles.Count];
        return date.DayOfWeek switch
        {
            DayOfWeek.Monday => tw.MondayWork, DayOfWeek.Tuesday => tw.TuesdayWork,
            DayOfWeek.Wednesday => tw.WednesdayWork, DayOfWeek.Thursday => tw.ThursdayWork,
            DayOfWeek.Friday => tw.FridayWork, DayOfWeek.Saturday => tw.SaturdayWork,
            DayOfWeek.Sunday => tw.SundayWork, _ => false
        };
    }
}
