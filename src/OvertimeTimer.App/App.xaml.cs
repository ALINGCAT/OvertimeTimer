using System.Diagnostics;
using System.Windows;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Services;
using OvertimeTimer.App.ViewModels;
using Prism.Ioc;
using Prism.Unity;

namespace OvertimeTimer.App;

public partial class App : PrismApplication
{
    protected override Window CreateShell() => Container.Resolve<MainWindow>();

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IStatusMessageService, StatusMessageService>();
        containerRegistry.RegisterSingleton<IMonthSelectionDialogService, MonthSelectionDialogService>();
        containerRegistry.RegisterSingleton<ISettingsDialogService, SettingsDialogService>();
        containerRegistry.RegisterSingleton<ISettingsInteractionService, SettingsInteractionService>();
        containerRegistry.RegisterSingleton<ISettingsStoreService, SettingsStoreService>();
        containerRegistry.RegisterSingleton<ISettingsPersistenceCoordinator, SettingsPersistenceCoordinator>();
        containerRegistry.RegisterSingleton<IAppearanceSettingsService, AppearanceSettingsService>();
        containerRegistry.RegisterSingleton<IRecordStoreService, RecordStoreService>();
        containerRegistry.RegisterSingleton<IDiaryFileService, DiaryFileService>();
        containerRegistry.RegisterSingleton<IWorkScheduleProvider, WorkScheduleProvider>();
        containerRegistry.RegisterSingleton<IGeneralSettingsService, GeneralSettingsService>();

        var localizationService = new LocalizationService();
        localizationService.Load();
        LocalizationService.Instance = localizationService;
        containerRegistry.RegisterInstance<ILocalizationService>(localizationService);

        containerRegistry.Register<MainWindowViewModel>();
        containerRegistry.Register<MainViewModel>();
        containerRegistry.Register<SettingsViewModel>();
    }

    protected override void OnInitialized()
    {
        ApplyInitialAppearance();
        base.OnInitialized();
        if (MainWindow is not null)
            MainWindow.DataContext = Container.Resolve<MainWindowViewModel>();

        _ = InitializeAsync();
    }

    private void ApplyInitialAppearance()
    {
        try
        {
            var store = Container.Resolve<ISettingsStoreService>();
            var appearanceSettingsService = Container.Resolve<IAppearanceSettingsService>();
            var data = store.LoadAppearance();
            var config = AppearancePresets.UpgradeLegacy(data.AppearanceConfig);

            appearanceSettingsService.Apply(config);
            appearanceSettingsService.ApplyPreviewColors(
                data.PreviewBackgroundColor, data.PreviewTextColor, data.PreviewLinkColor,
                data.PreviewCodeBackgroundColor);
            appearanceSettingsService.ApplyPreviewTypography(
                data.PreviewFontFamily, data.PreviewFontSize, data.PreviewCodeFontFamily);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Appearance init failed: {ex}");
        }
    }

    private async Task InitializeAsync()
    {
        try
        {
            var store = Container.Resolve<ISettingsStoreService>();
            var diaryFileService = Container.Resolve<IDiaryFileService>();

            var stData = await store.LoadStorageAsync();
            diaryFileService.Configure(stData.DiaryConfig);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"App init failed: {ex}");
        }
    }
}
