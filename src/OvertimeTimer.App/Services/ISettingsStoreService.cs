using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public interface ISettingsStoreService
{
    Task<WorkScheduleConfig> LoadWorkScheduleAsync(CancellationToken ct = default);
    Task SaveWorkScheduleAsync(WorkScheduleConfig config, CancellationToken ct = default);
    Task<AppearanceDataStore> LoadAppearanceAsync(CancellationToken ct = default);
    Task SaveAppearanceAsync(AppearanceDataStore data, CancellationToken ct = default);
    Task<StorageDataStore> LoadStorageAsync(CancellationToken ct = default);
    Task SaveStorageAsync(StorageDataStore data, CancellationToken ct = default);
}
