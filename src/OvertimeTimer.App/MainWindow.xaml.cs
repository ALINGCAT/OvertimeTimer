using System.ComponentModel;
using System.Windows;
using OvertimeTimer.App.Localization;

namespace OvertimeTimer.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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
