# User Manual — Mr.Cheater (Universal Cheat & Macro Manager)

---

## 1. Introduction & Concepts

**Mr.Cheater** is a high-performance Windows automation tool for retro and modern gaming. It allows you to trigger complex controller combinations and cheat codes in emulators (like PCSX2, RPCS3, Dolphin, RetroArch) and PC games using simple global hotkeys or one-click dashboard buttons.

### Core Concepts:
* **Game Profile**: A profile containing cheat definitions, macros, and input bindings for a specific game.
* **Cheat Definition**: A named sequence of controller actions (e.g. `UP, TRIANGLE, DOWN, CROSS`) designed to trigger in-game cheats.
* **Macro**: A repetitive or multi-loop combo routine (e.g. turbo money grinder, combo attack).
* **Input Mapping**: A translation map that maps generic game controller actions (`TRIANGLE`, `CIRCLE`, `D-PAD UP`) to emulator-specific keyboard scan codes or virtual keys.
* **Prerequisite Resolver**: Automatically ensures prerequisite cheats (such as the Master Code in Downhill Domination) are executed before the requested cheat.
* **Smart Session Lock**: Remembers if a Master Code is already active so it is not toggled off by subsequent cheat executions.
* **Sequence Engine**: The high-precision execution runner that resolves prerequisites, verifies emulator focus, and simulates hardware key strokes.

---

## 2. Simultaneous Gamepad + Cheat Execution (Important)

You **do not need to switch between Gamepad and Keyboard profiles** in PCSX2. PCSX2 supports **Dual-Input Mapping on Controller Port 1**.

### How to Play with your Controller while Cheats Work Simultaneously:

1. Open **PCSX2** -> **Settings** -> **Controllers** -> **Controller Port 1 (DualShock 2)**.
2. Keep your **Physical Gamepad** mapped (e.g. Xbox Controller / DualSense / DualShock).
3. Add **Keyboard secondary bindings** to the exact same buttons:
   * **D-Pad Up**: `Up Arrow`
   * **D-Pad Down**: `Down Arrow`
   * **D-Pad Left**: `Left Arrow`
   * **D-Pad Right**: `Right Arrow`
   * **Triangle (△)**: `I`
   * **Circle (◯)**: `L`
   * **Cross (✕)**: `K`
   * **Square (▢)**: `J`
   * **L1**: `Q`, **L2**: `1`, **R1**: `E`, **R2**: `3`
   * **Select**: `Backspace`, **Start**: `Return (Enter)`

> [!NOTE]
> PCSX2 allows multiple input sources per button. In the binding list, a button will display both your controller and keyboard (e.g. `SDL-0/Button Y, Keyboard/I`). This allows you to play the entire game with your physical controller, while Mr.Cheater injects cheats over the keyboard layer seamlessly.

---

## 3. PCSX2 & Downhill Domination Quickstart

### Step 1: Open PCSX2 & Start Downhill Domination
1. Launch PCSX2 (`pcsx2-qt.exe`).
2. Start **Downhill Domination**.

### Default PS2 Keyboard Mapping Table

| PS2 Action | PCSX2 Keyboard Key | Gamepad Equivalent |
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
2. Observe the top status pill: when PCSX2 is running, it shows `PCSX2 - Active & Focused`.

### Step 3: Trigger a Cheat
* **Method A (In-App Button)**: Go to **Dashboard** or **Cheats Library**, click **Execute** next to `Unlock Everything` or `Always Stoked`.
* **Method B (Global Hotkey)**: While playing the game on your controller, press **`F2`** (Unlock Everything), **`F4`** (Always Stoked), or **`F3`** ($5,000 Cash).
* **Automated Result**: Mr.Cheater brings PCSX2 to focus, sends the sequence instantly, and unlocks the feature in-game.

---

## 4. Downhill Domination Cheat Code Catalogue

