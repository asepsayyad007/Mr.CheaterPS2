using MrCheater.Domain.Models;

namespace MrCheater.Infrastructure.Storage;

public static class SeedData
{
    public static List<GameProfile> CreateDefaultProfiles()
    {
        return new List<GameProfile>
        {
            CreateDownhillDominationProfile(),
            CreateGtaSanAndreasProfile(),
            CreateGodOfWarProfile()
        };
    }

    public static GameProfile CreateDownhillDominationProfile()
    {
        var profile = new GameProfile
        {
            Id = "downhill-domination-ps2",
            Name = "Downhill Domination",
            GameCode = "SLUS-20516",
            EmulatorName = "PCSX2",
            TargetProcess = "pcsx2-qt.exe",
            WindowTitlePattern = "PCSX2",
            InputProfileName = "PCSX2 Standard Keyboard Mapping",
            InputMapping = InputMappingProfile.CreateDefaultPs2Mapping(),
            DefaultStepDelayMs = 75,
            DefaultHoldDurationMs = 75,
            Notes = "Complete Downhill Domination (PS2) cheat code library with Master Code prerequisite support."
        };

        CheatDefinition CreateCheat(
            string id,
            string name,
            string desc,
            string category,
            string[] steps,
            bool requiresMaster = true,
            string? hotkey = null,
            bool isFav = false)
        {
            return new CheatDefinition
            {
                Id = id,
                Name = name,
                Description = desc,
                Category = category,
                RequiresMasterCode = requiresMaster,
                PrerequisiteCheatIds = requiresMaster && id != "master_code" ? new List<string> { "master_code" } : new List<string>(),
                DefaultDelayMs = 75,
                AssignedHotkey = hotkey,
                IsFavorite = isFav,
                Sequence = steps.Select(s => new SequenceStep
                {
                    ActionName = s.ToUpperInvariant(),
                    HoldDurationMs = 75,
                    DelayAfterMs = 75
                }).ToList()
            };
        }

        // 1. Master Code (System Prerequisite)
        profile.Cheats.Add(CreateCheat(
            "master_code",
            "Master Code (Unlock Cheat Engine)",
            "Required before activating other codes during a race or in menus. Plays a sound when unlocked.",
            "System",
            new[] { "UP", "TRIANGLE", "DOWN", "CROSS", "LEFT", "CIRCLE", "RIGHT", "SQUARE" },
            requiresMaster: false,
            hotkey: "F1",
            isFav: true));

        // 2. Unlock Everything
        profile.Cheats.Add(CreateCheat(
            "unlock_everything",
            "Unlock Everything",
            "Unlocks all riders, bikes, special upgrades, and all tracks instantaneously.",
            "Unlocks",
            new[] { "DOWN", "UP", "UP", "DOWN", "DOWN", "UP", "UP" },
            requiresMaster: true,
            hotkey: "F2",
            isFav: true));

        // 3. Adrenaline Boost (Gauge Refill)
        profile.Cheats.Add(CreateCheat(
            "adrenaline_boost",
            "Adrenaline Boost",
            "Instantly fills and maxes out your adrenaline speed boost gauge.",
            "Energy & Boost",
            new[] { "DOWN", "LEFT", "LEFT", "RIGHT" },
            requiresMaster: true,
            hotkey: "F4",
            isFav: true));

        // 4. Infinite Energy / Unlimited Stamina
        profile.Cheats.Add(CreateCheat(
            "infinite_energy",
            "Infinite Energy (No Fatigue)",
            "Rider never runs out of stamina and can pedal at maximum power indefinitely.",
            "Energy & Boost",
            new[] { "DOWN", "TRIANGLE", "LEFT", "LEFT", "SQUARE" },
            requiresMaster: true,
            hotkey: "F5",
            isFav: true));

        // 5. Always Stoked (Permanent Boost)
        profile.Cheats.Add(CreateCheat(
            "always_stoked",
            "Always Stoked (Unlimited Boost)",
            "Locks your adrenaline boost at maximum so you never lose speed.",
            "Energy & Boost",
            new[] { "DOWN", "SQUARE", "SQUARE", "LEFT", "CIRCLE" },
            requiresMaster: true,
            isFav: true));

        // 6. Restore Energy & Health
        profile.Cheats.Add(CreateCheat(
            "restore_energy",
            "Restore Full Energy",
            "Instantly refills stamina and rider energy during a grueling descent.",
            "Energy & Boost",
            new[] { "DOWN", "RIGHT", "RIGHT", "LEFT", "LEFT" },
            requiresMaster: true));

        // 7. $5,000 Cash
        profile.Cheats.Add(CreateCheat(
            "money_5000",
            "$5,000 Cash",
            "Adds $5,000 to your bankroll for purchasing top-tier bikes and equipment in the shop.",
            "Money",
            new[] { "RIGHT", "UP", "UP", "CIRCLE", "CIRCLE", "SQUARE" },
            requiresMaster: true,
            hotkey: "F3",
            isFav: true));

        // 8. $2,000 Cash
        profile.Cheats.Add(CreateCheat(
            "money_2000",
            "$2,000 Cash",
            "Adds $2,000 cash instantaneously.",
            "Money",
            new[] { "RIGHT", "TRIANGLE", "TRIANGLE", "LEFT" },
            requiresMaster: true));

        // 9. Speed Freak (Max Velocity)
        profile.Cheats.Add(CreateCheat(
            "speed_freak",
            "Speed Freak (Insane Speed)",
            "Removes top speed limiter for blazing downhill velocity.",
            "Physics & Speed",
            new[] { "DOWN", "TRIANGLE", "RIGHT", "RIGHT", "SQUARE" },
            requiresMaster: true));

        // 10. Mega Flip (Rapid Stunt Rotations)
        profile.Cheats.Add(CreateCheat(
            "mega_flip",
            "Mega Flip (Instant Stunts)",
            "Allows hyper-fast flips and spins in mid-air for massive trick points.",
            "Physics & Speed",
            new[] { "RIGHT", "UP", "UP", "RIGHT", "RIGHT", "SQUARE" },
            requiresMaster: true,
            hotkey: "F6"));

        // 11. Super Bounce
        profile.Cheats.Add(CreateCheat(
            "super_bounce",
            "Super Bounce",
            "Springs high into the air upon touching down after any jump.",
            "Physics & Speed",
            new[] { "LEFT", "SQUARE", "CROSS", "UP", "TRIANGLE" },
            requiresMaster: true));

        // 12. Super Bunny Hop
        profile.Cheats.Add(CreateCheat(
            "super_bunny_hop",
            "Super Bunny Hop",
            "Jump triple height over obstacles and gaps from flat ground.",
            "Physics & Speed",
            new[] { "UP", "CROSS", "LEFT", "SQUARE", "UP" },
            requiresMaster: true));

        // 13. Anti-Gravity
        profile.Cheats.Add(CreateCheat(
            "anti_gravity",
            "Anti-Gravity (Low Gravity)",
            "Drastically reduces gravity for massive float time on mountain jumps.",
            "Physics & Speed",
            new[] { "DOWN", "TRIANGLE", "SQUARE", "SQUARE", "UP" },
            requiresMaster: true));

        // 14. Combat Upgrade (Level 2 Attacks)
        profile.Cheats.Add(CreateCheat(
            "combat_upgrade",
            "Combat Upgrade",
            "Upgrades standard attacks to high-damage kicks and punches.",
            "Combat",
            new[] { "UP", "DOWN", "LEFT", "RIGHT", "CROSS" },
            requiresMaster: true));

        // 15. Free Combat (Unlimited Attacks)
        profile.Cheats.Add(CreateCheat(
            "free_combat",
            "Free Combat",
            "Enables weapon attacks and combat moves without depleting stamina.",
            "Combat",
            new[] { "UP", "DOWN", "LEFT", "RIGHT", "SQUARE" },
            requiresMaster: true));

        // 16. Infinite Water Bottles
        profile.Cheats.Add(CreateCheat(
            "infinite_bottles",
            "Infinite Water Bottles",
            "Unlimited supply of throwable water bottles to take down rival racers.",
            "Combat",
            new[] { "UP", "CROSS", "LEFT", "LEFT", "CIRCLE", "CIRCLE" },
            requiresMaster: true));

        // Macro Demo
        profile.Macros.Add(new MacroDefinition
        {
            Id = "macro_triple_cash",
            Name = "Money Surge ($15,000 Turbo)",
            Description = "Automatically executes $5,000 Cash 3 times in a row for an instant $15,000 bankroll.",
            LoopCount = 3,
            Sequence = new List<SequenceStep>
            {
                new() { ActionName = "RIGHT", HoldDurationMs = 70, DelayAfterMs = 70 },
                new() { ActionName = "UP", HoldDurationMs = 70, DelayAfterMs = 70 },
                new() { ActionName = "UP", HoldDurationMs = 70, DelayAfterMs = 70 },
                new() { ActionName = "CIRCLE", HoldDurationMs = 70, DelayAfterMs = 70 },
                new() { ActionName = "CIRCLE", HoldDurationMs = 70, DelayAfterMs = 70 },
                new() { ActionName = "SQUARE", HoldDurationMs = 70, DelayAfterMs = 250 }
            }
        });

        return profile;
    }

