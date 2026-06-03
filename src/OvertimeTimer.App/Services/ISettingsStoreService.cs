using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public interface ISettingsStoreService
{
    Task<SettingsDataStore> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(SettingsDataStore settingsDataStore, CancellationToken cancellationToken = default);
}
