# Airk

A terminal-based text adventure game set in a cyberpunk world.

This project is an experiment in AI-driven game development.

The game is written, tested, played, and debugged primarily by Claude Code,
with limited human guidance. Claude Code designs the systems, creates the narrative, and iterates on gameplay by actually playing the game.

## About

Airk is an interactive fiction game where you explore the rain-soaked streets of Kreznik, a dystopian city of neon lights and corporate shadows. Navigate dark alleys, seedy bars, underground clinics, and metro tunnels while unraveling a conspiracy involving mass memory wiping, a shadowy research project called Icarus, and your own forgotten identity.

## Features

- Classic text adventure gameplay with parser-based commands
- Dual-mode execution: interactive REPL or single-turn batch mode (JSON output)
- NPC dialogue system with selectable choices and conditional responses
- Branching quest outcomes with narrative consequences
- Multi-sector world connected by a metro travel system
- Inventory, economy, and readable environment fixtures
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
| `talk <person>` | Talk to an NPC (select dialogue choices with number) |
| `use <item>` | Use an item from your inventory |
| `read` | Read a sign or notice in the room |
| `open <item>` | Open or unwrap something |
| `give <item> to <person>` | Give an item to an NPC |
| `pay` | Pay for services (e.g. metro fare) |
| `travel <code>` | Ride the metro to another station |
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
discarded tech.

You see: credstick, datapad
Exits: north, east

[Turn 1 | Health: 100 | Credits: 0]

> take credstick
You pick up the credstick. It transfers 5 credits to your account before
dissolving.

> exits
  north: A dimly lit synth bar behind a heavy steel door.
  east: A wide boulevard crowded with holographic ads and buzzing drones.
```

## License

See LICENSE.txt
