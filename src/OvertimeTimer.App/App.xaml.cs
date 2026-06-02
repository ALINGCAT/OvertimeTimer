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
    }
}
