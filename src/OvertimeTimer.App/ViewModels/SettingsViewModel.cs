using OvertimeTimer.App.Services;
using Prism.Commands;

namespace OvertimeTimer.App.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsPersistenceCoordinator _settingsPersistenceCoordinator;
    private SettingsSection _selectedSection = SettingsSection.WorkSchedule;

    public SettingsViewModel(
        IAppearanceSettingsService appearanceSettingsService,
        IColorSelectionService colorSelectionService,
        ISettingsInteractionService settingsInteractionService,
        ISettingsPersistenceCoordinator settingsPersistenceCoordinator)
    {
        _settingsPersistenceCoordinator = settingsPersistenceCoordinator;

        WorkScheduleSection = new WorkScheduleSettingsViewModel(SaveSettingsAsync);
        StorageSection = new StorageSettingsViewModel(settingsInteractionService, SaveSettingsAsync);
        AppearanceSection = new AppearanceSettingsViewModel(appearanceSettingsService, colorSelectionService, SaveSettingsAsync);
        LanguageSection = new LanguageSettingsViewModel();

        ShowWorkScheduleSectionCommand = new DelegateCommand(() => SelectSection(SettingsSection.WorkSchedule));
        ShowStorageSectionCommand = new DelegateCommand(() => SelectSection(SettingsSection.Storage));
        ShowAppearanceSectionCommand = new DelegateCommand(() => SelectSection(SettingsSection.Appearance));
        ShowLanguageSectionCommand = new DelegateCommand(() => SelectSection(SettingsSection.Language));

        _ = LoadSettingsAsync();
    }

    public string CurrentSectionTitle => SelectedSection switch
    {
        SettingsSection.WorkSchedule => "工作日规则",
        SettingsSection.Storage => "存储设置",
        SettingsSection.Appearance => "外观设置",
        SettingsSection.Language => "语言设置",
        _ => string.Empty
    };

    public string CurrentSectionDescription => SelectedSection switch
    {
        SettingsSection.WorkSchedule => "配置按周或按天的工作日循环规则。",
        SettingsSection.Storage => "配置日记文件的根目录和分组方式。",
        SettingsSection.Appearance => "配置窗口背景、月历日期和提示点的颜色。",
        SettingsSection.Language => "后续将在这里配置界面语言与语言包。",
        _ => string.Empty
    };

    public SettingsSection SelectedSection
    {
        get => _selectedSection;
        private set
        {
            if (SetProperty(ref _selectedSection, value))
            {
                RaisePropertyChanged(nameof(IsWorkScheduleSectionSelected));
                RaisePropertyChanged(nameof(IsStorageSectionSelected));
                RaisePropertyChanged(nameof(IsAppearanceSectionSelected));
                RaisePropertyChanged(nameof(IsLanguageSectionSelected));
                RaisePropertyChanged(nameof(CurrentSectionTitle));
                RaisePropertyChanged(nameof(CurrentSectionDescription));
            }
        }
    }

    public bool IsWorkScheduleSectionSelected => SelectedSection == SettingsSection.WorkSchedule;

    public bool IsStorageSectionSelected => SelectedSection == SettingsSection.Storage;

    public bool IsAppearanceSectionSelected => SelectedSection == SettingsSection.Appearance;

    public bool IsLanguageSectionSelected => SelectedSection == SettingsSection.Language;

    public WorkScheduleSettingsViewModel WorkScheduleSection { get; }

    public StorageSettingsViewModel StorageSection { get; }

    public AppearanceSettingsViewModel AppearanceSection { get; }

    public LanguageSettingsViewModel LanguageSection { get; }

    public DelegateCommand ShowWorkScheduleSectionCommand { get; }

    public DelegateCommand ShowStorageSectionCommand { get; }

    public DelegateCommand ShowAppearanceSectionCommand { get; }

    public DelegateCommand ShowLanguageSectionCommand { get; }

    private void SelectSection(SettingsSection section)
    {
        SelectedSection = section;
    }

    private async Task LoadSettingsAsync()
    {
        try
        {
            await _settingsPersistenceCoordinator.LoadAsync(WorkScheduleSection, StorageSection, AppearanceSection);
        }
        catch (Exception)
        {
            await WorkScheduleSection.ShowLoadFailedFeedbackAsync();
            return;
        }
    }

    private async Task SaveSettingsAsync()
    {
        await _settingsPersistenceCoordinator.SaveAsync(WorkScheduleSection, StorageSection, AppearanceSection);
    }
}
