using OvertimeTimer.App.ViewModels;
using OvertimeTimer.Core.Models;

namespace OvertimeTimer.App.Services;

public sealed class SettingsPersistenceCoordinator : ISettingsPersistenceCoordinator
{
    private readonly ISettingsStoreService _settingsStoreService;

    public SettingsPersistenceCoordinator(ISettingsStoreService settingsStoreService)
    {
        _settingsStoreService = settingsStoreService;
    }

    public async Task LoadAsync(
        WorkScheduleSettingsViewModel workScheduleSection,
        StorageSettingsViewModel storageSection,
        AppearanceSettingsViewModel appearanceSection,
        CancellationToken cancellationToken = default)
    {
        var settingsDataStore = await _settingsStoreService.LoadAsync(cancellationToken);
        workScheduleSection.LoadFrom(settingsDataStore.WorkScheduleConfig);
        storageSection.LoadFrom(settingsDataStore.DiaryStorageConfig);
        appearanceSection.LoadFrom(settingsDataStore.AppearanceConfig);
    }

    public async Task SaveAsync(
        WorkScheduleSettingsViewModel workScheduleSection,
        StorageSettingsViewModel storageSection,
        AppearanceSettingsViewModel appearanceSection,
        CancellationToken cancellationToken = default)
    {
        var settingsDataStore = new SettingsDataStore
        {
            WorkScheduleConfig = workScheduleSection.ToModel(),
            DiaryStorageConfig = storageSection.ToModel(),
            AppearanceConfig = appearanceSection.ToModel()
        };

        await _settingsStoreService.SaveAsync(settingsDataStore, cancellationToken);
    }
}
