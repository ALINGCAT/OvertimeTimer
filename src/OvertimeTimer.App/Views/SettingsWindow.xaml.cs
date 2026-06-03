using System.Windows;
using OvertimeTimer.App.Localization;

namespace OvertimeTimer.App.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        Title = LocalizationService.Instance["Settings.Title"];
    }
}
