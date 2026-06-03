using System.Windows;

namespace OvertimeTimer.App.Services;

public sealed class SettingsInteractionService : ISettingsInteractionService
{
    public string? ChooseFolder(string initialPath)
    {
        using var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "选择日记根目录",
            UseDescriptionForTitle = true,
            InitialDirectory = string.IsNullOrWhiteSpace(initialPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : initialPath
        };

        return dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK ? dialog.SelectedPath : null;
    }

    public bool ConfirmCreateFolder(string path)
    {
        var result = System.Windows.MessageBox.Show(
            $"路径不存在：\n{path}\n\n是否要在此路径新建文件夹？",
            "路径不存在",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        return result == System.Windows.MessageBoxResult.Yes;
    }
}
