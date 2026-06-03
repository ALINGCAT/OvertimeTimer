using System.Windows;
using OvertimeTimer.App.Localization;

namespace OvertimeTimer.App.Services;

public sealed class SettingsInteractionService : ISettingsInteractionService
{
    public string? ChooseFolder(string initialPath)
    {
        using var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = LocalizationService.Instance["Settings.StorageRootPath"],
            UseDescriptionForTitle = true,
            InitialDirectory = string.IsNullOrWhiteSpace(initialPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : initialPath
        };

        return dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK ? dialog.SelectedPath : null;
    }

    public bool ConfirmCreateFolder(string path)
    {
        var result = System.Windows.MessageBox.Show(
            string.Format(LocalizationService.Instance["Settings.StorageCreateConfirm"], path),
            LocalizationService.Instance["Settings.StoragePathNotExist"],
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        return result == System.Windows.MessageBoxResult.Yes;
    }
}
