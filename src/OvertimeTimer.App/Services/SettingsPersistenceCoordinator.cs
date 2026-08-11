using OvertimeTimer.App.ViewModels;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public sealed class SettingsPersistenceCoordinator : ISettingsPersistenceCoordinator
{
    private readonly ISettingsStoreService _store;
    private readonly IDiaryFileService _diaryFileService;
    private readonly IWorkScheduleProvider _workScheduleProvider;
    private readonly IAppearanceSettingsService _appearanceSettingsService;
    private readonly IGeneralSettingsService _generalSettingsService;

    public SettingsPersistenceCoordinator(
        ISettingsStoreService store, IDiaryFileService diaryFileService,
        IWorkScheduleProvider workScheduleProvider, IAppearanceSettingsService appearanceSettingsService,
        IGeneralSettingsService generalSettingsService)
    {
        _store = store;
        _diaryFileService = diaryFileService;
        _workScheduleProvider = workScheduleProvider;
        _appearanceSettingsService = appearanceSettingsService;
        _generalSettingsService = generalSettingsService;
    }

    public async Task LoadAsync(WorkScheduleSettingsViewModel ws, StorageSettingsViewModel ss, AppearanceSettingsViewModel ap,
        GeneralSettingsViewModel gs, PreviewSettingsViewModel ps, CancellationToken ct = default)
    {
        var wc = await _store.LoadWorkScheduleAsync(ct);
        ws.LoadFrom(wc);

        var ad = await _store.LoadAppearanceAsync(ct);
        ap.LoadFrom(ad.AppearanceConfig);
        ps.LoadFrom(ad.PreviewFontFamily, ad.PreviewFontSize, ad.PreviewLineHeight,
            ad.PreviewBackgroundColor, ad.PreviewTextColor, ad.PreviewLinkColor,
            ad.PreviewCodeBackgroundColor, ad.PreviewCodeFontFamily);

        var sd = await _store.LoadStorageAsync(ct);
        ss.LoadFrom(sd.DiaryConfig);

        gs.LoadFrom(await _store.LoadGeneralAsync(ct));
    }

    public async Task SaveAsync(WorkScheduleSettingsViewModel ws, StorageSettingsViewModel ss, AppearanceSettingsViewModel ap,
        GeneralSettingsViewModel gs, PreviewSettingsViewModel ps, CancellationToken ct = default)
    {
        await _store.SaveWorkScheduleAsync(ws.ToModel(), ct);

        await _store.SaveAppearanceAsync(new AppearanceDataStore
        {
            AppearanceConfig = ap.ToModel(),
            PreviewFontFamily = ps.PreviewFontFamily, PreviewFontSize = ps.PreviewFontSize,
            PreviewLineHeight = ps.PreviewLineHeight, PreviewBackgroundColor = ps.PreviewBackgroundColor,
            PreviewTextColor = ps.PreviewTextColor, PreviewLinkColor = ps.PreviewLinkColor,
            PreviewCodeBackgroundColor = ps.PreviewCodeBackgroundColor, PreviewCodeFontFamily = ps.PreviewCodeFontFamily
        }, ct);

        await _store.SaveStorageAsync(new StorageDataStore { DiaryConfig = ss.ToModel() }, ct);

        var generalConfig = gs.ToModel();
        await _store.SaveGeneralAsync(generalConfig, ct);
        _generalSettingsService.Apply(generalConfig);

        _diaryFileService.Configure(ss.ToModel());
        _appearanceSettingsService.ApplyPreviewSettings(ps.PreviewFontFamily, ps.PreviewFontSize, ps.PreviewLineHeight,
            ps.PreviewBackgroundColor, ps.PreviewTextColor, ps.PreviewLinkColor,
            ps.PreviewCodeBackgroundColor, ps.PreviewCodeFontFamily);
        await _workScheduleProvider.LoadAsync(ct);
    }
}
