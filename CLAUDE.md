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

Data-driven world loading from markdown asset files. Test sector with 8 rooms. Sharpex scripting engine for conditional logic on exits (and future systems). No NPCs, items, dialogue, or metro yet — those systems exist in code (commands, models) but have no world content.

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
│   ├── ReadCommand.cs     Read signs/notices in rooms
│   ├── OpenCommand.cs     Open/unwrap items
│   ├── GiveCommand.cs     Give items to NPCs
│   ├── TravelCommand.cs   Metro travel between stations
│   ├── HelpCommand.cs     Show commands
│   └── QuitCommand.cs     Exit game
├── State/
│   ├── GameState.cs       Full internal state + MetroStations
│   ├── PlayerView.cs      Projected player-visible state (output as JSON)
│   ├── WorldBuilder.cs    Creates initial world (delegates to WorldLoader)
│   └── WorldLoader.cs     Loads sector .md files, applies room definitions (two-pass)
├── Sharpex/
│   ├── Sharpex.cs         Sharpex scripting engine (external, single-file DSL)
│   └── SharpexFunctions.cs Game-specific Sharpex functions (#go, #msg, #pay) + Execute wrapper
├── World/
│   ├── Room.cs            Room + ExitGate
│   ├── Npc.cs             NPC + DialogueLine structures
│   ├── MetroStation.cs    Metro station data
│   ├── MapParser.cs       Parses ASCII grid map from markdown code blocks
│   ├── RoomFileParser.cs  Parses **field:** markdown format + Additional Exits for room definitions
│   └── WorldLoadException.cs  Custom exception for world/room parsing errors
├── Persistence/
│   └── SessionManager.cs  Load/save session.json
├── UI/
│   ├── IGameUI.cs         Interface for input/output
│   ├── InteractiveUI.cs   Console REPL mode + emphasis rendering
│   ├── BatchUI.cs         Single-turn JSON mode
│   └── TextFormatter.cs   Word wrapping utility (80 columns)
└── Assets/
    └── World/             Sector maps and room definition files (.md)
```

### Data Flow

1. `Program.cs` determines mode from args
2. `SessionManager.Load()` loads or creates `GameState`
3. On new game: `WorldBuilder.CreateNewGame()` → `WorldLoader.LoadSector()` → `MapParser.Parse()` + pass 1: `RoomFileParser.Apply()` (names/descriptions) + pass 2: `RoomFileParser.ParseAdditionalExits()` (custom exits + Sharpex script extraction)
4. On existing session: `WorldBuilder.ReloadScripts(state)` re-reads Sharpex scripts from .md files (scripts are `[JsonIgnore]`, not persisted in session.json)
5. `GameEngine.Run()` loops (once in batch, repeatedly in interactive):
   - `IGameUI.ReadCommand()` gets input
   - `CommandProcessor.Execute()` routes to `ICommand`
   - Command mutates `GameState`, returns `CommandResult`
   - `GameState.Project()` creates `PlayerView`
   - `IGameUI.DisplayView()` outputs result
   - `SessionManager.Save()` persists state

### Data-Driven World System

World content lives in `Assets/World/` as markdown files, copied to build output.

#### Sector Map Files

File: `{sectorId}.md` (e.g. `test.md`)

Map is inside a markdown code block (``` delimiters). Everything outside = comments/docs.

```
# Test Zone
‍```
    a00+a03
     +   +
a02+a01 a04
     +
    b00
‍```
Notes and documentation here are ignored by the parser.
```

**Grid rules:**
- Room = 3 chars: `[a-z][0-9][0-9]` (letter prefix = sub-area, digits = room number)
- `+` = open passage (horizontal between rooms, vertical between rows)
- Space = no connection / no room
- Rooms at columns 0, 4, 8, ... on even rows
- Horizontal connectors at columns 3, 7, 11, ... on even rows
- Vertical connectors at columns 1, 5, 9, ... on odd rows
- Empty/whitespace-only line inside code block = end of map data

**Global room IDs:** `{sectorId}_{localId}` (e.g. `test_a00`). One sector can have up to 2600 rooms (26 letters × 100 numbers). Local IDs must be unique within a sector (duplicates → error on load).

**Default spawn:** Lexicographically lowest room ID in the sector.

