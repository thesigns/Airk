using Airk.State;
using Airk.World;

namespace Airk.Commands;

public sealed class TalkCommand : ICommand
{
    public string Name => "talk";
    public string[] Aliases => ["speak", "chat", "ask"];
    public string Description => "Talk to someone (talk <name> or talk <name> <choice>)";

    public CommandResult Execute(GameState state, string[] args)
    {
        var room = state.Rooms[state.CurrentRoomId];

        // Find NPCs in current room
        var npcsHere = state.Npcs.Values
            .Where(n => n.RoomId == state.CurrentRoomId)
            .ToList();

        if (npcsHere.Count == 0)
        {
            return new CommandResult(false, "There's no one here to talk to.");
        }

        // Parse args: last arg may be a choice number
        int? choiceIndex = null;
        var nameArgs = args;
        if (args.Length > 0 && int.TryParse(args[^1], out var parsed))
        {
            choiceIndex = parsed;
            nameArgs = args[..^1];
        }

        // Resolve target NPC
        string targetName;
        if (nameArgs.Length == 0)
        {
            if (npcsHere.Count == 1)
            {
                targetName = npcsHere[0].Id;
            }
            else if (choiceIndex is not null)
            {
                // "talk 2" with multiple NPCs — ambiguous
                var names = string.Join(", ", npcsHere.Select(n => n.Name));
                return new CommandResult(false, $"Talk to whom? You see: {names}");
            }
            else
            {
                var names = string.Join(", ", npcsHere.Select(n => n.Name));
                return new CommandResult(false, $"Talk to whom? You see: {names}");
            }
        }
        else
        {
            targetName = string.Join(" ", nameArgs).ToLowerInvariant();
        }

        // Find the NPC
        var npc = state.FindNpcInRoom(targetName);

        if (npc is null)
        {
            return new CommandResult(false, $"You don't see '{targetName}' here.");
        }

        // Find available dialogue
        var availableLines = npc.Dialogue
            .Where(d => d.RequiresFlag is null || state.Flags.Contains(d.RequiresFlag))
            .Where(d => d.RequiresItem is null || state.Inventory.Contains(d.RequiresItem))
            .Where(d => d.Repeatable || !state.UsedDialogue.Contains(d.Id))
            .ToList();

        if (availableLines.Count == 0)
        {
            return new CommandResult(true, $"{npc.Name} has nothing more to say.");
        }

        // Categorize available lines:
        // - Story beats: no label, non-repeatable — fire automatically first
        // - Choices: have a label — player picks from menu
        // - Fallbacks: no label, repeatable — fire only when no choices remain
        var storyBeat = availableLines.FirstOrDefault(d => d.Label is null && !d.Repeatable);
        var labeledLines = availableLines.Where(d => d.Label is not null).ToList();
        var fallback = availableLines.FirstOrDefault(d => d.Label is null && d.Repeatable);

        DialogueLine line;

        if (storyBeat is not null)
        {
            // Non-repeatable auto line — story progression, fires first
            line = storyBeat;
        }
        else if (labeledLines.Count > 1 && choiceIndex is null)
        {
            // Multiple choices, no selection — show menu, don't advance turn
            var menu = $"{npc.Name} looks at you expectantly.\n";
            for (int i = 0; i < labeledLines.Count; i++)
            {
                menu += $"  {i + 1}. {labeledLines[i].Label}\n";
            }
            menu += $"(Use: talk {npc.Id} <number>)";
            return new CommandResult(false, menu);
        }
        else if (choiceIndex is not null && labeledLines.Count > 0)
        {
            // Player chose a specific option
            if (choiceIndex.Value < 1 || choiceIndex.Value > labeledLines.Count)
            {
                return new CommandResult(false, $"Choose a number between 1 and {labeledLines.Count}.");
            }
            line = labeledLines[choiceIndex.Value - 1];
        }
        else if (labeledLines.Count == 1)
        {
            // Single labeled line — execute it directly
            line = labeledLines[0];
        }
        else if (fallback is not null)
        {
            // Repeatable fallback — idle dialogue
            line = fallback;
        }
        else
        {
            return new CommandResult(true, $"{npc.Name} has nothing more to say.");
        }

        // Apply effects
        if (line.SetsFlag is not null)
        {
            state.Flags.Add(line.SetsFlag);
        }

        if (line.GivesCredits.HasValue)
        {
            state.Credits += line.GivesCredits.Value;
        }

        if (line.GivesItem is not null && !state.Inventory.Contains(line.GivesItem))
        {
            state.Inventory.Add(line.GivesItem);
        }

        if (line.RemovesItem is not null)
        {
            state.Inventory.Remove(line.RemovesItem);
        }

        if (!line.Repeatable)
        {
            state.UsedDialogue.Add(line.Id);
        }

        return new CommandResult(true, $"{npc.Name}: \"{line.Text}\"");
    }
}
