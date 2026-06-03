using System.Collections.ObjectModel;
using Prism.Commands;
using OvertimeTimer.App.Localization;

namespace OvertimeTimer.App.ViewModels;

public sealed class LanguageSettingsViewModel : SettingsSectionViewModelBase
{
    private readonly ILocalizationService _localizationService;
    private LanguageItem? _selectedLanguage;

    public LanguageSettingsViewModel(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        AvailableLanguages = new ObservableCollection<LanguageItem>(_localizationService.AvailableLanguages);
        _selectedLanguage = AvailableLanguages.FirstOrDefault(
            l => l.Code == _localizationService.CurrentLanguage);
        SelectLanguageCommand = new DelegateCommand<LanguageItem>(SelectLanguage);
    }

    public ObservableCollection<LanguageItem> AvailableLanguages { get; }

    public LanguageItem? SelectedLanguage
    {
        get => _selectedLanguage;
        set => SetProperty(ref _selectedLanguage, value);
    }

    public DelegateCommand<LanguageItem> SelectLanguageCommand { get; }

    private async void SelectLanguage(LanguageItem? language)
    {
        if (language is null)
        {
            return;
        }

        SelectedLanguage = language;
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
