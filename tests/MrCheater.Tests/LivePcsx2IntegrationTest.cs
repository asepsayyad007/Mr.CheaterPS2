using System.Diagnostics;
using MrCheater.Core.Services;
using MrCheater.Domain.Models;
using MrCheater.Infrastructure.Input;
using MrCheater.Infrastructure.Storage;
using MrCheater.Infrastructure.System;
using Xunit;

namespace MrCheater.Tests;

public class LivePcsx2IntegrationTest
{
    [Fact]
    public async Task LiveExecution_WhenPcsx2IsRunning_FocusesAndSendsInputs()
    {
        var pcsx2Procs = Process.GetProcessesByName("pcsx2-qt");
        if (pcsx2Procs.Length == 0)
        {
            // If PCSX2 isn't running in this context, skip live test gracefully
            return;
        }

        var processMonitor = new WindowsProcessMonitor();
        var inputProvider = new WindowsSendInputProvider();
        var settingsRepo = new MemorySettingsRepository();
        var prereqResolver = new PrerequisiteResolver();
        var logger = new AppLogger();
        var emergencyStop = new EmergencyStopManager(inputProvider, logger);

        var engine = new SequenceEngine(
            inputProvider,
            processMonitor,
            settingsRepo,
            prereqResolver,
            emergencyStop,
            logger);

        var profile = SeedData.CreateDownhillDominationProfile();
        var unlockCheat = profile.Cheats.First(c => c.Id == "unlock_everything");

        // Attempt live execution
        var result = await engine.ExecuteCheatAsync(unlockCheat, profile);

        Assert.True(result.Success, $"Execution failed: {result.Message}");
        Assert.Equal(15, result.TotalStepsExecuted);
    }
}
