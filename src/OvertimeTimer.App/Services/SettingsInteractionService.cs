using System.Diagnostics;
using System.IO;
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

    public void OpenSettingsDirectory()
    {
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OvertimeTimer");

        Directory.CreateDirectory(path);
        Process.Start(new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = path,
            UseShellExecute = true
        });
    }
}
