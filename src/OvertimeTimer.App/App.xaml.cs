using System.Windows;
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

        _ = InitializeAppearanceAsync();
    }

    private async Task InitializeAppearanceAsync()
    {
        try
        {
            var settingsStoreService = Container.Resolve<ISettingsStoreService>();
            var appearanceSettingsService = Container.Resolve<IAppearanceSettingsService>();
            var settingsDataStore = await settingsStoreService.LoadAsync();
            appearanceSettingsService.Apply(settingsDataStore.AppearanceConfig);
        }
        catch
        {
        }
    }
}
