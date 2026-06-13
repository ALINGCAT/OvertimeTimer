using System.IO;
using Prism.Commands;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Services;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.ViewModels;

public sealed class StorageSettingsViewModel : SettingsSectionViewModelBase
{
    private readonly ISettingsInteractionService _settingsInteractionService;
    private readonly Func<Task> _saveAsync;
    private readonly ILocalizationService _loc;
    private string _diaryRootPath = DiaryFileService.GetDefaultDiaryRoot();
    private DiaryStorageMode _diaryStorageMode = DiaryStorageMode.Flat;

    public StorageSettingsViewModel(
        ISettingsInteractionService settingsInteractionService,
        Func<Task> saveAsync,
        ILocalizationService localizationService)
    {
        _settingsInteractionService = settingsInteractionService;
        _saveAsync = saveAsync;
        _loc = localizationService;
        ChooseDiaryRootPathCommand = new DelegateCommand(ChooseDiaryRootPath);
        SaveCommand = new DelegateCommand(() => _ = SaveAsync());

        _loc.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]")
            {
                RaisePropertyChanged(nameof(DiaryStorageModeDescription));
                RaisePropertyChanged(nameof(DiaryStoragePathStatus));
            }
        };

        PropertyChanged += (_, _) => ScheduleAutoSave(SaveAsync);
    }

    public string DiaryRootPath
    {
        get => _diaryRootPath;
        set
        {
            if (SetProperty(ref _diaryRootPath, value))
            {
                RaisePropertyChanged(nameof(DiaryStorageModeDescription));
                RaisePropertyChanged(nameof(DiaryStoragePathStatus));
            }
        }
    }

    public DiaryStorageMode DiaryStorageMode
    {
        get => _diaryStorageMode;
        set
        {
            if (SetProperty(ref _diaryStorageMode, value))
            {
                RaisePropertyChanged(nameof(IsFlatStorageMode));
                RaisePropertyChanged(nameof(IsByYearStorageMode));
                RaisePropertyChanged(nameof(IsByMonthStorageMode));
                RaisePropertyChanged(nameof(DiaryStorageModeDescription));
            }
        }
    }

    public bool IsFlatStorageMode
    {
        get => DiaryStorageMode == DiaryStorageMode.Flat;
        set
        {
            if (value)
            {
                DiaryStorageMode = DiaryStorageMode.Flat;
            }
        }
    }

    public bool IsByYearStorageMode
    {
        get => DiaryStorageMode == DiaryStorageMode.ByYear;
        set
        {
            if (value)
            {
                DiaryStorageMode = DiaryStorageMode.ByYear;
            }
        }
    }

    public bool IsByMonthStorageMode
    {
        get => DiaryStorageMode == DiaryStorageMode.ByMonth;
        set
        {
            if (value)
            {
                DiaryStorageMode = DiaryStorageMode.ByMonth;
            }
        }
    }

    public string DiaryStorageModeDescription => DiaryStorageMode switch
    {
        DiaryStorageMode.Flat => _loc["Settings.StorageFlatDesc"],
        DiaryStorageMode.ByYear => _loc["Settings.StorageByYearDesc"],
        DiaryStorageMode.ByMonth => _loc["Settings.StorageByMonthDesc"],
        _ => string.Empty
    };

    public string DiaryStoragePathStatus => string.IsNullOrWhiteSpace(DiaryRootPath)
        ? _loc["Settings.StoragePathHint"]
        : string.Format(_loc["Settings.StorageCurrentPath"], DiaryRootPath);

    public DelegateCommand ChooseDiaryRootPathCommand { get; }

    public DelegateCommand SaveCommand { get; }

    public void LoadFrom(DiaryStorageConfig diaryStorageConfig)
    {
        DiaryRootPath = string.IsNullOrWhiteSpace(diaryStorageConfig.RootPath)
            ? DiaryFileService.GetDefaultDiaryRoot()
            : diaryStorageConfig.RootPath;
        DiaryStorageMode = diaryStorageConfig.Mode;
    }

    public DiaryStorageConfig ToModel()
    {
        return new DiaryStorageConfig
        {
            RootPath = DiaryRootPath,
            Mode = DiaryStorageMode
        };
    }

    private void ChooseDiaryRootPath()
    {
        var selectedPath = _settingsInteractionService.ChooseFolder(DiaryRootPath);
        if (!string.IsNullOrWhiteSpace(selectedPath))
        {
            DiaryRootPath = selectedPath;
        }
    }

    public async Task<bool> SaveAsync()
    {
        if (!TryValidateDiaryRootPath(out _, out var isError)) return !isError;
        try { await _saveAsync(); }
        catch { return false; }
        return true;
    }

    private bool TryValidateDiaryRootPath(out string feedbackMessage, out bool isError)
    {
        if (string.IsNullOrWhiteSpace(DiaryRootPath))
        {
            feedbackMessage = _loc["Settings.StorageRootPathEmpty"];
            isError = true;
            return false;
        }

        if (Directory.Exists(DiaryRootPath))
        {
            feedbackMessage = string.Empty;
            isError = false;
            return true;
        }

        if (!_settingsInteractionService.ConfirmCreateFolder(DiaryRootPath))
        {
            feedbackMessage = _loc["Settings.StorageSaveCancelled"];
            isError = true;
            return false;
        }

        try
        {
            Directory.CreateDirectory(DiaryRootPath);
            feedbackMessage = string.Empty;
            isError = false;
            return true;
        }
        catch (Exception)
        {
            feedbackMessage = _loc["Settings.StorageCreateDirFailed"];
            isError = true;
            return false;
        }
    }
}
