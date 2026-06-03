using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public interface IWorkScheduleProvider
{
    WorkScheduleConfig Config { get; }
    IReadOnlyList<DayOverride> Overrides { get; }
    bool IsRestDay(DateOnly date);
    bool IsWorkDay(DateOnly date);
    DayOverride? GetOverride(DateOnly date);
    void AddOverride(DateOnly date, bool isHoliday);
    void RemoveOverride(DateOnly date);
    void Load();
    Task LoadAsync(CancellationToken cancellationToken = default);

    event Action? ConfigChanged;
}
