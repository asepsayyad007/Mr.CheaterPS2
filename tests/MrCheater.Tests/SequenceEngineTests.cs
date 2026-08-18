using MrCheater.Core.Services;
using MrCheater.Domain.Enums;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;
using MrCheater.Infrastructure.Input;
using MrCheater.Infrastructure.Storage;
using Xunit;

namespace MrCheater.Tests;

public class DummyProcessMonitor : IProcessMonitor
{
    public TargetProcessStatus CurrentStatus { get; set; } = TargetProcessStatus.Focused;
    public event EventHandler<TargetProcessStatus>? StatusChanged;

    public TargetProcessStatus GetProcessStatus(IEnumerable<string> processNames, string? windowTitlePattern = null) => CurrentStatus;
    public bool IsProcessFocused(IEnumerable<string> processNames) => CurrentStatus == TargetProcessStatus.Focused;
    public bool TryFocusTargetProcess(string processName) => true;
    public void SetWatchedTargets(IEnumerable<string> processNames, string? windowTitlePattern = null) { }
    public void Start() { }
    public void Stop() { }
    public void Dispose() { }
}

public class MemorySettingsRepository : ISettingsRepository
{
    public AppSettings Settings { get; set; } = new()
    {
        RequireTargetProcessRunning = false,
        RequireTargetWindowFocus = false,
        MaxSequenceDurationSeconds = 10
    };

    public Task<AppSettings> LoadSettingsAsync() => Task.FromResult(Settings);
    public Task SaveSettingsAsync(AppSettings settings)
    {
        Settings = settings;
        return Task.CompletedTask;
    }
}

public class SequenceEngineTests
{
    [Fact]
    public async Task ExecuteCheat_SendsCorrectKeySequence()
    {
        // Arrange
        var mockInput = new MockInputProvider { SimulateDelays = false };
        var processMonitor = new DummyProcessMonitor();
        var settingsRepo = new MemorySettingsRepository();
        var prereqResolver = new PrerequisiteResolver();
        var logger = new AppLogger();
        var emergencyStop = new EmergencyStopManager(mockInput, logger);

        var engine = new SequenceEngine(mockInput, processMonitor, settingsRepo, prereqResolver, emergencyStop, logger);

        var profile = SeedData.CreateDownhillDominationProfile();
        var cheat = profile.Cheats.First(c => c.Id == "master_code");

        // Act
        var result = await engine.ExecuteCheatAsync(cheat, profile);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(8, result.TotalStepsExecuted);
        Assert.Equal(new[] { "UP", "TRIANGLE", "DOWN", "CROSS", "LEFT", "CIRCLE", "RIGHT", "SQUARE" }, mockInput.SentActions);
    }

    [Fact]
    public async Task ExecuteCheat_WithPrerequisiteMasterCode_ExecutesMasterCodeFirst()
    {
        // Arrange
        var mockInput = new MockInputProvider { SimulateDelays = false };
        var processMonitor = new DummyProcessMonitor();
        var settingsRepo = new MemorySettingsRepository();
        var prereqResolver = new PrerequisiteResolver();
        var logger = new AppLogger();
        var emergencyStop = new EmergencyStopManager(mockInput, logger);

        var engine = new SequenceEngine(mockInput, processMonitor, settingsRepo, prereqResolver, emergencyStop, logger);

        var profile = SeedData.CreateDownhillDominationProfile();
        var unlockEverything = profile.Cheats.First(c => c.Id == "unlock_everything");

        // Act
        var result = await engine.ExecuteCheatAsync(unlockEverything, profile);

        // Assert
        Assert.True(result.Success);
        // Master code (8) + Unlock everything (7) = 15 steps
        Assert.Equal(15, result.TotalStepsExecuted);

        // Verify master code was sent first
        Assert.Equal("UP", mockInput.SentActions[0]);
        Assert.Equal("TRIANGLE", mockInput.SentActions[1]);
        Assert.Equal("DOWN", mockInput.SentActions[2]);
        Assert.Equal("CROSS", mockInput.SentActions[3]);
        Assert.Equal("LEFT", mockInput.SentActions[4]);
        Assert.Equal("CIRCLE", mockInput.SentActions[5]);
        Assert.Equal("RIGHT", mockInput.SentActions[6]);
        Assert.Equal("SQUARE", mockInput.SentActions[7]);

        // Then Unlock Everything
        Assert.Equal("DOWN", mockInput.SentActions[8]);
        Assert.Equal("UP", mockInput.SentActions[9]);
        Assert.Equal("UP", mockInput.SentActions[10]);
        Assert.Equal("DOWN", mockInput.SentActions[11]);
        Assert.Equal("DOWN", mockInput.SentActions[12]);
        Assert.Equal("UP", mockInput.SentActions[13]);
        Assert.Equal("UP", mockInput.SentActions[14]);
    }

    [Fact]
    public async Task EmergencyStop_AbortsExecutionAndReleasesKeys()
    {
        // Arrange
        var mockInput = new MockInputProvider { SimulateDelays = true };
        var processMonitor = new DummyProcessMonitor();
        var settingsRepo = new MemorySettingsRepository();
        var prereqResolver = new PrerequisiteResolver();
        var logger = new AppLogger();
        var emergencyStop = new EmergencyStopManager(mockInput, logger);

        var engine = new SequenceEngine(mockInput, processMonitor, settingsRepo, prereqResolver, emergencyStop, logger);

        var profile = SeedData.CreateDownhillDominationProfile();
        var cheat = profile.Cheats.First(c => c.Id == "unlock_everything");

        // Act
        var execTask = Task.Run(() => engine.ExecuteCheatAsync(cheat, profile));

        await Task.Delay(50); // let it start
        emergencyStop.TriggerEmergencyStop();

        var result = await execTask;

        // Assert
        Assert.False(result.Success);
        Assert.True(result.WasCancelled);
        Assert.True(mockInput.IsAllReleasedCalled);
    }

    [Fact]
    public async Task ExecuteCheat_WhenMasterCodeAlreadyActive_DoesNotRepeatMasterCode()
    {
        // Arrange
        var mockInput = new MockInputProvider { SimulateDelays = false };
        var processMonitor = new DummyProcessMonitor();
        var settingsRepo = new MemorySettingsRepository();
        var prereqResolver = new PrerequisiteResolver();
        var logger = new AppLogger();
        var emergencyStop = new EmergencyStopManager(mockInput, logger);

        var engine = new SequenceEngine(mockInput, processMonitor, settingsRepo, prereqResolver, emergencyStop, logger);

        var profile = SeedData.CreateDownhillDominationProfile();
        var alwaysStoked = profile.Cheats.First(c => c.Id == "always_stoked");

        // Mark Master Code as ALREADY active in session
        engine.SetMasterCodeActive(profile.Id, true);

        // Act
        var result = await engine.ExecuteCheatAsync(alwaysStoked, profile);

        // Assert
        Assert.True(result.Success);
        // Should only have executed the 5 steps of always_stoked (DOWN, SQUARE, SQUARE, LEFT, CIRCLE)
        Assert.Equal(5, result.TotalStepsExecuted);
        Assert.Equal(5, mockInput.SentActions.Count);
        Assert.Equal("DOWN", mockInput.SentActions[0]);
        Assert.Equal("SQUARE", mockInput.SentActions[1]);
        Assert.Equal("SQUARE", mockInput.SentActions[2]);
        Assert.Equal("LEFT", mockInput.SentActions[3]);
        Assert.Equal("CIRCLE", mockInput.SentActions[4]);
    }
}