**`MapParser.Parse(sectorId, mapText)`**: Strips BOM, scans grid, builds Room objects with exits. Generic defaults: Name = localId, ShortDescription = "A room.", Description = "You are in room {localId}."

#### Room Definition Files

File: `{sectorId}_{localId}.md` (e.g. `test_a00.md`)

**Optional** — if no file exists, room keeps MapParser defaults.

```markdown
# Starting Alley

**short:** Dead end alley with a flickering light.

**description:**
You're in a narrow dead-end alley. *Dumpsters* overflow
against one wall. A single light flickers overhead,
casting unsteady shadows on wet concrete.

## Additional Exits
- **up:** c00 (Rooftop) #pay 10 ? #go c00 #msg "Paid." : #msg "No money."
- **down:** d00 (Maintenance Tunnel)
```

**Format rules:**
- **Room name** = `# H1 heading` (exactly one allowed per file; multiple → parser error)
- **Fields** = lines starting with `**fieldname:**`
- Content = text after `:** ` on same line + subsequent non-empty lines
- If nothing after `:**`, content starts from next line (field-name-only line skipped)
- Content ends at: empty line, next field, or EOF
- Everything outside H1/fields/Additional Exits = ignored (comments, markdown formatting)
- Newlines in content are preserved (each line becomes a separate line in-game)

**Supported fields:** `short`, `description`

**`RoomFileParser.Apply(room, fileText)`**: Extracts name from H1, parses fields, overrides Room properties. Room.Name/ShortDescription/Description use `set` (not `init`) to allow post-construction override.

#### Additional Exits

Optional `## Additional Exits` section in room files. Defines custom named exits (up, down, enter, climb, etc.) beyond the cardinal directions from the grid map.

**Format:** `- **direction:** localId (Room Name) [optional Sharpex script]`

**Rules:**
- Direction = lowercase word, must not conflict with command names, aliases, or cardinal directions
- Destination = local room ID `[a-z][0-9][0-9]` within the same sector
- Room Name in parentheses = mandatory checksum; must match the destination room's actual Name
- Optional Sharpex script after the parentheses overrides default go behavior for this exit
- Exits are **one-directional** — no automatic return exits; each room defines its own
- Strict parser: reserved word collision, duplicate directions, or malformed lines → `WorldLoadException`

**`RoomFileParser.ParseAdditionalExits(roomId, text)`**: Parses the section, returns list of `(direction, localId, expectedName, scriptText)`. Called by WorldLoader in pass 2 after all room names are finalized. WorldLoader validates destination existence and name match, resolves local IDs in `#go` references to global IDs, stores raw script string in `Room.ExitScripts`.

**Behavior:** `up` or `go up` moves; `look up` shows destination's ShortDescription. Bare custom direction words are caught by CommandProcessor fallback (checks current room exits when verb is unknown).

#### Sharpex

Sharpex is an external, single-file behavioral scripting DSL for C#. Located in `Airk/Sharpex/Sharpex.cs`. Game-specific functions are in `Airk/Sharpex/SharpexFunctions.cs`.

**Syntax:** `#function arg1 arg2` — every function returns bool. Quoted strings for args with spaces: `#msg "Hello world"`.

**Operators** (high → low precedence):
- Space (AND): `#f1 arg #f2 arg` — short-circuit, stops on first false
- `|` (OR): `#f1 arg | #f2 arg` — short-circuit, stops on first true
- `? :` (conditional): `#cond ? #true : #false`
- `>` (sequence): `#s1 > #s2` — always runs both, returns last result
- `~` (negation): `~func arg` — inverts result

**Current game functions** (defined in `SharpexFunctions.cs`):
- `#go globalRoomId` — move player to room, always returns true. Local ID resolved to global ID at load time by `WorldLoader.ResolveGoReferences()`.
- `#msg "text"` — display message text, always returns true
- `#pay amount` — deduct credits if enough → true; not enough → false (no deduction)

**Example:** `#pay 10 ? #go c00 #msg "You walk up the stairs." : #msg "Not enough money to go up."`

**Architecture:**
- `Sharpex.Eval(script)` — tokenizes, parses, and evaluates in one call; returns bool
- Functions registered via `[Sharpex("name")]` attribute on static bool methods; auto-discovered by reflection at startup
- `SharpexFunctions.Execute(state, script, direction)` — sets static context, calls `Sharpex.Eval()`, builds `CommandResult` from accumulated side effects (movement, messages)

