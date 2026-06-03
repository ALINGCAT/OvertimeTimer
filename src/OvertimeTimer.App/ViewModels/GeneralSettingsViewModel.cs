using System.Collections.ObjectModel;
using System.Windows.Media;
using Prism.Commands;
using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Services;

namespace OvertimeTimer.App.ViewModels;

public sealed class GeneralSettingsViewModel : SettingsSectionViewModelBase
{
    private readonly ILocalizationService _localizationService;
    private readonly StorageSettingsViewModel _storageSection;
    private LanguageItem? _selectedLanguage;

    public GeneralSettingsViewModel(
        ISettingsInteractionService settingsInteractionService,
        Func<Task> saveAsync,
        ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        _storageSection = new StorageSettingsViewModel(settingsInteractionService, saveAsync, localizationService);
        AvailableLanguages = new ObservableCollection<LanguageItem>(localizationService.AvailableLanguages);
        _selectedLanguage = AvailableLanguages.FirstOrDefault(
            l => l.Code == localizationService.CurrentLanguage);

        foreach (var family in Fonts.SystemFontFamilies.OrderBy(f => f.Source))
        {
            AvailableFontFamilies.Add(family.Source);
        }

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
    }

    public StorageSettingsViewModel StorageSection => _storageSection;

    public ObservableCollection<LanguageItem> AvailableLanguages { get; }

    public LanguageItem? SelectedLanguage
    {
        get => _selectedLanguage;
        set => SetProperty(ref _selectedLanguage, value);
    }

    private string _previewFontFamily = "Microsoft YaHei UI";
    public string PreviewFontFamily
    {
        get => _previewFontFamily;
        set => SetProperty(ref _previewFontFamily, value);
    }

    private double _previewFontSize = 8;
    public double PreviewFontSize
    {
        get => _previewFontSize;
        set => SetProperty(ref _previewFontSize, value);
    }

    private double _previewLineHeight = 12;
    public double PreviewLineHeight
    {
        get => _previewLineHeight;
        set => SetProperty(ref _previewLineHeight, value);
    }

    public ObservableCollection<string> AvailableFontFamilies { get; } = new();

    public DelegateCommand SaveCommand { get; }

    public void LoadFrom(string fontFamily, double fontSize, double lineHeight)
    {
        PreviewFontFamily = fontFamily;
        PreviewFontSize = fontSize;
        PreviewLineHeight = lineHeight;
    }

    private async Task SaveAsync()
    {
        var saved = await _storageSection.SaveAsync();
        if (!saved)
        {
            return;
        }

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
