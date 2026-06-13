using System.Threading;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace OvertimeTimer.App.Services;

public sealed class StatusMessageService : BindableBase, IStatusMessageService
{
    private string _message = string.Empty;
    private CancellationTokenSource? _clearCts;

    public string Message
    {
        get => _message;
        private set => SetProperty(ref _message, value);
    }

    public void Show(string message)
    {
        Message = message;

        _clearCts?.Cancel();
        _clearCts = new CancellationTokenSource();
        var token = _clearCts.Token;
        _ = Task.Delay(3000, token).ContinueWith(_ =>
        {
            if (!token.IsCancellationRequested)
                Message = string.Empty;
        }, TaskScheduler.Default);
    }
}
