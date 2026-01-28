# Airk Project — BOOTSTRAP

## Core Idea

Airk is a **text-based console game** inspired by classic interactive fiction (e.g. Zork), with a **cyberpunk setting**, designed to be playable **both by humans and by AI agents**.

The same game logic must support two execution modes:
- an interactive, human-facing mode
- a non-interactive, machine-facing mode

Both modes must behave identically in terms of game rules and outcomes.

---

## Game Genre and Tone

- The game is a **text adventure** (interactive fiction).
- Narrative, world, and mechanics must reflect **cyberpunk themes**: megacorporations, hackers, neon cities, surveillance, body augmentation, etc.
- The game should be engaging, coherent, and replayable.
- No omniscient narration or meta-knowledge is allowed.

---

## Execution Modes

### Interactive Mode
- REPL-style console application.
- Intended for human players.
- Prints a textual response after each command.
- Waits for the next command.
- No JSON output required.

### Batch Mode
- Intended for AI agents and automation.
- Executed with command-line arguments.
- Exactly one turn per process execution.
- Flow:
  1. Load game state from `session.json` (working directory)
  2. Apply exactly one player command
  3. Save updated state back to `session.json`
  4. Print a single JSON object to stdout
  5. Terminate immediately

- The JSON output is the **only supported API**.

---

## Save File and Internal State (Critical)

- Game state is stored in `session.json`.
- The file:
  - is an internal implementation detail
  - is not a public or stable data format
  - is not an API
  - may change at any time

The file serves two purposes:
1. Internal engine state storage
2. Save file for Interactive Mode

Only the game engine may read or write this file.

Modifying or inspecting the save file outside the engine is considered cheating **during normal gameplay**.

During development and debugging, inspecting or editing the save file is explicitly allowed and expected.
No gameplay feature may rely on such access.

---

## Player-Visible Information Rules

Players (human or AI) may only receive information that a human could realistically track using pen and paper.

Allowed:
- player position
- basic stats (e.g. health)
- turn number
- result messages

Disallowed:
- world maps
- hidden objects or NPC state
- RNG seeds or internal flags
- future events or triggers

---

## Projection Rule

There must be a strict separation between:
- internal game state
- player-visible game state

Game logic must:
1. Update internal state
2. Project it into a limited, player-facing view

Guiding question:
> “What could a human reasonably write down on paper?”

Only that information may be exposed.

---

## Single Source of Truth

- Game rules exist exactly once.
- Interactive and Batch modes share the same engine.
- Differences are limited to input handling, output formatting, and process lifetime.

---

## Safety and Execution Constraints

The program must be **safe to build and safe to run**.

- No shell or external program execution
- No dynamic code generation
- No network access
- No file access outside the project directory
- No privilege escalation or sandbox escape

Security and determinism take priority over convenience.
