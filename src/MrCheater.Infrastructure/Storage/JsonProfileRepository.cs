using System.Text.Json;
using MrCheater.Core.Common;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;

namespace MrCheater.Infrastructure.Storage;

public class JsonProfileRepository : IProfileRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<GameProfile>> GetAllProfilesAsync()
    {
        var dir = StorageHelper.GetProfilesDirectory();
        var files = Directory.GetFiles(dir, "*.json");

        var list = new List<GameProfile>();

        if (files.Length == 0)
        {
            // Seed defaults
            var defaults = SeedData.CreateDefaultProfiles();
            foreach (var p in defaults)
            {
                await SaveProfileAsync(p);
            }
            return defaults;
        }

        foreach (var file in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var profile = JsonSerializer.Deserialize<GameProfile>(json, JsonOptions);
                if (profile != null)
                {
                    list.Add(profile);
                }
            }
            catch
            {
                // Skip or log corrupted profile
            }
        }

        return list;
    }

    public async Task<GameProfile?> GetProfileByIdAsync(string id)
    {
        var profiles = await GetAllProfilesAsync();
        return profiles.FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    }

    public async Task SaveProfileAsync(GameProfile profile)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));

        var dir = StorageHelper.GetProfilesDirectory();
        var safeFileName = MakeValidFileName(profile.Id) + ".json";
        var fullPath = Path.Combine(dir, safeFileName);

        var json = JsonSerializer.Serialize(profile, JsonOptions);
        await File.WriteAllTextAsync(fullPath, json);
    }

    public Task DeleteProfileAsync(string id)
    {
        var dir = StorageHelper.GetProfilesDirectory();
        var safeFileName = MakeValidFileName(id) + ".json";
        var fullPath = Path.Combine(dir, safeFileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<GameProfile> ImportProfileFromJsonAsync(string jsonContent)
    {
        if (string.IsNullOrWhiteSpace(jsonContent))
            throw new ArgumentException("JSON content is empty.", nameof(jsonContent));

        var profile = JsonSerializer.Deserialize<GameProfile>(jsonContent, JsonOptions);
        if (profile == null || string.IsNullOrWhiteSpace(profile.Name))
        {
            throw new InvalidOperationException("Failed to deserialize valid game profile.");
        }

        return Task.FromResult(profile);
    }

    public string ExportProfileToJson(GameProfile profile)
    {
        return JsonSerializer.Serialize(profile, JsonOptions);
    }

    public async Task<string> ExportAllProfilesToJsonAsync()
    {
        var profiles = await GetAllProfilesAsync();
        return JsonSerializer.Serialize(profiles, JsonOptions);
    }

    public async Task<List<GameProfile>> ImportProfilesBundleAsync(string jsonContent)
    {
        if (string.IsNullOrWhiteSpace(jsonContent))
            throw new ArgumentException("JSON content is empty.", nameof(jsonContent));

        // Try bundle array first
        try
        {
            var list = JsonSerializer.Deserialize<List<GameProfile>>(jsonContent, JsonOptions);
            if (list != null && list.Count > 0)
            {
                foreach (var p in list)
                {
                    await SaveProfileAsync(p);
                }
                return list;
            }
        }
        catch
        {
            // Fallback to single profile import
        }

        var single = await ImportProfileFromJsonAsync(jsonContent);
        await SaveProfileAsync(single);
        return new List<GameProfile> { single };
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
