using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.Services;

public sealed class GeneralSettingsService : IGeneralSettingsService
{
    private readonly ISettingsStoreService _store;

    public GeneralConfig Config { get; private set; } = new();

    public event Action? ConfigChanged;

    public GeneralSettingsService(ISettingsStoreService store) { _store = store; }

    public void Load()
    {
        Config = Task.Run(() => _store.LoadGeneralAsync()).Result;
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Config = await _store.LoadGeneralAsync(cancellationToken);
        ConfigChanged?.Invoke();
    }

    public void Apply(GeneralConfig config)
    {
        Config = config;
        ConfigChanged?.Invoke();
    }
}
