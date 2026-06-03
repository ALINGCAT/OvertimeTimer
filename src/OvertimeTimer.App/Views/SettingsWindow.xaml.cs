using System.ComponentModel;
using System.Windows;
using OvertimeTimer.App.Localization;

namespace OvertimeTimer.App.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        var loc = LocalizationService.Instance;
        Title = loc["Settings.Title"];

        loc.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]")
            {
                Title = loc["Settings.Title"];
            }
        };
    }
}
