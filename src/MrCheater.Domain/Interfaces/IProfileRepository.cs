using MrCheater.Domain.Models;

namespace MrCheater.Domain.Interfaces;

public interface IProfileRepository
{
    Task<List<GameProfile>> GetAllProfilesAsync();
    Task<GameProfile?> GetProfileByIdAsync(string id);
    Task SaveProfileAsync(GameProfile profile);
    Task DeleteProfileAsync(string id);
    Task<GameProfile> ImportProfileFromJsonAsync(string jsonContent);
    string ExportProfileToJson(GameProfile profile);
    Task<string> ExportAllProfilesToJsonAsync();
    Task<List<GameProfile>> ImportProfilesBundleAsync(string jsonContent);
}