**Serialization:** `Room.ExitScripts` is `Dictionary<string, string>` with `[JsonIgnore]`. Scripts stored as raw strings (with global IDs resolved). Not persisted in session.json. On session load, `WorldBuilder.ReloadScripts(state)` re-reads scripts from asset .md files.

**Integration with GoCommand:** If `room.ExitScripts` has an entry for the direction, the script completely replaces default behavior (no GatedExits/ExitCosts check). Script controls movement, messaging, and success/failure.

#### Emphasis

`*text*` in descriptions = emphasis. Preserved as-is in the data.

- **Interactive mode**: `WriteWithEmphasis()` in InteractiveUI renders emphasized text in White (ConsoleColor.White), normal text in Gray. Asterisks are stripped from display.
- **Batch mode (JSON)**: Asterisks preserved in output. AI agents can parse them.

### World Content

**Sector:** test (8 rooms)
**Rooms:** test_a00 (Starting Alley), test_a01 (Pipe Junction), test_a02 (Ventilation Shaft), test_a03 (Dumpster Nook), test_a04 (Fire Escape Landing), test_b00 (Neon Boulevard), test_c00 (Rooftop), test_d00 (Maintenance Tunnel)
**Items:** none yet
**NPCs:** none yet
**Commands** (14): look, go, take, drop, inventory, talk, use, read, open, give, travel, exits, help, quit

### Design Decisions

- **Turn counter**: Only increments on successful commands (failed commands don't cost a turn)
- **CommandResult.Success**: Determines if turn increments and can be used for future mechanics
- **Data-driven world**: Rooms defined in .md files, not hardcoded. MapParser handles spatial layout, RoomFileParser handles content. WorldLoader orchestrates both.
- **Markdown as data format**: Sector maps use code blocks (parseable + readable as docs). Room files use bold field markers (**field:**). Files double as documentation.
- **Global room IDs**: `{sector}_{localId}` allows same local IDs in different sectors (e.g. `s7_a00` and `rim_a00` are distinct rooms).
- **Text wrapping**: `TextFormatter.WordWrap()` wraps at 80 columns, preserves existing newlines, handles long words by forced break.
- **Emphasis rendering**: `*text*` → colored in interactive mode, preserved in batch JSON. Simple toggle-on-asterisk approach.
- **Custom exits (Additional Exits)**: Room files can define named exits (up, down, etc.) via `## Additional Exits` section. One-directional, same-sector, with mandatory name checksum. Two-pass loading: pass 1 finalizes names, pass 2 parses/validates exits + scripts. Bare custom directions handled by CommandProcessor fallback.
- **Sharpex scripting**: External single-file DSL (`Sharpex.cs`) with `#function arg` syntax. Functions registered via `[Sharpex]` attribute, auto-discovered by reflection. Game functions in `SharpexFunctions.cs` use static context pattern for GameState access. Scripts stored as raw strings in `Room.ExitScripts`, not persisted in session.json (re-read from asset files on every load).
- **Gated exits**: `Room.GatedExits` dictionary with `RequiresFlag` and `FailureMessage`. Checked by GoCommand. Not yet used in world content. Can be superseded by Sharpex scripts for complex gating.
- **Exit costs**: `Room.ExitCosts` dictionary mapping direction → credit cost. Can be superseded by `#pay` in Sharpex scripts.
- **ShowDescription flag**: `CommandResult.ShowDescription` controls detail level. `look` (no args) and `go` set it to `true`.
- **Cyan prompt**: Interactive mode prompt `>` rendered in cyan.

### Known Gaps (for future iterations)

- No NPCs or dialogue in current world content (code infrastructure exists)
- No items in current world content (take/drop/use/give commands exist but nothing to interact with)
- No combat system
- Health stat tracked but never changes
- No metro stations in current world content (travel command exists)
- No equipment/wearable system
- No save slot or multiple saves
- Room file format supports name/short/description/additional exits with Sharpex scripts (items, gates, costs, readables, spawn not yet in file format)
- Sharpex has only 3 game functions (#go, #msg, #pay) — future: #flag, #require, #give, #take, etc.
- No multi-sector world (only test sector loaded)
