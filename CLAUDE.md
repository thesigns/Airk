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

7-room cyberpunk world with 2 quest chains, dialogue choices, gated exits, and 10 commands. Fully playable.

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
│   ├── GoCommand.cs       Movement + gated exit checks
│   ├── TakeCommand.cs     Pick up items
│   ├── DropCommand.cs     Drop items
│   ├── InventoryCommand.cs List carried items
│   ├── TalkCommand.cs     NPC dialogue with choice system
│   ├── UseCommand.cs      Context-sensitive item usage
│   ├── PayCommand.cs      Pay for services (metro fare)
│   ├── HelpCommand.cs     Show commands
│   └── QuitCommand.cs     Exit game
├── State/
│   ├── GameState.cs       Full internal state (serialized to session.json)
│   ├── PlayerView.cs      Projected player-visible state (output as JSON)
│   └── WorldBuilder.cs    Creates initial world with NPCs
├── World/
│   ├── Room.cs            Room + ExitGate data structures
│   └── Npc.cs             NPC + DialogueLine structures
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

**Rooms** (7): alley (start), bar, street, metro, platform, clinic, market
**Items** (6): credstick, datapad, transit-map, package, cortex-chip, neural-interface
**NPCs** (4): Chrome (bar), Security Guard (metro), Noodle Vendor (street), Kira (clinic)
**Commands** (10): look, go, take, drop, inventory, talk, use, pay, help, quit

### World Map

```
                    [market]
                       |
                      east
                       |
[alley] --east-- [street] --north-- [metro] --north(gated)--> [platform] --east-- [clinic]
   |
  north
   |
  [bar]
```

### Gated Exits

`Room.GatedExits` maps a direction to an `ExitGate` with `RequiresFlag` and `FailureMessage`. `GoCommand` checks gates after confirming an exit exists. If the flag is missing, the failure message is returned and movement is blocked.

Currently used: metro north → platform (requires `metro_paid` flag, set by `PayCommand`).

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

**Quest 1: Chrome's Package**
1. Talk to Chrome → intro (auto) → choices appear: "Ask about work" / "Ask about Kreznik"
2. Pick work → job offer → accept (auto, receives package)
3. Go to metro, talk guard with package → package delivered
4. Return to Chrome → receive 10 credits
5. Post-quest: new choices appear ("Ask about the datapad" if carrying datapad)

**Quest 2: Kira's Memory Restoration**
1. Take datapad in alley → `use datapad` → reveals Kira/clinic clue (sets `read_datapad`)
2. (Optional) Talk to Noodle Vendor → choices: "Ask about Kira" / "Ask about rumors"
3. Pay 10 credits at metro → `pay` command sets `metro_paid` flag
4. Go north through turnstile → platform → east to clinic
5. Talk to Kira → intro (auto) → choices: "Show the datapad" / "Ask about Kira" / "Ask about NeoCortex"
6. Show datapad → Kira identifies memory extraction log → quest accepted (auto)
7. Go to night market (street east) → take cortex-chip
8. Return to clinic → talk Kira → memory restoration scene (removes chip, reveals Project Icarus)

**Credit economy**: credstick +5, Chrome job +10, metro fare -10 → 5 remaining. Chrome quest must complete before affording metro.

### Design Decisions

- **Turn counter**: Only increments on successful commands (failed commands don't cost a turn)
- **CommandResult.Success**: Determines if turn increments and can be used for future mechanics
- **Dialogue priority**: Story beats (auto, non-repeatable) → choices (labeled) → fallbacks (auto, repeatable)
- **Text wrapping**: `TextFormatter.WordWrap()` wraps text at 80 columns, breaking only at spaces; preserves existing newlines; handles words longer than 80 chars by forced break
- **Gated exits**: Separate `GatedExits` dictionary on Room avoids changing the `Exits` type; backward-compatible with old saves (empty dict by default)
- **UseCommand**: Hardcoded item logic, matching pattern of TakeCommand (credstick special case) and LookCommand (item descriptions dict)
- **PayCommand**: Room-specific; currently only metro. Extensible via room ID switch.

### Known Gaps (for future iterations)

- No combat system
- Health stat tracked but never changes
- Metro fare is one-time (could be per-trip)
- Project Icarus storyline introduced but not continued
- Kira's second memory restoration session not implemented
- No equipment/wearable system
- Neural-interface item in clinic has no use yet
- No save slot or multiple saves
