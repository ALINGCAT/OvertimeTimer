using OvertimeTimer.App.Views;

namespace OvertimeTimer.App.Services;

public sealed class SettingsDialogService : ISettingsDialogService
{
    public void Show()
    {
        var window = new SettingsWindow
        {
            Owner = System.Windows.Application.Current?.MainWindow
        };

        window.ShowDialog();
    }
}
