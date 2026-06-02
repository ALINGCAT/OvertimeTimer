using Prism.Commands;
using OvertimeTimer.App.Services;

namespace OvertimeTimer.App.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly IStatusMessageService _statusMessageService;
    private readonly ISettingsDialogService _settingsDialogService;
    private object _currentView;

    public MainWindowViewModel(
        IStatusMessageService statusMessageService,
        ISettingsDialogService settingsDialogService,
        MainViewModel mainViewModel,
        SettingsViewModel settingsViewModel)
    {
        _statusMessageService = statusMessageService;
        _settingsDialogService = settingsDialogService;
        _currentView = mainViewModel;

        _statusMessageService.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(IStatusMessageService.Message))
            {
                RaisePropertyChanged(nameof(StatusMessage));
            }
        };

        NavigateSettingsCommand = new DelegateCommand(() => _settingsDialogService.Show());
    }

    public string StatusMessage => _statusMessageService.Message;

    public object CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }

    public DelegateCommand NavigateSettingsCommand { get; }
}
