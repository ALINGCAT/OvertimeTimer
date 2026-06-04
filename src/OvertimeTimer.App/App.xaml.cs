using System.Diagnostics;
using System.IO;
using System.Windows;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Services;
using OvertimeTimer.App.ViewModels;
using Prism.Ioc;
using Prism.Unity;

namespace OvertimeTimer.App;

public partial class App : PrismApplication
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

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
        {
            MainWindow.DataContext = Container.Resolve<MainWindowViewModel>();
        }

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            var settingsStoreService = Container.Resolve<ISettingsStoreService>();
            var appearanceSettingsService = Container.Resolve<IAppearanceSettingsService>();
            var diaryFileService = Container.Resolve<IDiaryFileService>();

            var settingsDataStore = await settingsStoreService.LoadAsync();

            var settingsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "OvertimeTimer", "settings.json");
            if (!File.Exists(settingsFilePath))
            {
                await settingsStoreService.SaveAsync(settingsDataStore);
            }

            appearanceSettingsService.Apply(settingsDataStore.AppearanceConfig);
            appearanceSettingsService.ApplyPreviewSettings(
                settingsDataStore.PreviewFontFamily, settingsDataStore.PreviewFontSize, settingsDataStore.PreviewLineHeight,
                settingsDataStore.PreviewBackgroundColor, settingsDataStore.PreviewTextColor, settingsDataStore.PreviewLinkColor,
                settingsDataStore.PreviewCodeBackgroundColor, settingsDataStore.PreviewCodeFontFamily);
            diaryFileService.Configure(settingsDataStore.DiaryStorageConfig);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"App initialization failed: {ex}");
        }
    }
}
