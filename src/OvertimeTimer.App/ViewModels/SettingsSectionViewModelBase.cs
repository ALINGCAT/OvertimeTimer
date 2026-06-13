using System.Threading;
using System.Threading.Tasks;

namespace OvertimeTimer.App.ViewModels;

public abstract class SettingsSectionViewModelBase : ViewModelBase
{
    private string _saveFeedbackMessage = string.Empty;
    private bool _isSaveFeedbackError;
    private CancellationTokenSource? _saveFeedbackCancellationTokenSource;

    public string SaveFeedbackMessage
    {
        get => _saveFeedbackMessage;
        protected set
        {
            if (SetProperty(ref _saveFeedbackMessage, value))
            {
                RaisePropertyChanged(nameof(HasSaveFeedback));
            }
        }
    }

    public bool HasSaveFeedback => !string.IsNullOrWhiteSpace(SaveFeedbackMessage);

    public bool IsSaveFeedbackError
    {
        get => _isSaveFeedbackError;
        protected set => SetProperty(ref _isSaveFeedbackError, value);
    }

    public async Task ShowSaveFeedbackAsync(string message, bool isError)
    {
        _saveFeedbackCancellationTokenSource?.Cancel();
        _saveFeedbackCancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _saveFeedbackCancellationTokenSource.Token;

        IsSaveFeedbackError = isError;
        SaveFeedbackMessage = message;

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            SaveFeedbackMessage = string.Empty;
        }
        catch (TaskCanceledException)
        {
        }
    }

    private CancellationTokenSource? _autoSaveCts;

    protected void ScheduleAutoSave(Func<Task> save)
    {
        _autoSaveCts?.Cancel();
        _autoSaveCts = new CancellationTokenSource();
        var token = _autoSaveCts.Token;
        _ = Task.Delay(100, token).ContinueWith(_ =>
        {
            if (!token.IsCancellationRequested) save();
        }, TaskScheduler.Default);
    }
}
