# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## IMPORTANT

Before doing anything else, **read `BOOTSTRAP.md`**.

`BOOTSTRAP.md` defines all non-negotiable rules of the project.
If there is any ambiguity or conflict, `BOOTSTRAP.md` takes precedence.

Do not invent new rules.
Do not weaken or reinterpret existing constraints.

---

## Project Overview

Airk is a **terminal-based / console game** written in **C#**.

- The game runs entirely in a text console (no GUI).
- Output is produced using standard console I/O.
- The project targets **.NET 10.0**.
- The application is designed to run as a local, standalone executable.

## Build and Run Commands

```bash
# Build
dotnet build Airk/Airk.csproj

# Run interactive mode
dotnet run --project Airk/Airk.csproj

# Run batch mode (single turn)
dotnet run --project Airk/Airk.csproj -- <command>

# Start new game in batch mode
dotnet run --project Airk/Airk.csproj -- new
```

## Iteration Workflow for Claude Code (Mandatory)

Claude Code must follow a **gameplay-driven iteration loop**.

### Standard Loop

1. **Run the Game in Batch Mode**
   - Execute one turn using batch mode.
   - Treat stdout JSON as the only source of player knowledge.

2. **Play as a Fair Player**
   - Test mechanics, balance, clarity, and fun.
   - Look for bugs, unclear rules, and narrative issues.

3. **Stop After 20 Turns**
   - After at most 20 turns, stop playing.
   - Leave the game state in `session.json`.

4. **Improve the Code**
   - Fix bugs and improve design based on gameplay.
   - Respect all rules defined in `BOOTSTRAP.md`.

5. **Update This File (`CLAUDE.md`)**
   - Document:
     - newly added classes
     - major changes to existing classes
     - important architectural decisions
   - Keep documentation factual and concise.

6. **Resume the Game**
   - Continue from the existing `session.json`.

---

## Starting a New Game

Claude Code may start a new game only when justified, for example:
- testing early-game balance
- validating onboarding
- breaking changes invalidate the save
- explicit instruction

In such cases:
- use the `new` parameter in batch mode
- explicitly state the reason
- do not silently discard the previous session

---

## Code and Architecture Notes (Living Section)

This section must be actively maintained.

Document:
- core classes and responsibilities
- key subsystems and data flow
- non-obvious design decisions

This is not user documentation.
It is a **technical memory** for future iterations.

### Current State

10-room, 2-sector cyberpunk world connected by metro system. 2 quest chains (Chrome's package with branching outcome, Kira's memory restoration with deep scan), cross-quest connections, Icarus storyline, 8 NPCs, dialogue choices, metro travel with automatic entry fee, readables, and 14 commands. Fully playable.

### Key Constraints from BOOTSTRAP.md

- **Dual-mode execution**: Same game logic for interactive (REPL) and batch (single-turn, JSON output) modes
- **Projection rule**: Strict separation between internal state and player-visible state
- **session.json**: Internal state file, not a public API; only the engine reads/writes it
- **No external access**: No shell exec, no network, no dynamic code gen, no files outside project dir

### Architecture Overview

```
Program.cs                 Entry point, mode detection
├── Engine/
│   ├── GameEngine.cs      Main loop coordinator
│   └── CommandProcessor.cs Command routing and execution
├── Commands/
│   ├── ICommand.cs        Interface + CommandResult record
│   ├── LookCommand.cs     Examine room/items/NPCs
│   ├── GoCommand.cs       Movement + gated exits + exit costs
│   ├── TakeCommand.cs     Pick up items
│   ├── DropCommand.cs     Drop items
│   ├── InventoryCommand.cs List carried items
│   ├── TalkCommand.cs     NPC dialogue with choice system
│   ├── UseCommand.cs      Context-sensitive item usage
│   ├── ReadCommand.cs     Read signs/notices in rooms (regulamin)
│   ├── OpenCommand.cs     Open/unwrap items (package)
│   ├── GiveCommand.cs     Give items to NPCs (neural-interface to Kira)
│   ├── TravelCommand.cs   Metro travel between stations
│   ├── HelpCommand.cs     Show commands
│   └── QuitCommand.cs     Exit game
├── State/
│   ├── GameState.cs       Full internal state + MetroStations
│   ├── PlayerView.cs      Projected player-visible state (output as JSON)
│   └── WorldBuilder.cs    Creates initial world with NPCs and metro network
├── World/
│   ├── Room.cs            Room + ExitGate + ExitCosts + Readables
│   ├── Npc.cs             NPC + DialogueLine structures
│   └── MetroStation.cs    Metro station data (Code, Name, PlatformRoomId, Line)
├── Persistence/
│   └── SessionManager.cs  Load/save session.json
└── UI/
    ├── IGameUI.cs         Interface for input/output
    ├── InteractiveUI.cs   Console REPL mode
    ├── BatchUI.cs         Single-turn JSON mode
    └── TextFormatter.cs   Word wrapping utility (80 columns)
```

