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
        _selectedLanguage = AvailableLanguages.FirstOrDefault(
            l => l.Code == localizationService.CurrentLanguage);

        _storageSection.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(SettingsSectionViewModelBase.HasSaveFeedback)
                || args.PropertyName == nameof(SettingsSectionViewModelBase.SaveFeedbackMessage))
            {
                RaisePropertyChanged(nameof(HasSaveFeedback));
                RaisePropertyChanged(nameof(SaveFeedbackMessage));
            }
        };

        SaveCommand = new DelegateCommand(() => _ = SaveAsync());
        OpenSettingsDirectoryCommand = new DelegateCommand(() => _settingsInteractionService.OpenSettingsDirectory());
    }

    public StorageSettingsViewModel StorageSection => _storageSection;

    public ObservableCollection<LanguageItem> AvailableLanguages { get; }

    public LanguageItem? SelectedLanguage
    {
        get => _selectedLanguage;
        set => SetProperty(ref _selectedLanguage, value);
    }

    public DelegateCommand SaveCommand { get; }

    public DelegateCommand OpenSettingsDirectoryCommand { get; }

    private async Task SaveAsync()
    {
        var saved = await _storageSection.SaveAsync();
        if (!saved)
            return;

        if (_selectedLanguage is not null && _selectedLanguage.Code != _localizationService.CurrentLanguage)
        {
            await SetLanguageAsync(_selectedLanguage);
        }
    }

    private async Task SetLanguageAsync(LanguageItem language)
    {
        try
        {
            await _localizationService.SetLanguageAsync(language.Code);
        }
        catch (Exception)
        {
            await ShowSaveFeedbackAsync(_localizationService["Language.SwitchFailed"], true);
        }
    }
}
