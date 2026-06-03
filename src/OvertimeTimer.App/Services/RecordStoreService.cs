using System.IO;
using System.Text.Json;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public sealed class RecordStoreService : IRecordStoreService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly string _recordsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OvertimeTimer",
        "records.json");

    public async Task<List<DailyRecord>> LoadAllAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_recordsFilePath))
        {
            return new List<DailyRecord>();
        }

        await using var stream = new FileStream(
            _recordsFilePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            FileOptions.Asynchronous);

        var records = await JsonSerializer.DeserializeAsync<List<DailyRecord>>(
            stream,
            SerializerOptions,
            cancellationToken);

        return records ?? new List<DailyRecord>();
    }

    public async Task<DailyRecord?> LoadAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var records = await LoadAllAsync(cancellationToken);
        return records.Find(r => r.Date == date);
    }

    public async Task SaveAsync(DailyRecord record, CancellationToken cancellationToken = default)
    {
        var records = await LoadAllAsync(cancellationToken);

        var existingIndex = records.FindIndex(r => r.Date == record.Date);
        if (existingIndex >= 0)
        {
            records[existingIndex] = record;
        }
        else
        {
            records.Add(record);
        }

        await SaveAllAsync(records, cancellationToken);
    }

    public async Task SaveAllAsync(List<DailyRecord> records, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_recordsFilePath)!);

        var temporaryFilePath = $"{_recordsFilePath}.tmp";

        await using (var stream = new FileStream(
                         temporaryFilePath,
                         FileMode.Create,
                         FileAccess.Write,
                         FileShare.None,
                         4096,
                         FileOptions.Asynchronous))
        {
            await JsonSerializer.SerializeAsync(stream, records, SerializerOptions, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }

        if (File.Exists(_recordsFilePath))
        {
            File.Replace(temporaryFilePath, _recordsFilePath, null);
            return;
        }

        File.Move(temporaryFilePath, _recordsFilePath);
    }
}
