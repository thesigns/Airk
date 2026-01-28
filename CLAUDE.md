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

3. **Stop After 10 Turns**
   - After at most 10 turns, stop playing.
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

Core game infrastructure is implemented and playable. 4-room cyberpunk scenario with basic commands.

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
│   ├── GoCommand.cs       Movement (supports n/s/e/w shortcuts)
│   ├── TakeCommand.cs     Pick up items
│   ├── DropCommand.cs     Drop items
│   ├── InventoryCommand.cs List carried items
│   ├── TalkCommand.cs     NPC dialogue system
│   ├── HelpCommand.cs     Show commands
│   └── QuitCommand.cs     Exit game
├── State/
│   ├── GameState.cs       Full internal state (serialized to session.json)
│   ├── PlayerView.cs      Projected player-visible state (output as JSON)
│   └── WorldBuilder.cs    Creates initial world with NPCs
├── World/
│   ├── Room.cs            Room data structure
│   └── Npc.cs             NPC + DialogueLine structures
├── Persistence/
│   └── SessionManager.cs  Load/save session.json
└── UI/
    ├── IGameUI.cs         Interface for input/output
    ├── InteractiveUI.cs   Console REPL mode
    └── BatchUI.cs         Single-turn JSON mode
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

**Rooms**: alley (start), bar, street, metro
**Items**: credstick (5 credits, dissolves on take), datapad, transit-map, package (quest item)
**NPCs**: Chrome (bartender, quest giver), Security Guard (quest target)
**Commands**: look, go, take, drop, inventory, talk, help, quit

### Dialogue System

`DialogueLine` properties:
- `RequiresFlag` / `RequiresItem`: Conditions for availability
- `SetsFlag`: Sets game flag when spoken
- `GivesCredits` / `GivesItem` / `RemovesItem`: Inventory/economy effects
- `Repeatable`: Whether line can be repeated

**Quest: Chrome's Package**
1. Talk to Chrome → intro → job offer → accept (receive package)
2. Talk to guard at metro with package → delivers package (removed from inventory)
3. Return to Chrome → receive 10 credits

### Design Decisions

- **Turn counter**: Only increments on successful commands (failed commands don't cost a turn)
- **CommandResult.Success**: Determines if turn increments and can be used for future mechanics
- **Dialogue priority**: First matching dialogue line is used (order matters in WorldBuilder)

### Known Gaps (for future iterations)

- Datapad mentions "Kira" with no follow-up content
- Metro interior not accessible yet (could unlock after paying fare)
- No combat system
- Chrome has no more jobs after first one
