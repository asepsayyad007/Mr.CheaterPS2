using MrCheater.Core.Common;
using MrCheater.Domain.Models;
using MrCheater.Infrastructure.Storage;
using Xunit;

namespace MrCheater.Tests;

public class ProfileStorageTests
{
    [Fact]
    public void Profile_Clone_CreatesDeepIndependentCopy()
    {
        var original = SeedData.CreateDownhillDominationProfile();
        var copy = original.Clone();

        Assert.NotEqual(original.Id, copy.Id);
        Assert.Equal(original.Cheats.Count, copy.Cheats.Count);

        // Modifying copy cheats must not affect original
        copy.Cheats[0].Name = "Modified Name";
        Assert.NotEqual(original.Cheats[0].Name, copy.Cheats[0].Name);

        copy.InputMapping.Mappings["UP"].KeyboardKey = "W";
        Assert.NotEqual(original.InputMapping.Mappings["UP"].KeyboardKey, copy.InputMapping.Mappings["UP"].KeyboardKey);
    }

    [Fact]
    public async Task JsonProfileRepository_ExportAndImport_RoundtripsAccurately()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "MrCheaterTests_" + Guid.NewGuid().ToString("N"));
        StorageHelper.SetCustomBaseDirectory(tempDir);

        try
        {
            var repo = new JsonProfileRepository();
            var profile = SeedData.CreateDownhillDominationProfile();

            var json = repo.ExportProfileToJson(profile);
            Assert.Contains("Downhill Domination", json);
            Assert.Contains("SLUS-20516", json);

            var imported = await repo.ImportProfileFromJsonAsync(json);
            Assert.Equal(profile.Name, imported.Name);
            Assert.Equal(profile.Cheats.Count, imported.Cheats.Count);
            Assert.Equal(profile.TargetProcess, imported.TargetProcess);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
