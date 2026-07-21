using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace OvertimeTimer.App.Views;

public partial class MainView : System.Windows.Controls.UserControl
{
    private bool _webViewInitialized;

    public MainView()
    {
        InitializeComponent();
        System.Windows.DataObject.AddPastingHandler(OvertimeHoursInput, OnNumericPaste);
        System.Windows.DataObject.AddPastingHandler(OvertimeMinutesInput, OnNumericPaste);
        System.Windows.DataObject.AddPastingHandler(DiaryTextBox, OnDiaryPaste);

        DataContextChanged += (_, _) => SetupWebView();

        if (DataContext is ViewModels.MainViewModel)
            SetupWebView();
    }

    private async void SetupWebView()
    {
        if (_webViewInitialized || DataContext is null)
            return;

        if (DataContext is ViewModels.MainViewModel vm)
        {
            vm.SelectedDayRecord.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(ViewModels.DayRecordViewModel.HtmlPreview))
                {
                    RefreshPreview();
                }
            };
        }

        try
        {
            var userDataDir = Path.Combine(Path.GetTempPath(), "OvertimeTimer_WebView2");
            var env = await CoreWebView2Environment.CreateAsync(null, userDataDir);
            await PreviewWebView.EnsureCoreWebView2Async(env);
        }
        catch
        {
            return;
        }

        _webViewInitialized = true;
        RefreshPreview();
    }

    private void DiaryTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (DataContext is not ViewModels.MainViewModel vm) return;

        if (e.Key is Key.LeftCtrl or Key.RightCtrl or Key.LeftShift or Key.RightShift
            or Key.LeftAlt or Key.RightAlt or Key.LWin or Key.RWin or Key.System
            or Key.Home or Key.End or Key.PageUp or Key.PageDown or Key.Insert
            or Key.Left or Key.Up or Key.Right or Key.Down
            or Key.Scroll or Key.Pause or Key.PrintScreen
            or Key.NumLock or Key.CapsLock or Key.Escape)
            return;

        if (e.Key >= Key.F1 && e.Key <= Key.F12)
            return;

        if (e.Key is Key.Delete or Key.Back)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var isEmpty = string.IsNullOrWhiteSpace(DiaryTextBox.Text);
                var day = vm.CalendarDays.FirstOrDefault(d => d.Date == vm.SelectedDayRecord.Date);
                if (isEmpty && day is not null && !day.HasDiary)
                    vm.SelectedDayRecord.IsDirty = false;
                else if (!isEmpty)
                    vm.SelectedDayRecord.IsDirty = true;
            }), System.Windows.Threading.DispatcherPriority.Background);
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) != 0 && e.Key is Key.C or Key.A or Key.Insert)
            return;

        // Ctrl+Shift+S is "Save As" — do not mark dirty
        if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift)
            && e.Key is Key.S)
            return;

        vm.SelectedDayRecord.IsDirty = true;
    }

    private void RefreshPreview()
    {
        if (!_webViewInitialized || DataContext is not ViewModels.MainViewModel vm)
            return;

        PreviewWebView.CoreWebView2.NavigateToString(vm.SelectedDayRecord.HtmlPreview);
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

    private void OnDiaryPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (DataContext is ViewModels.MainViewModel vm)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var isEmpty = string.IsNullOrWhiteSpace(DiaryTextBox.Text);
                var day = vm.CalendarDays.FirstOrDefault(d => d.Date == vm.SelectedDayRecord.Date);
                if (!isEmpty)
                    vm.SelectedDayRecord.IsDirty = true;
                else if (day is not null && !day.HasDiary)
                    vm.SelectedDayRecord.IsDirty = false;
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