### Data Flow

1. `Program.cs` determines mode from args
2. `SessionManager.Load()` loads or creates `GameState`
3. `GameEngine.Run()` loops (once in batch, repeatedly in interactive):
   - `IGameUI.ReadCommand()` gets input
   - `CommandProcessor.Execute()` routes to `ICommand`
   - Command mutates `GameState`, returns `CommandResult`
   - `GameState.Project()` creates `PlayerView`
   - `IGameUI.DisplayView()` outputs result
   - `SessionManager.Save()` persists state

### World Content

**Rooms** (10): alley (start), bar, street, metro, platform, clinic, market, outerrim-platform, facility-gate, maintenance
**Items** (7): credstick, datapad, transit-map, package, cortex-chip, neural-interface, access-card
**NPCs** (8): Chrome (bar), Security Guard (metro), Noodle Vendor (street), Kira (clinic), Scavenger (market), Drifter (platform), Echo (maintenance), Facility Guard (facility-gate)
**Commands** (14): look, go, take, drop, inventory, talk, use, read, open, give, travel, exits, help, quit

### World Map

```
SECTOR 7 (Station: s7)
                    [market]
                       |
[alley] --east-- [street] --north(5cr)--> [metro] --north-- [platform] --east-- [clinic]
   |                                                               |
  north                                                     travel (Red Line)
   |                                                               |
  [bar]                                                            |
                                                                   |
OUTER RIM (Station: rim)                                           |
                                                       [outerrim-platform] --east-- [maintenance]
                                                               |
                                                             north
                                                               |
                                                         [facility-gate]
```

### Gated Exits and Exit Costs

`Room.GatedExits` maps a direction to an `ExitGate` with `RequiresFlag` and `FailureMessage`. `GoCommand` checks gates after confirming an exit exists. If the flag is missing, the failure message is returned and movement is blocked. Currently unused (reserved for future content).

`Room.ExitCosts` maps a direction to a credit cost (int). `GoCommand` checks costs after gates. If the player has insufficient credits, movement is blocked with a message. Otherwise, the cost is deducted automatically and the player is informed. Currently used: street north → metro (5 credits).

### Metro System

`MetroStation` class (`World/MetroStation.cs`): Code, Name, PlatformRoomId, Line. Stored in `GameState.MetroStations` keyed by station code.

**TravelCommand**: checks if player's current room matches any station's `PlatformRoomId`. No args lists same-line destinations. `travel <code>` moves player to target platform. Same-line only (cross-line requires future hub transfer station).

**Active stations**: `s7` (Sector 7 Platform, Red Line), `rim` (Outer Rim, Red Line). Blue Line service suspended (future).

**Fare model**: Automatic 5-credit deduction when entering metro from street (via `Room.ExitCosts`). Exit from metro to street is free. Internal metro transfers between platforms are free. No `pay` command — fare is automatic on movement.

**Regulamin**: Readable sign at street (before entry) and all metro platforms — in-world tutorial explaining travel system, station codes, entry fee. Read with `read regulamin`.

### Readables

`Room.Readables`: `Dictionary<string, string>` mapping fixture names to text content. Fixed room features that can't be taken (signs, notices). `ReadCommand` checks room readables first, then delegates to `UseCommand` for inventory items (`read datapad`, `read transit-map`). The "read" alias was removed from `UseCommand` to avoid conflict.

### Dialogue System

`DialogueLine` properties:
- `Label`: Short text for dialogue choice menu (nullable; if null, line is auto-executed)
- `RequiresFlag` / `RequiresItem`: Conditions for availability
- `SetsFlag`: Sets game flag when spoken
- `GivesCredits` / `GivesItem` / `RemovesItem`: Inventory/economy effects
- `Repeatable`: Whether line can be repeated

**Dialogue priority in TalkCommand:**
1. **Story beats** (no Label, non-repeatable) — fire automatically first (intros, quest progression)
2. **Choices** (have Label) — if multiple, show numbered menu; if single, auto-execute
3. **Fallbacks** (no Label, repeatable) — idle dialogue when no choices remain

Player selects choices with `talk <npc> <number>`. Menu display does not advance the turn.

### Quests

**Quest 1: Chrome's Package** (branching)
1. Talk to Chrome → intro (auto) → choices appear: "Ask about work" / "Ask about Kreznik"
2. Pick work → job offer → accept (auto, receives package)
3. Player can optionally `open package` — reveals NS-7 Neural Suppressor, sets `opened_package` flag
4. Go to metro, talk guard with package:
   - **Sealed**: Guard takes it normally → sets `job_complete`
   - **Opened**: Guard notices tampering → sets `job_complete_tampered`
