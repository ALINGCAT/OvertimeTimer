using Prism.Mvvm;

namespace OvertimeTimer.App.Services;

public sealed class StatusMessageService : BindableBase, IStatusMessageService
{
    private string _message = "Ready";

    public string Message
    {
        get => _message;
        private set => SetProperty(ref _message, value);
    }

    public void Show(string message)
    {
        Message = message;
    }
}