    public static GameProfile CreateGtaSanAndreasProfile()
    {
        var profile = new GameProfile
        {
            Id = "gta-san-andreas-ps2",
            Name = "Grand Theft Auto: San Andreas",
            GameCode = "SLUS-20946",
            EmulatorName = "PCSX2",
            TargetProcess = "pcsx2-qt.exe",
            WindowTitlePattern = "PCSX2",
            InputProfileName = "PCSX2 Standard Keyboard Mapping",
            InputMapping = InputMappingProfile.CreateDefaultPs2Mapping(),
            DefaultStepDelayMs = 80,
            DefaultHoldDurationMs = 80,
            Notes = "Classic GTA San Andreas PS2 cheat codes for PCSX2."
        };

        CheatDefinition CreateGtaCheat(string id, string name, string desc, string category, string[] steps, string? hotkey = null)
        {
            return new CheatDefinition
            {
                Id = id,
                Name = name,
                Description = desc,
                Category = category,
                RequiresMasterCode = false,
                DefaultDelayMs = 80,
                AssignedHotkey = hotkey,
                Sequence = steps.Select(s => new SequenceStep
                {
                    ActionName = s.ToUpperInvariant(),
                    HoldDurationMs = 80,
                    DelayAfterMs = 80
                }).ToList()
            };
        }

        profile.Cheats.Add(CreateGtaCheat(
            "gta_health_armor_money",
            "Health, Armor & $250,000",
            "Full health, body armor, and grants $250,000 cash.",
            "Player",
            new[] { "R1", "R2", "L1", "CROSS", "LEFT", "DOWN", "RIGHT", "UP", "LEFT", "DOWN", "RIGHT", "UP" },
            hotkey: "F1"));

        profile.Cheats.Add(CreateGtaCheat(
            "gta_weapons_tier1",
            "Weapon Tier 1 (Bat, Pistol, Shotgun, AK)",
            "Brass Knuckles, Bat, 9mm, Shotgun, Micro SMG, AK-47, Rifle, Rocket Launcher, Molotovs.",
            "Weapons",
            new[] { "R1", "R2", "L1", "R2", "LEFT", "DOWN", "RIGHT", "UP", "LEFT", "DOWN", "RIGHT", "UP" },
            hotkey: "F2"));

        profile.Cheats.Add(CreateGtaCheat(
            "gta_jetpack",
            "Spawn Jetpack",
            "Spawns the Rocketman Jetpack on CJ.",
            "Spawns",
            new[] { "LEFT", "RIGHT", "L1", "L2", "R1", "R2", "UP", "DOWN", "LEFT", "RIGHT" },
            hotkey: "F3"));

        profile.Cheats.Add(CreateGtaCheat(
            "gta_lower_wanted",
            "Clear Wanted Level",
            "Removes all police attention and wanted stars.",
            "Police",
            new[] { "R1", "R1", "CIRCLE", "R2", "UP", "DOWN", "UP", "DOWN", "UP", "DOWN" },
            hotkey: "F4"));

        profile.Cheats.Add(CreateGtaCheat(
            "gta_infinite_ammo",
            "Infinite Ammo",
            "Unlimited ammunition without needing to reload.",
            "Weapons",
            new[] { "L1", "R1", "SQUARE", "R1", "LEFT", "R2", "R1", "LEFT", "SQUARE", "DOWN", "L1", "L1" }));

        profile.Cheats.Add(CreateGtaCheat(
            "gta_spawn_tank",
            "Spawn Rhino Tank",
            "Spawns a heavy military Rhino tank right in front of CJ.",
            "Spawns",
            new[] { "CIRCLE", "CIRCLE", "L1", "CIRCLE", "CIRCLE", "CIRCLE", "L1", "L2", "R1", "TRIANGLE", "CIRCLE", "TRIANGLE" }));

        return profile;
    }

