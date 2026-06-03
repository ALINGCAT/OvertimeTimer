using System.IO;
using System.Text.Json;
using OvertimeTimer.Core.Models;

namespace OvertimeTimer.App.Services;

public sealed class SettingsStoreService : ISettingsStoreService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _settingsDirectoryPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OvertimeTimer");

    public async Task<SettingsDataStore> LoadAsync(CancellationToken cancellationToken = default)
    {
        var settingsFilePath = GetSettingsFilePath();
        if (!File.Exists(settingsFilePath))
        {
            return new SettingsDataStore();
        }

        await using var stream = new FileStream(
            settingsFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            FileOptions.Asynchronous);

        var settingsDataStore = await JsonSerializer.DeserializeAsync<SettingsDataStore>(
            stream,
            SerializerOptions,
            cancellationToken);

        return settingsDataStore ?? new SettingsDataStore();
    }

    public async Task SaveAsync(SettingsDataStore settingsDataStore, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_settingsDirectoryPath);

        var settingsFilePath = GetSettingsFilePath();
        var temporaryFilePath = $"{settingsFilePath}.tmp";

        await using (var stream = new FileStream(
                         temporaryFilePath,
                         FileMode.Create,
                         FileAccess.Write,
                         FileShare.None,
                         4096,
                         FileOptions.Asynchronous))
        {
            await JsonSerializer.SerializeAsync(stream, settingsDataStore, SerializerOptions, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }

        if (File.Exists(settingsFilePath))
        {
            File.Replace(temporaryFilePath, settingsFilePath, null);
            return;
        }

        File.Move(temporaryFilePath, settingsFilePath);
    }

    private string GetSettingsFilePath() => Path.Combine(_settingsDirectoryPath, "settings.json");
}
