using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Models;

public sealed class StorageDataStore
{
    public DiaryStorageConfig DiaryConfig { get; set; } = new();
    public string LanguageCode { get; set; } = "zh-CN";
}
