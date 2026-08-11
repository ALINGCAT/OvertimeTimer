using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public interface IGeneralSettingsService
{
    GeneralConfig Config { get; }

    void Load();

    Task LoadAsync(CancellationToken cancellationToken = default);

    void Apply(GeneralConfig config);

    event Action? ConfigChanged;
}
