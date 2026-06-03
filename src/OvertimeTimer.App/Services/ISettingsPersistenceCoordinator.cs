using OvertimeTimer.App.ViewModels;

namespace OvertimeTimer.App.Services;

public interface ISettingsPersistenceCoordinator
{
    Task LoadAsync(
        WorkScheduleSettingsViewModel workScheduleSection,
        StorageSettingsViewModel storageSection,
        AppearanceSettingsViewModel appearanceSection,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        WorkScheduleSettingsViewModel workScheduleSection,
        StorageSettingsViewModel storageSection,
        AppearanceSettingsViewModel appearanceSection,
        CancellationToken cancellationToken = default);
}
