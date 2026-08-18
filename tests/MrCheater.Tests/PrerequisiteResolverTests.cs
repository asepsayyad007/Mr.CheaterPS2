using MrCheater.Core.Services;
using MrCheater.Domain.Models;
using MrCheater.Infrastructure.Storage;
using Xunit;

namespace MrCheater.Tests;

public class PrerequisiteResolverTests
{
    [Fact]
    public void Resolve_WithoutPrerequisites_ReturnsOnlyTarget()
    {
        var resolver = new PrerequisiteResolver();
        var profile = SeedData.CreateDownhillDominationProfile();
        var master = profile.Cheats.First(c => c.Id == "master_code");

        var chain = resolver.ResolveExecutionChain(master, profile.Cheats);

        Assert.Single(chain);
        Assert.Equal("master_code", chain[0].Id);
    }

    [Fact]
    public void Resolve_WithMasterCodePrerequisite_ReturnsMasterCodeFirst()
    {
        var resolver = new PrerequisiteResolver();
        var profile = SeedData.CreateDownhillDominationProfile();
        var unlock = profile.Cheats.First(c => c.Id == "unlock_everything");

        var chain = resolver.ResolveExecutionChain(unlock, profile.Cheats);

        Assert.Equal(2, chain.Count);
        Assert.Equal("master_code", chain[0].Id);
        Assert.Equal("unlock_everything", chain[1].Id);
    }

    [Fact]
    public void Resolve_WithCyclicDependency_DoesNotInfiniteLoop()
    {
        var resolver = new PrerequisiteResolver();

        var cheatA = new CheatDefinition
        {
            Id = "cheat_a",
            Name = "Cheat A",
            PrerequisiteCheatIds = new List<string> { "cheat_b" }
        };

        var cheatB = new CheatDefinition
        {
            Id = "cheat_b",
            Name = "Cheat B",
            PrerequisiteCheatIds = new List<string> { "cheat_a" }
        };

        var list = new List<CheatDefinition> { cheatA, cheatB };

        var chain = resolver.ResolveExecutionChain(cheatA, list);

        Assert.Equal(2, chain.Count);
    }
}
