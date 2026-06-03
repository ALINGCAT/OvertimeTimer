namespace OvertimeTimer.App.Services;

public interface ISettingsInteractionService
{
    string? ChooseFolder(string initialPath);

    bool ConfirmCreateFolder(string path);
}
