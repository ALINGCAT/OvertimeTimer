using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public interface IWorkScheduleProvider
{
    WorkScheduleConfig Config { get; }
    IReadOnlyList<DayOverride> Overrides { get; }
    IReadOnlySet<DateOnly> MarkedDates { get; }
    bool IsRestDay(DateOnly date);
    bool IsWorkDay(DateOnly date);
    DayOverride? GetOverride(DateOnly date);
    void AddOverride(DateOnly date, OverrideType type);
    void RemoveOverride(DateOnly date);
    bool IsMarked(DateOnly date);
    void ToggleMark(DateOnly date);
    void Load();
    Task LoadAsync(CancellationToken cancellationToken = default);

    event Action? ConfigChanged;
}
