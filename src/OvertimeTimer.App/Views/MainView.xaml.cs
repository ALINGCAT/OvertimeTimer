using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OvertimeTimer.App.Views;

public partial class MainView : System.Windows.Controls.UserControl
{
    public MainView()
    {
        InitializeComponent();
        System.Windows.DataObject.AddPastingHandler(OvertimeHoursInput, OnNumericPaste);
        System.Windows.DataObject.AddPastingHandler(OvertimeMinutesInput, OnNumericPaste);
    }

    private static void OnNumericPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (!e.DataObject.GetDataPresent(typeof(string)))
        {
            e.CancelCommand();
            return;
        }

        var text = (string)e.DataObject.GetData(typeof(string))!;
        if (!Regex.IsMatch(text, @"^\d+$"))
        {
            e.CancelCommand();
        }
    }
}
