using System.ComponentModel;
using System.Windows;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Services;

namespace OvertimeTimer.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowThemeHelper.Attach(this);
        var loc = LocalizationService.Instance;
        Title = loc["Main.Title"];

        loc.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]")
            {
                Title = loc["Main.Title"];
            }
        };
    }
}