5. Return to Chrome:
   - **Sealed**: Receive 50 credits
   - **Opened**: Receive 0 credits (punishment for disobedience)
6. Both paths set `job_paid` flag, enabling Kira quest progression
7. Post-quest: new choices appear ("Ask about the datapad" if carrying datapad)
8. **Tampered path only**: "Ask about the neural suppressor" choice — Chrome confesses to running NS-7s for months

**Quest 2: Kira's Memory Restoration**
1. Take datapad in alley → `use datapad` → reveals Kira/clinic clue (sets `read_datapad`)
2. (Optional) Talk to Noodle Vendor → choices: "Ask about Kira" / "Ask about rumors"
3. Enter metro from street (5 credits auto-deducted at turnstile)
4. Go north to platform → east to clinic
5. Talk to Kira → intro (auto) → choices: "Show the datapad" / "Ask about Kira" / "Ask about NeoCortex"
6. (If opened package) "Tell her about the neural suppressor" — Kira connects NS-7 to player's wipe
7. Show datapad → Kira identifies memory extraction log → quest accepted (auto)
8. Go to night market (street east) → take cortex-chip
9. Return to clinic → talk Kira → memory restoration scene (removes chip, reveals Project Icarus)
10. `give neural-interface to kira` → deep scan: reveals player was a NeoCortex researcher on Project Icarus who was wiped for trying to expose mass neural suppression (sets `deep_scan_done`)

**Credit economy**: credstick +15, Chrome job +50 (sealed) or +0 (opened), metro entry -5 per visit. Opening the package is a choice with major economic consequences (65 vs 15 total credits). Metro is affordable from the start (credstick alone covers 3 entries).

### Design Decisions

- **Turn counter**: Only increments on successful commands (failed commands don't cost a turn)
- **CommandResult.Success**: Determines if turn increments and can be used for future mechanics
- **Dialogue priority**: Story beats (auto, non-repeatable) → choices (labeled) → fallbacks (auto, repeatable)
- **Text wrapping**: `TextFormatter.WordWrap()` wraps text at 80 columns, breaking only at spaces; preserves existing newlines; handles words longer than 80 chars by forced break
- **Gated exits**: Separate `GatedExits` dictionary on Room avoids changing the `Exits` type; backward-compatible with old saves (empty dict by default). Currently unused (reserved for future content).
- **Exit costs**: `ExitCosts` dictionary on Room maps directions to credit costs. `GoCommand` checks after gates, deducts automatically, or blocks if insufficient. Used for metro entry fee (street → metro, 5 credits).
- **UseCommand**: Hardcoded item logic, matching pattern of TakeCommand (credstick special case) and LookCommand (item descriptions dict). `use package` hints toward `open package`.
- **OpenCommand**: Separate from `use` — handles destructive/irreversible item actions (unwrapping package). Aliases: `unwrap`. Sets `opened_package` flag which branches guard/Chrome dialogue.
- **GiveCommand**: `give <item> to <npc>` syntax. Parses with `LastIndexOf(" to ")`. NPC-specific reactions via switch expression. Default: "doesn't seem to want it". Currently handles: neural-interface → Kira (deep scan after memory restoration).
- **TravelCommand**: Data-driven via `GameState.MetroStations`. Same-line only restriction allows future hub-transfer mechanic. Aliases: `ride`. `ShowDescription: true` on arrival.
- **ReadCommand**: Checks `Room.Readables` first, inventory items second (delegates to `UseCommand`). Auto-reads single readable when no args.
- **Readables**: Separate from `Items` — fixtures that can't be taken. Used for the regulamin sign. Keys are case-sensitive lowercase.
- **ShortDescription**: One-sentence room summaries used by `look <direction>` for previewing adjacent rooms. Separate from full `Description`.
- **ShowDescription flag**: `CommandResult.ShowDescription` controls interactive UI detail level. `look` (no args) and `go` set it to `true` (full description + items/people/exits). All other commands show only room name + result message. `[JsonIgnore]` on `PlayerView` — batch JSON always includes full data.
- **Directional look**: `look north` etc. shows `ShortDescription` of the target room, or "nothing noteworthy" if no exit. Does not consume extra info — player gets a preview before committing to move.
- **Cyan prompt**: Interactive mode prompt `>` rendered in cyan via `Console.ForegroundColor`.

### Known Gaps (for future iterations)

- No localization system (removed — all text is hardcoded English)
- No combat system
- Health stat tracked but never changes
- Icarus facility interior not yet accessible (blast door blocks north exit at facility-gate). Access-card from Echo is a quest item for future use
- Blue Line stations (Corp District, Central Hub, Docklands) mentioned in regulamin but not implemented
- Cross-line metro transfers not implemented (future hub station)
- No equipment/wearable system
- No save slot or multiple saves
- No way to earn additional credits after Chrome's one-time job (sealed path gives 65 total, opened gives 15)
