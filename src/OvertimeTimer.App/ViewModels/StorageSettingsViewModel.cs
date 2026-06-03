using System.IO;
using Prism.Commands;
using OvertimeTimer.App.Services;
using OvertimeTimer.Core.Models;

namespace OvertimeTimer.App.ViewModels;

public sealed class StorageSettingsViewModel : SettingsSectionViewModelBase
{
    private const string SaveSuccessMessage = "存储设置已保存。";

    private readonly ISettingsInteractionService _settingsInteractionService;
    private readonly Func<Task> _saveAsync;
    private string _diaryRootPath = Path.Combine(AppContext.BaseDirectory, "dailies");
    private DiaryStorageMode _diaryStorageMode = DiaryStorageMode.Flat;

    public StorageSettingsViewModel(
        ISettingsInteractionService settingsInteractionService,
        Func<Task> saveAsync)
    {
        _settingsInteractionService = settingsInteractionService;
        _saveAsync = saveAsync;
        ChooseDiaryRootPathCommand = new DelegateCommand(ChooseDiaryRootPath);
        SaveCommand = new DelegateCommand(() => _ = SaveCurrentSectionAsync());
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
        DiaryStorageMode.Flat => "所有日记文件直接放在根目录中。",
        DiaryStorageMode.ByYear => "按年份创建子目录，例如 2026\\2026-06-03.md。",
        DiaryStorageMode.ByMonth => "按年月创建子目录，例如 2026-06\\2026-06-03.md。",
        _ => string.Empty
    };

    public string DiaryStoragePathStatus => string.IsNullOrWhiteSpace(DiaryRootPath)
        ? "请先选择或输入日记根目录。"
        : $"当前目录：{DiaryRootPath}";

    public DelegateCommand ChooseDiaryRootPathCommand { get; }

    public DelegateCommand SaveCommand { get; }

    public void LoadFrom(DiaryStorageConfig diaryStorageConfig)
    {
        DiaryRootPath = string.IsNullOrWhiteSpace(diaryStorageConfig.RootPath)
            ? Path.Combine(AppContext.BaseDirectory, "dailies")
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

    private async Task SaveCurrentSectionAsync()
    {
        if (!TryValidateDiaryRootPath(out var feedbackMessage, out var isError))
        {
            await ShowSaveFeedbackAsync(feedbackMessage, isError);
            return;
        }

        try
        {
            await _saveAsync();
        }
        catch (Exception)
        {
            await ShowSaveFeedbackAsync("存储设置保存失败。", true);
            return;
        }

        await ShowSaveFeedbackAsync(SaveSuccessMessage, false);
    }

    private bool TryValidateDiaryRootPath(out string feedbackMessage, out bool isError)
    {
        if (string.IsNullOrWhiteSpace(DiaryRootPath))
        {
            feedbackMessage = "日记根目录不能为空。";
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
            feedbackMessage = "已取消保存。";
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
            feedbackMessage = "创建目录失败，请检查路径是否有效或是否有权限。";
            isError = true;
            return false;
        }
    }
}
