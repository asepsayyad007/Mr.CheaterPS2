# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.0] - 2026-08-18

### Added
* **Universal Cheat and Macro Architecture**:
  * 5-project clean .NET 8 architecture (`MrCheater.Domain`, `MrCheater.Infrastructure`, `MrCheater.Core`, `MrCheater.UI`, `MrCheater.Tests`).
* **Frame-Accurate Hardware Scan-Code Injection**:
  * Win32 `SendInput` keyboard simulation with direct hardware scancodes (`KEYEVENTF_SCANCODE`) for 100% compatibility with PCSX2, RetroArch, RPCS3, Dolphin, and DirectInput/RawInput games.
* **Smart Master Code Prerequisite Engine**:
  * Prerequisite resolver with cycle detection and session-state memory that remembers when a Master Code is active, preventing accidental toggle-off lockouts.
* **Target Process Auto-Focus Switcher**:
  * Process monitor with multi-window enumeration (`EnumWindows`, `SetForegroundWindow`, `BringWindowToTop`, `AttachThreadInput`) ensuring target game windows receive input seamlessly upon in-app execution.
* **Global Hotkey Daemon**:
  * Low-level Windows hotkey registry (`RegisterHotKey`) for triggering cheats directly in-game (`F1`-`F12`, modifiers).
* **Zero-Latency Emergency Stop Watchdog**:
  * System-wide `Ctrl + Shift + Escape` hook that instantly aborts active sequences, clears queues, and releases all held hardware keys.
* **Real-Time XInput Controller Visualizer**:
  * Sub-millisecond polling service for Xbox and DirectInput gamepads with live button lighting and stick axis telemetry.
* **Visual Sequence Editor and Macro Engine**:
  * Interactive UI for crafting custom button sequences, configuring hold/delay timings, and defining multi-loop combo macros.
* **Curated Profile Library**:
  * Complete Downhill Domination (PS2) cheat collection (16 verified codes + Master Code prerequisite chain + cash turbo macro).
  * GTA San Andreas (PS2) profile with weapons, armor, jetpack, and wanted level cheats.
  * God of War (PS2) combat combo macro profile.
* **Full Test Suite**:
  * 17 automated xUnit unit and integration tests covering prerequisite resolution, deep cloning, JSON roundtripping, emergency release, and live emulator execution.

### Changed
* Standardized default PS2 keyboard layout to match official PCSX2 Qt keyboard mapping (`Triangle: I`, `Circle: L`, `Cross: K`, `Square: J`, `D-Pad: Arrows`).
* Upgraded UI dispatcher communication to non-blocking `BeginInvoke` queues, eliminating UI thread contention.

---

[1.0.0]: https://github.com/asepsayyad007/Mr.CheaterPS2/releases/tag/v1.0.0
