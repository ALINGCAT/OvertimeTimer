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
        containerRegistry.RegisterSingleton<IColorSelectionService, ColorSelectionService>();
        containerRegistry.RegisterSingleton<ISettingsStoreService, SettingsStoreService>();
        containerRegistry.RegisterSingleton<ISettingsPersistenceCoordinator, SettingsPersistenceCoordinator>();
        containerRegistry.RegisterSingleton<IAppearanceSettingsService, AppearanceSettingsService>();
        containerRegistry.RegisterSingleton<IRecordStoreService, RecordStoreService>();
        containerRegistry.RegisterSingleton<IDiaryFileService, DiaryFileService>();
        containerRegistry.RegisterSingleton<IWorkScheduleProvider, WorkScheduleProvider>();

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
        base.OnInitialized();
        if (MainWindow is not null)
            MainWindow.DataContext = Container.Resolve<MainWindowViewModel>();

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            var store = Container.Resolve<ISettingsStoreService>();
            var appearanceSettingsService = Container.Resolve<IAppearanceSettingsService>();
            var diaryFileService = Container.Resolve<IDiaryFileService>();

            var apData = await store.LoadAppearanceAsync();
            appearanceSettingsService.Apply(apData.AppearanceConfig);
            appearanceSettingsService.ApplyPreviewSettings(
                apData.PreviewFontFamily, apData.PreviewFontSize, apData.PreviewLineHeight,
                apData.PreviewBackgroundColor, apData.PreviewTextColor, apData.PreviewLinkColor,
                apData.PreviewCodeBackgroundColor, apData.PreviewCodeFontFamily);

            var stData = await store.LoadStorageAsync();
            diaryFileService.Configure(stData.DiaryConfig);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"App init failed: {ex}");
        }
    }
}
