using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public interface IDiaryFileService
{
    void Configure(DiaryStorageConfig config);
    Task<string> LoadDiaryAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task SaveDiaryAsync(DateOnly date, string markdown, CancellationToken cancellationToken = default);
    string GetDiaryFilePath(DateOnly date);
    Task<bool> ExistsAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task DeleteDiaryAsync(DateOnly date, CancellationToken cancellationToken = default);
}
