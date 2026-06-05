namespace OvertimeTimer.App.Services;

public interface ISettingsInteractionService
{
    string? ChooseFolder(string initialPath);

    bool ConfirmCreateFolder(string path);

    void OpenSettingsDirectory();

    string? SaveFile(string filter, string defaultFileName);

    string? OpenFile(string filter);
}
