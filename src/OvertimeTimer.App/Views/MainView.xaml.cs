using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
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
                Debug.WriteLine($"[DayRecord.PropertyChanged] prop={e.PropertyName}");
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
        catch (Exception ex)
        {
            Debug.WriteLine($"WebView2 init failed: {ex.Message}");
            PreviewErrorText.Visibility = Visibility.Visible;
            return;
        }

        _webViewInitialized = true;
        RefreshPreview();
    }

    private void RefreshPreview()
    {
        Debug.WriteLine($"[RefreshPreview] webViewInit={_webViewInitialized}, vm={DataContext != null}");

        if (!_webViewInitialized || DataContext is not ViewModels.MainViewModel vm)
            return;

        var html = vm.SelectedDayRecord.HtmlPreview;
        Debug.WriteLine($"[NavigateToString] len={html.Length}, start={html.Substring(0, Math.Min(html.Length, 120))}");
        PreviewWebView.CoreWebView2.NavigateToString(html);
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
