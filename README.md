# Airk

A terminal-based text adventure game set in a cyberpunk world. Written in C# targeting .NET 10.0.

This project is an experiment in AI-driven game development.

The game is written, tested, played, and debugged primarily by Claude Code,
with limited human guidance. Claude Code designs the systems, creates the narrative, and iterates on gameplay by actually playing the game.

## About

Airk is an interactive fiction game where you explore the rain-soaked streets of Kreznik, a dystopian city of neon lights and corporate shadows. Navigate through dark alleys, seedy bars, and metro stations while interacting with NPCs and completing quests.

## Features

- Classic text adventure gameplay with parser-based commands
- Dual-mode execution: interactive REPL or single-turn batch mode (JSON output)
- NPC dialogue system with conditional responses
- Inventory and economy system
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
| `look` | Examine your surroundings |
| `go <direction>` | Move (north/south/east/west or n/s/e/w) |
| `take <item>` | Pick up an item |
| `drop <item>` | Drop an item |
| `inventory` | List carried items |
| `talk <person>` | Talk to an NPC |
| `help` | Show available commands |
| `quit` | Exit the game |

## Example Session

```
=== Dark Alley ===

You wake up in a dark alley. Your head pounds. You don't remember how you got
here. A flickering neon sign above reads 'Welcome to Kreznik' - but that doesn't
help. Everyone knows Kreznik.

Rain drips from rusted fire escapes above. Neon signs flicker through the smog,
casting red and blue shadows on the wet pavement. A dumpster overflows with
discarded tech. To the north, a heavy steel door leads into a building. The
alley continues east toward the main street.

You see: credstick, datapad

Exits: north, east
[Turn 1 | Health: 100 | Credits: 0]

> take credstick
You pick up the credstick. It dissolves in your hand, transferring 5 credits.

> go north
You head north.

=== Rusty Bolt Bar ===

Synth music pulses from cracked speakers...
```

## License

See LICENSE.txt
