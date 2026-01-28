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

The project is in early bootstrap phase. `Program.cs` contains only a minimal entry point. Core game systems (state management, command parsing, batch/interactive modes, JSON output) are not yet implemented.

### Key Constraints from BOOTSTRAP.md

- **Dual-mode execution**: Same game logic for interactive (REPL) and batch (single-turn, JSON output) modes
- **Projection rule**: Strict separation between internal state and player-visible state
- **session.json**: Internal state file, not a public API; only the engine reads/writes it
- **No external access**: No shell exec, no network, no dynamic code gen, no files outside project dir
