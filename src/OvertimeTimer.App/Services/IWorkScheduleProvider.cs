using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public interface IWorkScheduleProvider
{
    WorkScheduleConfig Config { get; }
    bool IsRestDay(DateOnly date);
    bool IsWorkDay(DateOnly date);
    void Load();
    Task LoadAsync(CancellationToken cancellationToken = default);

    event Action? ConfigChanged;
}
