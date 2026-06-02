namespace OvertimeTimer.Core.Models;

public sealed class DiaryStorageConfig
{
    public string RootPath { get; set; } = string.Empty;

    public DiaryStorageMode Mode { get; set; } = DiaryStorageMode.Flat;
}