    public static GameProfile CreateGodOfWarProfile()
    {
        var profile = new GameProfile
        {
            Id = "god-of-war-ps2",
            Name = "God of War",
            GameCode = "SCUS-97399",
            EmulatorName = "PCSX2",
            TargetProcess = "pcsx2-qt.exe",
            WindowTitlePattern = "PCSX2",
            InputProfileName = "PCSX2 Standard Keyboard Mapping",
            InputMapping = InputMappingProfile.CreateDefaultPs2Mapping(),
            DefaultStepDelayMs = 80,
            DefaultHoldDurationMs = 80,
            Notes = "God of War (PS2) cheat codes and combat shortcuts."
        };

        CheatDefinition CreateGowCheat(string id, string name, string desc, string[] steps)
        {
            return new CheatDefinition
            {
                Id = id,
                Name = name,
                Description = desc,
                Category = "Combat",
                RequiresMasterCode = false,
                DefaultDelayMs = 80,
                Sequence = steps.Select(s => new SequenceStep
                {
                    ActionName = s.ToUpperInvariant(),
                    HoldDurationMs = 80,
                    DelayAfterMs = 80
                }).ToList()
            };
        }

        profile.Cheats.Add(CreateGowCheat(
            "gow_poseidon_rage",
            "Poseidon's Rage Combo",
            "Instant combo execution for Poseidon's Rage magic.",
            new[] { "L2", "CIRCLE", "CIRCLE", "CIRCLE" }));

        profile.Cheats.Add(CreateGowCheat(
            "gow_blade_fury",
            "Cyclone of Chaos",
            "Fast 360-degree Blade spin combo.",
            new[] { "L1", "SQUARE", "SQUARE", "SQUARE" }));

        return profile;
    }
}
