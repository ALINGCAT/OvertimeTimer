using System.Collections.ObjectModel;
using Prism.Commands;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Services;

namespace OvertimeTimer.App.ViewModels;

public sealed class GeneralSettingsViewModel : SettingsSectionViewModelBase
{
    private readonly ILocalizationService _localizationService;
    private readonly ISettingsInteractionService _settingsInteractionService;
    private readonly StorageSettingsViewModel _storageSection;
    private LanguageItem? _selectedLanguage;

    public GeneralSettingsViewModel(
        ISettingsInteractionService settingsInteractionService,
        Func<Task> saveAsync,
        ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        _settingsInteractionService = settingsInteractionService;
        _storageSection = new StorageSettingsViewModel(settingsInteractionService, saveAsync, localizationService);
        AvailableLanguages = new ObservableCollection<LanguageItem>(localizationService.AvailableLanguages);
        _selectedLanguage = AvailableLanguages.FirstOrDefault(l => l.Code == localizationService.CurrentLanguage);

        _storageSection.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(HasSaveFeedback) || args.PropertyName == nameof(SaveFeedbackMessage))
            {
                RaisePropertyChanged(nameof(HasSaveFeedback));
                RaisePropertyChanged(nameof(SaveFeedbackMessage));
            }
        };

        OpenSettingsDirectoryCommand = new DelegateCommand(() => _settingsInteractionService.OpenSettingsDirectory());
        SaveCommand = new DelegateCommand(() => _ = SaveAsync());
    }

    public StorageSettingsViewModel StorageSection => _storageSection;
    public ObservableCollection<LanguageItem> AvailableLanguages { get; }
    public LanguageItem? SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (SetProperty(ref _selectedLanguage, value) && value is not null)
                _ = SetLanguageAsync(value);
        }
    }
    public DelegateCommand OpenSettingsDirectoryCommand { get; }
    public DelegateCommand SaveCommand { get; }

    private async Task SaveAsync()
    {
        var saved = await _storageSection.SaveAsync();
        if (!saved)
        {
            await ShowSaveFeedbackAsync("保存失败", true);
            return;
        }

        await ShowSaveFeedbackAsync("保存成功", false);
    }

    private async Task SetLanguageAsync(LanguageItem language)
    {
        try { await _localizationService.SetLanguageAsync(language.Code); }
        catch { await ShowSaveFeedbackAsync(_localizationService["Language.SwitchFailed"], true); }
    }
}
