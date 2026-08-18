using MrCheater.Domain.Models;

namespace MrCheater.Domain.Interfaces;

public interface ISettingsRepository
{
    Task<AppSettings> LoadSettingsAsync();
    Task SaveSettingsAsync(AppSettings settings);
}

public interface IEmulatorRepository
{
    Task<List<EmulatorProfile>> GetAllEmulatorsAsync();
    Task SaveEmulatorAsync(EmulatorProfile emulator);
}
