using System.ComponentModel;

namespace OvertimeTimer.App.Localization;

public interface ILocalizationService : INotifyPropertyChanged
{
    string this[string key] { get; }
    string CurrentLanguage { get; }
    IReadOnlyList<LanguageItem> AvailableLanguages { get; }
    Task SetLanguageAsync(string languageCode);
    Task LoadAsync();
}
