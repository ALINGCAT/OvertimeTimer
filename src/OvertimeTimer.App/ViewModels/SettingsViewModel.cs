using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Services;
using Prism.Commands;

namespace OvertimeTimer.App.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsPersistenceCoordinator _settingsPersistenceCoordinator;
    private SettingsSection _selectedSection = SettingsSection.General;
    private readonly ILocalizationService _loc;

    public SettingsViewModel(
        IAppearanceSettingsService appearanceSettingsService,
        IColorSelectionService colorSelectionService,
        ISettingsInteractionService settingsInteractionService,
        ISettingsPersistenceCoordinator settingsPersistenceCoordinator,
        ILocalizationService localizationService)
    {
        _settingsPersistenceCoordinator = settingsPersistenceCoordinator;
        _loc = localizationService;

        _loc.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]")
            {
                RaisePropertyChanged(nameof(CurrentSectionTitle));
                RaisePropertyChanged(nameof(CurrentSectionDescription));
            }
        };

        GeneralSection = new GeneralSettingsViewModel(settingsInteractionService, SaveSettingsAsync, localizationService);
        WorkScheduleSection = new WorkScheduleSettingsViewModel(SaveSettingsAsync, localizationService);
        AppearanceSection = new AppearanceSettingsViewModel(appearanceSettingsService, colorSelectionService, SaveSettingsAsync, localizationService);
        PreviewSection = new PreviewSettingsViewModel(appearanceSettingsService, colorSelectionService, SaveSettingsAsync, localizationService, settingsInteractionService);

        ShowGeneralSectionCommand = new DelegateCommand(() => SelectSection(SettingsSection.General));
        ShowWorkScheduleSectionCommand = new DelegateCommand(() => SelectSection(SettingsSection.WorkSchedule));
        ShowAppearanceSectionCommand = new DelegateCommand(() => SelectSection(SettingsSection.Appearance));
        ShowPreviewSectionCommand = new DelegateCommand(() => SelectSection(SettingsSection.Preview));

        _ = LoadSettingsAsync();
    }

    public string CurrentSectionTitle => SelectedSection switch
    {
        SettingsSection.General => _loc["Settings.General"],
        SettingsSection.WorkSchedule => _loc["Settings.WorkSchedule"],
        SettingsSection.Appearance => _loc["Settings.Appearance"],
        SettingsSection.Preview => _loc["Settings.Preview"],
        _ => string.Empty
    };

    public string CurrentSectionDescription => SelectedSection switch
    {
        SettingsSection.General => _loc["Settings.GeneralDesc"],
        SettingsSection.WorkSchedule => _loc["Settings.WorkScheduleDesc"],
        SettingsSection.Appearance => _loc["Settings.AppearanceDesc"],
        SettingsSection.Preview => _loc["Settings.PreviewDesc"],
        _ => string.Empty
    };

    public SettingsSection SelectedSection
    {
        get => _selectedSection;
        private set
        {
            if (SetProperty(ref _selectedSection, value))
            {
                RaisePropertyChanged(nameof(IsGeneralSectionSelected));
                RaisePropertyChanged(nameof(IsWorkScheduleSectionSelected));
                RaisePropertyChanged(nameof(IsAppearanceSectionSelected));
                RaisePropertyChanged(nameof(IsPreviewSectionSelected));
                RaisePropertyChanged(nameof(CurrentSectionTitle));
                RaisePropertyChanged(nameof(CurrentSectionDescription));
            }
        }
    }

    public bool IsGeneralSectionSelected => SelectedSection == SettingsSection.General;

    public bool IsWorkScheduleSectionSelected => SelectedSection == SettingsSection.WorkSchedule;

    public bool IsAppearanceSectionSelected => SelectedSection == SettingsSection.Appearance;

    public bool IsPreviewSectionSelected => SelectedSection == SettingsSection.Preview;

    public GeneralSettingsViewModel GeneralSection { get; }

    public WorkScheduleSettingsViewModel WorkScheduleSection { get; }

    public AppearanceSettingsViewModel AppearanceSection { get; }

    public PreviewSettingsViewModel PreviewSection { get; }

    public DelegateCommand ShowGeneralSectionCommand { get; }

    public DelegateCommand ShowWorkScheduleSectionCommand { get; }

    public DelegateCommand ShowAppearanceSectionCommand { get; }

    public DelegateCommand ShowPreviewSectionCommand { get; }

    private void SelectSection(SettingsSection section) => SelectedSection = section;

    private async Task LoadSettingsAsync()
    {
        try
        {
            await _settingsPersistenceCoordinator.LoadAsync(
                WorkScheduleSection, GeneralSection.StorageSection, AppearanceSection, GeneralSection, PreviewSection);
        }
        catch (Exception)
        {
            await WorkScheduleSection.ShowLoadFailedFeedbackAsync();
            await GeneralSection.StorageSection.ShowSaveFeedbackAsync(_loc["Settings.LoadFailed"], true);
            await AppearanceSection.ShowSaveFeedbackAsync(_loc["Settings.LoadFailed"], true);
        }
    }

    private async Task SaveSettingsAsync()
    {
        await _settingsPersistenceCoordinator.SaveAsync(
            WorkScheduleSection, GeneralSection.StorageSection, AppearanceSection, GeneralSection, PreviewSection);
    }
}
