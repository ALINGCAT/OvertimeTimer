using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public interface IRecordStoreService
{
    Task<List<DailyRecord>> LoadAllAsync(CancellationToken cancellationToken = default);
    Task<DailyRecord?> LoadAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task SaveAsync(DailyRecord record, CancellationToken cancellationToken = default);
    Task SaveAllAsync(List<DailyRecord> records, CancellationToken cancellationToken = default);
}
