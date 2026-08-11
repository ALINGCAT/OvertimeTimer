using System.IO;
using System.Text.Json;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public sealed class SettingsStoreService : ISettingsStoreService
{
    private static readonly JsonSerializerOptions Opts = new() { PropertyNameCaseInsensitive = true, WriteIndented = true };
    private readonly string _baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OvertimeTimer");

    private string WsPath => Path.Combine(_baseDir, "work-schedule.json");
    private string ApPath => Path.Combine(_baseDir, "appearance.json");
    private string StPath => Path.Combine(_baseDir, "storage.json");
    private string GnPath => Path.Combine(_baseDir, "general.json");

    public async Task<WorkScheduleConfig> LoadWorkScheduleAsync(CancellationToken ct = default)
        => await LoadAsync<WorkScheduleConfig>(WsPath, ct) ?? new();

    public async Task SaveWorkScheduleAsync(WorkScheduleConfig config, CancellationToken ct = default)
        => await SaveAsync(WsPath, config, ct);

    public async Task<AppearanceDataStore> LoadAppearanceAsync(CancellationToken ct = default)
        => await LoadAsync<AppearanceDataStore>(ApPath, ct) ?? new();

    public async Task SaveAppearanceAsync(AppearanceDataStore data, CancellationToken ct = default)
        => await SaveAsync(ApPath, data, ct);

    public async Task<StorageDataStore> LoadStorageAsync(CancellationToken ct = default)
        => await LoadAsync<StorageDataStore>(StPath, ct) ?? new();

    public async Task SaveStorageAsync(StorageDataStore data, CancellationToken ct = default)
        => await SaveAsync(StPath, data, ct);

    public async Task<GeneralConfig> LoadGeneralAsync(CancellationToken ct = default)
        => await LoadAsync<GeneralConfig>(GnPath, ct) ?? new();

    public async Task SaveGeneralAsync(GeneralConfig config, CancellationToken ct = default)
        => await SaveAsync(GnPath, config, ct);

    private static async Task<T?> LoadAsync<T>(string path, CancellationToken ct) where T : class
    {
        if (!File.Exists(path)) return null;
        var json = await File.ReadAllTextAsync(path, ct);
        return JsonSerializer.Deserialize<T>(json, Opts);
    }

    private static async Task SaveAsync<T>(string path, T data, CancellationToken ct)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var tmp = path + ".tmp";
        await File.WriteAllTextAsync(tmp, JsonSerializer.Serialize(data, Opts), ct);
        if (File.Exists(path)) File.Replace(tmp, path, null);
        else File.Move(tmp, path);
    }
}
