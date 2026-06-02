using System.Collections.ObjectModel;

namespace OvertimeTimer.App.DesignTime;

public sealed class DesignMainWindowViewModel
{
    public string StatusMessage { get; } = "设计时状态";

    public object CurrentView { get; } = new DesignMainViewModel();

    public object NavigateHomeCommand { get; } = new object();

    public object NavigateSettingsCommand { get; } = new object();
}
