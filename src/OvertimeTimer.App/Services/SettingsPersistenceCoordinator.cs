using OvertimeTimer.App.ViewModels;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public sealed class SettingsPersistenceCoordinator : ISettingsPersistenceCoordinator
{
    private readonly ISettingsStoreService _settingsStoreService;
    private readonly IDiaryFileService _diaryFileService;
    private readonly IWorkScheduleProvider _workScheduleProvider;
    private readonly IAppearanceSettingsService _appearanceSettingsService;

    public SettingsPersistenceCoordinator(
        ISettingsStoreService settingsStoreService,
        IDiaryFileService diaryFileService,
        IWorkScheduleProvider workScheduleProvider,
        IAppearanceSettingsService appearanceSettingsService)
    {
        _settingsStoreService = settingsStoreService;
        _diaryFileService = diaryFileService;
        _workScheduleProvider = workScheduleProvider;
        _appearanceSettingsService = appearanceSettingsService;
    }

    public async Task LoadAsync(
        WorkScheduleSettingsViewModel workScheduleSection,
        StorageSettingsViewModel storageSection,
        AppearanceSettingsViewModel appearanceSection,
        GeneralSettingsViewModel generalSection,
        PreviewSettingsViewModel previewSection,
        CancellationToken cancellationToken = default)
    {
        var settingsDataStore = await _settingsStoreService.LoadAsync(cancellationToken);
        workScheduleSection.LoadFrom(settingsDataStore.WorkScheduleConfig);
        storageSection.LoadFrom(settingsDataStore.DiaryStorageConfig);
        appearanceSection.LoadFrom(settingsDataStore.AppearanceConfig);
        previewSection.LoadFrom(settingsDataStore.PreviewFontFamily, settingsDataStore.PreviewFontSize, settingsDataStore.PreviewLineHeight,
            settingsDataStore.PreviewBackgroundColor, settingsDataStore.PreviewTextColor,
            settingsDataStore.PreviewLinkColor, settingsDataStore.PreviewCodeBackgroundColor,
            settingsDataStore.PreviewCodeFontFamily);
    }

    public async Task SaveAsync(
        WorkScheduleSettingsViewModel workScheduleSection,
        StorageSettingsViewModel storageSection,
        AppearanceSettingsViewModel appearanceSection,
        GeneralSettingsViewModel generalSection,
        PreviewSettingsViewModel previewSection,
        CancellationToken cancellationToken = default)
    {
        var settingsDataStore = new SettingsDataStore
        {
            WorkScheduleConfig = workScheduleSection.ToModel(),
            DiaryStorageConfig = storageSection.ToModel(),
            AppearanceConfig = appearanceSection.ToModel(),
            PreviewFontFamily = previewSection.PreviewFontFamily,
            PreviewFontSize = previewSection.PreviewFontSize,
            PreviewLineHeight = previewSection.PreviewLineHeight,
            PreviewBackgroundColor = previewSection.PreviewBackgroundColor,
            PreviewTextColor = previewSection.PreviewTextColor,
            PreviewLinkColor = previewSection.PreviewLinkColor,
            PreviewCodeBackgroundColor = previewSection.PreviewCodeBackgroundColor,
            PreviewCodeFontFamily = previewSection.PreviewCodeFontFamily,
            Overrides = _workScheduleProvider.Overrides.ToList()
        };

        await _settingsStoreService.SaveAsync(settingsDataStore, cancellationToken);
        _diaryFileService.Configure(settingsDataStore.DiaryStorageConfig);
        _appearanceSettingsService.ApplyPreviewSettings(
            settingsDataStore.PreviewFontFamily, settingsDataStore.PreviewFontSize, settingsDataStore.PreviewLineHeight,
            settingsDataStore.PreviewBackgroundColor, settingsDataStore.PreviewTextColor, settingsDataStore.PreviewLinkColor,
            settingsDataStore.PreviewCodeBackgroundColor, settingsDataStore.PreviewCodeFontFamily);
        await _workScheduleProvider.LoadAsync(cancellationToken);
    }
}
