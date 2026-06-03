using Prism.Commands;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Services;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.ViewModels;

public sealed class DayRecordViewModel : ViewModelBase
{
    private readonly IStatusMessageService _statusMessageService;
    private readonly IRecordStoreService _recordStoreService;
    private readonly IDiaryFileService _diaryFileService;
    private readonly ILocalizationService _loc;
    private DateOnly _date = DateOnly.FromDateTime(DateTime.Today);
    private int _overtimeHours;
    private int _overtimeMinutes;
    private string _diaryMarkdown = string.Empty;
    private bool _isDirty;

    public DayRecordViewModel(
        IStatusMessageService statusMessageService,
        IRecordStoreService recordStoreService,
        IDiaryFileService diaryFileService,
        ILocalizationService localizationService)
    {
        _statusMessageService = statusMessageService;
        _recordStoreService = recordStoreService;
        _diaryFileService = diaryFileService;
        _loc = localizationService;
        SaveCommand = new DelegateCommand(() => _ = SaveAsync());

        _loc.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]")
            {
                RaisePropertyChanged(nameof(OvertimeDisplay));
            }
        };
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
                IsDirty = true;
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
                IsDirty = true;
            }
        }
    }

    public string OvertimeDisplay => string.Format(_loc["Calendar.OvertimeFormat"], OvertimeHours, OvertimeMinutes);

    public string DiaryMarkdown
    {
        get => _diaryMarkdown;
        set
        {
            if (SetProperty(ref _diaryMarkdown, value))
            {
                IsDirty = true;
            }
        }
    }

    public bool IsDirty
    {
        get => _isDirty;
        private set => SetProperty(ref _isDirty, value);
    }

    public DelegateCommand SaveCommand { get; }

    public event Action? Saved;

    public async Task LoadAsync(DateOnly date)
    {
        Date = date;
        IsDirty = false;

        var record = await _recordStoreService.LoadAsync(date);
        if (record is not null)
        {
            OvertimeHours = record.OvertimeHours;
            OvertimeMinutes = record.OvertimeMinutes;
        }
        else
        {
            OvertimeHours = 0;
            OvertimeMinutes = 0;
        }

        var diary = await _diaryFileService.LoadDiaryAsync(date);
        DiaryMarkdown = diary;

        IsDirty = false;
    }

    private async Task SaveAsync()
    {
        try
        {
            var record = new DailyRecord
            {
                Date = Date,
                OvertimeHours = OvertimeHours,
                OvertimeMinutes = OvertimeMinutes,
                DiaryMarkdown = DiaryMarkdown,
                LastModified = DateTime.Now
            };

            await _recordStoreService.SaveAsync(record);

            if (string.IsNullOrWhiteSpace(DiaryMarkdown))
            {
                await _diaryFileService.DeleteDiaryAsync(Date);
            }
            else
            {
                await _diaryFileService.SaveDiaryAsync(Date, DiaryMarkdown);
            }

            IsDirty = false;
            _statusMessageService.Show(_loc["Diary.Saved"]);
            Saved?.Invoke();
        }
        catch (Exception)
        {
            _statusMessageService.Show(_loc["Diary.SaveFailed"]);
        }
    }
}
