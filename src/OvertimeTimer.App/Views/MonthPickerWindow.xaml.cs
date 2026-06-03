using System.ComponentModel;
using System.Windows;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Services;
using OvertimeTimer.App.ViewModels;

namespace OvertimeTimer.App.Views;

public partial class MonthPickerWindow : Window
{
    public MonthPickerWindow(DateOnly currentMonth)
    {
        InitializeComponent();
        var loc = LocalizationService.Instance;
        Title = loc["MonthPicker.Title"];

        loc.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]")
            {
                Title = loc["MonthPicker.Title"];
            }
        };

        var viewModel = new MonthPickerViewModel(currentMonth);
        viewModel.RequestClose += OnRequestClose;
        DataContext = viewModel;
        SelectedMonth = currentMonth;
    }

    public DateOnly SelectedMonth { get; private set; }

    public bool UseTodayAsSelectedDate { get; private set; }

    private void OnRequestClose(object? sender, MonthSelectionResult result)
    {
        SelectedMonth = result.SelectedMonth;
        UseTodayAsSelectedDate = result.UseTodayAsSelectedDate;
        DialogResult = true;
        Close();
    }
}
