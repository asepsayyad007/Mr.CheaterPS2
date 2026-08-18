using System.Text.Json;
using MrCheater.Core.Common;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;

namespace MrCheater.Infrastructure.Storage;

public class JsonSettingsRepository : ISettingsRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private AppSettings? _cachedSettings;

    public async Task<AppSettings> LoadSettingsAsync()
    {
        if (_cachedSettings != null)
            return _cachedSettings;

        var path = StorageHelper.GetSettingsFilePath();
        if (File.Exists(path))
        {
            try
            {
                var json = await File.ReadAllTextAsync(path);
                var settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions);
                if (settings != null)
                {
                    _cachedSettings = settings;
                    return settings;
                }
            }
            catch
            {
                // Fallback to default
            }
        }

        var defaultSettings = new AppSettings();
        await SaveSettingsAsync(defaultSettings);
        _cachedSettings = defaultSettings;
        return defaultSettings;
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        _cachedSettings = settings;
        var path = StorageHelper.GetSettingsFilePath();
        var json = JsonSerializer.Serialize(settings, JsonOptions);
        await File.WriteAllTextAsync(path, json);
    }
}

public class JsonEmulatorRepository : IEmulatorRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<EmulatorProfile>> GetAllEmulatorsAsync()
    {
        var dir = StorageHelper.GetEmulatorsDirectory();
        var files = Directory.GetFiles(dir, "*.json");

        if (files.Length == 0)
        {
            var defaults = EmulatorProfile.CreateDefaultEmulators();
            foreach (var emu in defaults)
            {
                await SaveEmulatorAsync(emu);
            }
            return defaults;
        }

        var list = new List<EmulatorProfile>();
        foreach (var file in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var emu = JsonSerializer.Deserialize<EmulatorProfile>(json, JsonOptions);
                if (emu != null)
                {
                    list.Add(emu);
                }
            }
            catch
            {
                // Skip corrupted
            }
        }

        return list;
    }

    public async Task SaveEmulatorAsync(EmulatorProfile emulator)
    {
        if (emulator == null)
            throw new ArgumentNullException(nameof(emulator));

        var dir = StorageHelper.GetEmulatorsDirectory();
        var safeFileName = MakeValidFileName(emulator.Name) + ".json";
        var path = Path.Combine(dir, safeFileName);

        var json = JsonSerializer.Serialize(emulator, JsonOptions);
        await File.WriteAllTextAsync(path, json);
    }

    private static string MakeValidFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }
}
