using Airk.State;

namespace Airk.Commands;

public sealed class TalkCommand : ICommand
{
    public string Name => "talk";
    public string[] Aliases => ["speak", "chat", "ask"];
    public string Description => "Talk to someone";

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

        // If no target specified and only one NPC, talk to them
        string targetName;
        if (args.Length == 0)
        {
            if (npcsHere.Count == 1)
            {
                targetName = npcsHere[0].Id;
            }
            else
            {
                var names = string.Join(", ", npcsHere.Select(n => n.Name));
                return new CommandResult(false, $"Talk to whom? You see: {names}");
            }
        }
        else
        {
            targetName = string.Join(" ", args).ToLowerInvariant();
        }

        // Find the NPC
        var npc = npcsHere.FirstOrDefault(n =>
            n.Id.Equals(targetName, StringComparison.OrdinalIgnoreCase) ||
            n.Name.Contains(targetName, StringComparison.OrdinalIgnoreCase));

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

        // Get the first available line (priority order)
        var line = availableLines.First();

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
