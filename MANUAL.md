# Universal Game Cheat & Macro Manager (Mr.Cheater PS2) — User Manual

Welcome to **Universal Game Cheat & Macro Manager** — a professional, high-performance Windows desktop application designed to automate cheat-code button sequences, fighting game combos, controller macros, and rapid input sequences for emulators (**PCSX2, RPCS3, Dolphin, DuckStation, RetroArch, PPSSPP**) and native PC games.

---

## Table of Contents
1. [Core Concepts](#1-core-concepts)
2. [First-Time Launch & Wizard](#2-first-time-launch--wizard)
3. [PCSX2 & Downhill Domination Quickstart](#3-pcsx2--downhill-domination-quickstart)
4. [Downhill Domination Cheat Code Catalogue](#4-downhill-domination-cheat-code-catalogue)
5. [Master Codes & Prerequisite Chains](#5-master-codes--prerequisite-chains)
6. [Visual Sequence Editor Guide](#6-visual-sequence-editor-guide)
7. [XInput Controller Mapping & Live Visualizer](#7-xinput-controller-mapping--live-visualizer)
8. [Macro & Combo Engine](#8-macro--combo-engine)
9. [Global Hotkeys & Emergency Stop](#9-global-hotkeys--emergency-stop)
10. [Game Profiles & Emulators Management](#10-game-profiles--emulators-management)
11. [Importing and Exporting Profiles](#11-importing-and-exporting-profiles)
12. [Safety, Focus Protection & Settings](#12-safety-focus-protection--settings)
13. [Troubleshooting & FAQ](#13-troubleshooting--faq)

---

## 1. Core Concepts

Instead of hard-coding cheats for specific games, **Mr.Cheater** treats all cheats, macros, and button sequences as **data**:

* **Game Profile**: Contains the game name, target emulator/executable, input bindings, cheat collection, and macros.
* **Input Mapping**: Maps abstract console buttons (e.g. `UP`, `DOWN`, `TRIANGLE`, `CIRCLE`, `CROSS`, `SQUARE`, `L1`, `R1`) to concrete keyboard scan codes and XInput gamepad buttons.
* **Sequence**: An ordered list of atomic input steps with configurable hold durations (in milliseconds) and delays between presses.
* **Sequence Engine**: The high-precision execution runner that resolves prerequisites (e.g. automatically typing a Master Code first), verifies emulator focus, and simulates hardware key strokes.

---

## 2. First-Time Launch & Wizard

On first launch, you will be greeted by the **Setup Wizard**:

1. **Controller Detection**: Detects any connected XInput (Xbox standard) controllers automatically.
2. **Default Profile Initialization**: Automatically creates and seeds the complete **Downhill Domination (PS2)** profile with 15+ verified cheats, preconfigured PCSX2 emulator profile, and standard PS2 keyboard mappings.
3. Click **Get Started** to open the main dashboard.

---

## 3. PCSX2 & Downhill Domination Quickstart

### Step 1: Open PCSX2 & Start Downhill Domination
1. Launch PCSX2 (`pcsx2-qt.exe`).
2. Start **Downhill Domination**.
3. ### Default PS2 Keyboard Mapping (100% PCSX2 Layout)

| PS2 Action | PCSX2 Default Key | XInput Controller Equivalent |
|---|---|---|
| **D-Pad Up (↑)** | `Up Arrow` | D-Pad Up |
| **D-Pad Down (↓)** | `Down Arrow` | D-Pad Down |
| **D-Pad Left (←)** | `Left Arrow` | D-Pad Left |
| **D-Pad Right (→)** | `Right Arrow` | D-Pad Right |
| **Triangle (△)** | `I` | `Y` Button |
| **Circle (◯)** | `L` | `B` Button |
| **Cross (✕)** | `K` | `A` Button |
| **Square (▢)** | `J` | `X` Button |
| **L1 Bumper** | `Q` | Left Shoulder (`LB`) |
| **L2 Trigger** | `1` | Left Trigger (`LT`) |
| **R1 Bumper** | `E` | Right Shoulder (`RB`) |
| **R2 Trigger** | `3` | Right Trigger (`RT`) |
| **Select** | `Backspace` | `Back` Button |
| **Start** | `Return` (Enter) | `Start` Button |
| **L3 Click** | `2` | Left Thumb Click |
| **R3 Click** | `4` | Right Thumb Click |
| **Left Stick** | `W` (Up), `S` (Down), `A` (Left), `D` (Right) | Left Analog Stick |
| **Right Stick** | `T` (Up), `G` (Down), `F` (Left), `H` (Right) | Right Analog Stick |

### Step 2: Open Mr.Cheater
1. In Mr.Cheater, ensure **Downhill Domination** is the active game profile in the top header.
2. Observe the top status pill: when PCSX2 is running, it shows `PCSX2 ● Active & Focused`.

### Step 3: Trigger a Cheat
* **Method A (In-App Button)**: Go to **Dashboard** or **Cheats Library**, click **Execute** next to `Unlock Everything`.
* **Method B (Global Hotkey)**: While playing the game in full screen or windowed mode, press **`F2`** (assigned to Unlock Everything) or **`F1`** (Master Code).
* **Automated Result**: Mr.Cheater will automatically type the Master Code sequence, wait 150ms, and type the Unlock Everything sequence directly into PCSX2!

---

## 4. Downhill Domination Cheat Code Catalogue

| Cheat Name | Effect | Sequence | Prerequisite | Hotkey |
| :--- | :--- | :--- | :--- | :--- |
| **Master Code** | Unlocks cheat engine | `UP, TRIANGLE, DOWN, CROSS, LEFT, CIRCLE, RIGHT, SQUARE` | None | `F1` |
| **Unlock Everything** | All riders, bikes, upgrades & tracks | `DOWN, UP, UP, DOWN, DOWN, UP, UP` | Master Code | `F2` |
| **$5,000 Cash** | Adds $5,000 money | `RIGHT, UP, UP, CIRCLE, CIRCLE, SQUARE` | Master Code | `F3` |
| **$2,000 Cash** | Adds $2,000 money | `RIGHT, TRIANGLE, TRIANGLE, LEFT` | Master Code | None |
| **Adrenaline Boost** | Max adrenaline boost gauge | `DOWN, LEFT, LEFT, RIGHT` | Master Code | `F4` |
| **Always Stoked** | Unlimited combat & boost energy | `DOWN, SQUARE, SQUARE, LEFT, CIRCLE` | Master Code | `F5` |
| **Mega Flip** | Extreme rotational flip speed | `RIGHT, UP, UP, RIGHT, RIGHT, SQUARE` | Master Code | `F6` |
| **Super Bounce** | Super high ground bounce | `LEFT, SQUARE, CROSS, UP, TRIANGLE` | Master Code | None |
| **Super Bunny Hop** | Super jump height | `UP, CROSS, LEFT, SQUARE, UP` | Master Code | None |
| **Anti-Gravity** | Low gravity physics | `DOWN, TRIANGLE, SQUARE, SQUARE, UP` | Master Code | None |
| **Infinite Bottles** | Unlimited water bottles to throw | `UP, CROSS, LEFT, LEFT, CIRCLE, CIRCLE` | Master Code | None |
| **Unlimited Energy** | Rider never gets exhausted | `DOWN, RIGHT, RIGHT, LEFT, LEFT` | Master Code | None |
| **Combat Upgrade** | Heavy punch/kick tier upgrade | `UP, DOWN, LEFT, RIGHT, SQUARE` | Master Code | None |
| **Speed Freak** | Uncapped downhill speed | `DOWN, TRIANGLE, RIGHT, RIGHT, SQUARE` | Master Code | None |

---

## 5. Master Codes & Prerequisite Chains

Many PlayStation 2 games require entering a "Master Code" before subsequent cheats can be recognized.

In Mr.Cheater:
1. When configuring any cheat in the **Sequence Editor**, check **Requires Master Code**.
2. When executing that cheat, Mr.Cheater dynamically resolves the dependency chain and automatically executes the Master Code first, pauses for a clean gap (150ms), and then executes your requested cheat.
3. You never need to manually enter the Master Code every time!

---

## 6. Visual Sequence Editor Guide

To create or edit any cheat sequence:

1. Click **Sequence Editor** on the sidebar.
2. Enter the **Cheat Name**, **Category**, and optional **Hotkey** (e.g. `F4` or `Ctrl+F1`).
3. Click buttons on the **Input Palette** (`↑ UP`, `↓ DOWN`, `△ Triangle`, `◯ Circle`, etc.) to append steps to your sequence.
4. **Step Options**:
   - **Hold Duration (ms)**: How long the virtual key is pressed down (default 70ms).
   - **Delay (ms)**: Wait duration before the next button press (default 70ms).
5. **Reorder & Duplicate**: Use `↑ Move Up`, `↓ Move Down`, and `Duplicate Step` to quickly assemble complex codes.
6. **Live Recording**: Click `⏺ Record Live Controller Inputs`, tap buttons on your gamepad/keyboard, and click `Stop Recording`.
7. **Simulate Run**: Click **Simulate Run** to test the timing and logic inside Mr.Cheater without sending keys to the game.
8. Click **Save Cheat** to save into the active profile.

---

## 7. XInput Controller Mapping & Live Visualizer

1. Navigate to **Controllers & Keys**.
2. **Live Visualizer**: When you connect an Xbox/XInput gamepad, the buttons on the visual gamepad illuminate in real-time as you press them.
3. **Key Mappings**: Customize the keyboard scan code and controller button mapped to each console action.
4. Click **Save Mappings** or **Reset to Default PS2** at any time.

---

## 8. Macro & Combo Engine

The Macro Engine allows you to automate combo attacks and repeating input loops:

1. Navigate to **Macro Engine**.
2. Click **+ Add New Macro**.
3. Set the **Loop Count** (e.g., `1x` for a combo, or `5x` for a repeating cycle).
4. Configure your step sequence with fine-grained delays (e.g., 50ms hold, 100ms delay between hits).
5. Assign a global hotkey or click **Run Macro** to execute.

---

## 9. Global Hotkeys & Emergency Stop

### Global Hotkeys
You can assign hotkeys (e.g., `F1`, `F2`, `Ctrl+F1`, `Alt+F3`) to any cheat or macro.
When playing in PCSX2 or any emulator, pressing the hotkey triggers the sequence in the background without needing to switch windows.

### Emergency Stop
* Default Hotkey: **`Ctrl + Shift + Escape`** (or click the red Emergency Stop button in the top header).
* When triggered:
  - Instantly aborts all active sequences and background tasks.
  - Releases all held down virtual keys immediately.
  - Displays an **Emergency Stop** notification.

---

## 10. Game Profiles & Emulators Management

Mr.Cheater supports unlimited game profiles and emulators:

1. Navigate to **Game Profiles**.
2. Click **+ Create Profile**.
3. Set:
   - **Game Name**: e.g., `Gran Turismo 4`
   - **Emulator**: `PCSX2`
   - **Target Process**: `pcsx2-qt.exe`
4. Click **Set As Active Profile**.
5. Add cheats and macros specific to that game!

Supported out of the box:
* **PCSX2** (`pcsx2-qt.exe`, `pcsx2.exe`)
* **RPCS3** (`rpcs3.exe`)
* **Dolphin** (`Dolphin.exe`)
* **DuckStation** (`duckstation-qt-x64-ReleaseLTCG.exe`)
* **RetroArch** (`retroarch.exe`)
* **PPSSPP** (`PPSSPPWindows64.exe`)
* **Custom PC Games**

---

## 11. Importing and Exporting Profiles

Profiles are 100% portable JSON files:

* **Export**: Click **Export Profile** in the Game Profiles tab to save a `.json` profile that you can share with other users or backup.
* **Import**: Click **Import Profile (.json)** and select any exported JSON file. Mr.Cheater will validate and load the game profile, cheats, and mappings automatically.

---

## 12. Safety, Focus Protection & Settings

Navigate to **Settings** to adjust safety thresholds:

* **Require target process running**: Ensures cheats are never sent if the emulator isn't open.
* **Require target window focus** *(Default: ON)*: Prevents typing cheat sequences into Discord, Notepad, or web browsers if you accidentally press a hotkey while tabbed out.
* **Max sequence duration watchdog**: Prevents runaway loops by hard-killing any sequence exceeding the configured timeout (e.g. 15 seconds).
* **System Tray**: Minimize the application to the Windows Notification Area.

---

## 13. Troubleshooting & FAQ

#### Q: The cheat sequence didn't register in PCSX2.
* **Fix**: Ensure PCSX2 is in the foreground and focused.
* Check your PCSX2 Pad settings to ensure the keyboard keys match the Mr.Cheater mapping (`I`, `O`, `K`, `J`, Arrow Keys).
* If your emulator runs in high CPU load, increase the default step delay in Settings from `70ms` to `90ms` or `100ms`.

#### Q: Can I run cheats in background while PCSX2 is not focused?
* Yes, in **Settings**, toggle **Allow background input injection** (note: some emulators ignore background input depending on their DirectInput/RawInput settings).

#### Q: Where are my profile files saved?
* Click **Open Profiles & Data Directory** in **Settings**, which opens `%APPDATA%\UniversalCheatManager\profiles\`.
* To run in portable mode, place an empty file named `portable.dat` in the same directory as the executable.
