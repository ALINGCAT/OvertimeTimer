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
    private bool _htmlLoaded;

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

    private void RefreshPreview()
    {
        if (!_webViewInitialized || DataContext is not ViewModels.MainViewModel vm)
            return;

        if (!_htmlLoaded)
        {
            PreviewWebView.CoreWebView2.NavigateToString(vm.SelectedDayRecord.HtmlPreview);
            _htmlLoaded = true;
        }
        else
        {
            var body = vm.SelectedDayRecord.BodyHtml
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\r\n", "")
                .Replace("\n", "");
            PreviewWebView.CoreWebView2.ExecuteScriptAsync(
                $"document.body.innerHTML='{body}';");
        }
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