| Cheat Name | Effect | Sequence | Prerequisite | Hotkey |
| :--- | :--- | :--- | :--- | :--- |
| **Master Code** | Unlocks cheat engine | `UP, TRIANGLE, DOWN, CROSS, LEFT, CIRCLE, RIGHT, SQUARE` | None | `F1` |
| **Unlock Everything** | All riders, bikes, upgrades & tracks | `DOWN, UP, UP, DOWN, DOWN, UP, UP` | Master Code | `F2` |
| **$5,000 Cash** | Adds $5,000 money | `RIGHT, UP, UP, CIRCLE, CIRCLE, SQUARE` | Master Code | `F3` |
| **$2,000 Cash** | Adds $2,000 money | `RIGHT, TRIANGLE, TRIANGLE, LEFT` | Master Code | None |
| **Adrenaline Boost** | Max adrenaline boost gauge | `DOWN, LEFT, LEFT, RIGHT` | Master Code | None |
| **Always Stoked** | Unlimited permanent boost | `DOWN, SQUARE, SQUARE, LEFT, CIRCLE` | Master Code | `F4` |
| **Infinite Energy** | Rider never gets exhausted | `DOWN, TRIANGLE, LEFT, LEFT, SQUARE` | Master Code | `F5` |
| **Restore Energy** | Instantly refills energy | `DOWN, RIGHT, RIGHT, LEFT, LEFT` | Master Code | None |
| **Speed Freak** | Uncaps top speed limiter | `DOWN, TRIANGLE, RIGHT, RIGHT, SQUARE` | Master Code | None |
| **Mega Flip** | Extreme rotational flip speed | `RIGHT, UP, UP, RIGHT, RIGHT, SQUARE` | Master Code | `F6` |
| **Super Bounce** | Super high ground bounce | `LEFT, SQUARE, CROSS, UP, TRIANGLE` | Master Code | None |
| **Super Bunny Hop** | Super jump height | `UP, CROSS, LEFT, SQUARE, UP` | Master Code | None |
| **Anti-Gravity** | Low gravity physics | `DOWN, TRIANGLE, SQUARE, SQUARE, UP` | Master Code | None |
| **Infinite Bottles** | Unlimited water bottles to throw | `UP, CROSS, LEFT, LEFT, CIRCLE, CIRCLE` | Master Code | None |
| **Combat Upgrade** | Heavy punch/kick tier upgrade | `UP, DOWN, LEFT, RIGHT, CROSS` | Master Code | None |
| **Free Combat** | Unlimited attacks without stamina drain | `UP, DOWN, LEFT, RIGHT, SQUARE` | Master Code | None |

---

## 5. Sequence Editor Guide

Create custom button combinations with microsecond accuracy:

1. Open **Sequence Editor** from the navigation sidebar.
2. Enter a **Name**, **Description**, and **Category**.
3. Use the **Button Palette** to add controller actions:
   * Face buttons: `TRIANGLE`, `CIRCLE`, `CROSS`, `SQUARE`
   * D-Pad: `UP`, `DOWN`, `LEFT`, `RIGHT`
   * Shoulders / Triggers: `L1`, `L2`, `R1`, `R2`
   * Stick clicks: `L3`, `R3`
4. Set custom hold duration and pause delay per step if required.
5. Check **Requires Master Code** if this cheat needs the game's prerequisite code.
6. Click **Simulate Run** to verify, or **Test Live** to send to the running game.
7. Click **Save Cheat**.

---

## 6. Macro Engine Guide

Automate repetitive routines or turbo buttons:

1. Open **Macro Engine** from the navigation sidebar.
2. Click **+ Add New Macro**.
3. Set the **Loop Count** (e.g., `1x` for a combo, or `5x` for a repeating cycle).
4. Configure your step sequence with fine-grained delays.
5. Assign a global hotkey or click **Run Macro** to execute.

---

## 7. Emergency Stop & Safety

* **Global Emergency Stop Hotkey**: `Ctrl + Shift + Escape`
* **Trigger Mechanism**: Pressing this key combination immediately interrupts any active sequence execution, aborts background delays, and sends hardware key-release signals for all held keys to prevent stuck inputs.
