# Airk

A single-player MUD set in a cyberpunk metropolis, built in C#.

This project is an experiment in AI-driven game development.
The game is written, tested, played, and debugged primarily by Claude Code,
with limited human guidance.

## About

Airk is a text-based game inspired by classic MUDs. Explore the streets and back alleys of Kreznik, a sprawling cyberpunk city of neon lights and corporate shadows. The world is defined in human-readable markdown files — maps as ASCII grids, rooms as `.md` documents.

Design pillars: systemic gameplay, player freedom, a living world, and character progression.

## Features

- Dual-mode execution: interactive REPL or single-turn batch mode (JSON output)
- Data-driven world: sector maps and room definitions in markdown files
- ASCII grid maps parsed at runtime — easy to hand-edit and extend
- Room descriptions with `*emphasis*` rendered as colored text in interactive mode
- NPC dialogue system with selectable choices and conditional responses
- Inventory, economy, gated exits, and metro travel infrastructure
- Persistent game state saved between sessions

## Requirements

- .NET 10.0 SDK

## Build & Run

```bash
# Build
dotnet build Airk/Airk.csproj

# Run interactive mode
dotnet run --project Airk/Airk.csproj

# Run batch mode (single command, JSON output)
dotnet run --project Airk/Airk.csproj -- <command>

# Start new game
dotnet run --project Airk/Airk.csproj -- new
```

## Commands

| Command | Description |
|---------|-------------|
| `look` | Examine your surroundings or an item/NPC |
| `look <direction>` | Preview what lies in a direction |
| `go <direction>` | Move (north/south/east/west or n/s/e/w) |
| `exits` | List all exits with short descriptions |
| `take <item>` | Pick up an item |
| `drop <item>` | Drop an item |
| `inventory` | List carried items |
| `talk <person>` | Talk to an NPC |
| `use <item>` | Use an item from your inventory |
| `read` | Read a sign or notice in the room |
| `open <item>` | Open or unwrap something |
| `give <item> to <person>` | Give an item to an NPC |
| `travel <code>` | Ride the metro to another station |
| `help` | Show available commands |
| `quit` | Exit the game |

## World Authoring

Sectors are defined in `Assets/World/` as markdown files.

**Map** (`test.md`) — ASCII grid inside a code block:
```
    a00+a03
     +   +
a02+a01 a04
     +
    b00
```

**Room** (`test_a00.md`) — markdown with H1 name and field syntax:
```markdown
# Starting Alley

**short:** Dead end alley with a flickering light.

**description:**
You're in a narrow dead-end alley. *Dumpsters* overflow
against one wall. A single light flickers overhead.
```

## License

See LICENSE.txt
