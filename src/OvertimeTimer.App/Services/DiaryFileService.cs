using System.IO;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public sealed class DiaryFileService : IDiaryFileService
{
    private readonly string _defaultRootPath = Path.Combine(AppContext.BaseDirectory, "dailies");
    private DiaryStorageConfig _config = new();

    public void Configure(DiaryStorageConfig config)
    {
        _config = new DiaryStorageConfig
        {
            RootPath = string.IsNullOrWhiteSpace(config.RootPath)
                ? _defaultRootPath
                : config.RootPath,
            Mode = config.Mode
        };
    }

    public string GetDiaryFilePath(DateOnly date)
    {
        var rootPath = _config.RootPath;
        var fileName = $"{date:yyyy-MM-dd}.md";

        return _config.Mode switch
        {
            DiaryStorageMode.ByYear => Path.Combine(rootPath, date.Year.ToString(), fileName),
            DiaryStorageMode.ByMonth => Path.Combine(rootPath, $"{date:yyyy-MM}", fileName),
            _ => Path.Combine(rootPath, fileName)
        };
    }

    public async Task<string> LoadDiaryAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var filePath = GetDiaryFilePath(date);
        if (!File.Exists(filePath))
        {
            return string.Empty;
        }

        return await File.ReadAllTextAsync(filePath, cancellationToken);
    }

    public async Task SaveDiaryAsync(DateOnly date, string markdown, CancellationToken cancellationToken = default)
    {
        var filePath = GetDiaryFilePath(date);
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(filePath, markdown, cancellationToken);
    }

    public Task<bool> ExistsAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var filePath = GetDiaryFilePath(date);
        return Task.FromResult(File.Exists(filePath));
    }

    public Task DeleteDiaryAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var filePath = GetDiaryFilePath(date);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
