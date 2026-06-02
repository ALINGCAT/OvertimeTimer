using Prism.Commands;
using OvertimeTimer.App.Services;

namespace OvertimeTimer.App.ViewModels;

public sealed class DayRecordViewModel : ViewModelBase
{
    private readonly IStatusMessageService _statusMessageService;
    private DateOnly _date = DateOnly.FromDateTime(DateTime.Today);
    private int _overtimeHours;
    private int _overtimeMinutes;
    private string _diaryMarkdown = string.Empty;
    private bool _isDirty;

    public DayRecordViewModel(IStatusMessageService statusMessageService)
    {
        _statusMessageService = statusMessageService;
        SaveCommand = new DelegateCommand(Save);
    }

    public DateOnly Date
    {
        get => _date;
        set
        {
            if (SetProperty(ref _date, value))
            {
                RaisePropertyChanged(nameof(DateDisplay));
            }
        }
    }

    public string DateDisplay => Date.ToString("yyyy-MM-dd");

    public int OvertimeHours
    {
        get => _overtimeHours;
        set
        {
            if (SetProperty(ref _overtimeHours, value))
            {
                RaisePropertyChanged(nameof(OvertimeDisplay));
            }
        }
    }

    public int OvertimeMinutes
    {
        get => _overtimeMinutes;
        set
        {
            if (SetProperty(ref _overtimeMinutes, value))
            {
                RaisePropertyChanged(nameof(OvertimeDisplay));
            }
        }
    }

    public string OvertimeDisplay => $"{OvertimeHours} 小时 {OvertimeMinutes} 分钟";

    public string DiaryMarkdown
    {
        get => _diaryMarkdown;
        set
        {
            if (SetProperty(ref _diaryMarkdown, value))
            {
                RaisePropertyChanged(nameof(DiaryPreview));
            }
        }
    }

    public string DiaryPreview => DiaryMarkdown;

    public bool IsDirty
    {
        get => _isDirty;
        private set => SetProperty(ref _isDirty, value);
    }

    public DelegateCommand SaveCommand { get; }

    private void Save()
    {
        _statusMessageService.Show("日记已保存（待接入持久化）");
        IsDirty = false;
    }
}
