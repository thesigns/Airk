using Airk.State;

namespace Airk.Commands;

public sealed class LookCommand : ICommand
{
    private static readonly Dictionary<string, string> ItemDescriptions = new()
    {
        ["credstick"] = "A thin plastic stick with a dim LED. Contains 5 credits.",
        ["datapad"] = "A cracked datapad. The screen flickers with corrupted data about someone named 'Kira'.",
        ["transit-map"] = "A worn paper map of the Night City metro system. Lines crisscross in a confusing web.",
        ["package"] = "A small sealed package wrapped in black plastic. Chrome told you not to open it."
    };

    public string Name => "look";
    public string[] Aliases => ["l", "examine", "x"];
    public string Description => "Look around or examine an item";

    public CommandResult Execute(GameState state, string[] args)
    {
        if (args.Length == 0)
        {
            return new CommandResult(true, "You look around.");
        }

        var itemName = string.Join(" ", args).ToLowerInvariant();
        var room = state.Rooms[state.CurrentRoomId];

        // Check room items
        if (room.Items.Contains(itemName))
        {
            if (ItemDescriptions.TryGetValue(itemName, out var desc))
            {
                return new CommandResult(true, desc);
            }
            return new CommandResult(true, $"You see a {itemName}.");
        }

        // Check inventory
        if (state.Inventory.Contains(itemName))
        {
            if (ItemDescriptions.TryGetValue(itemName, out var desc))
            {
                return new CommandResult(true, desc);
            }
            return new CommandResult(true, $"You have a {itemName}.");
        }

        // Check NPCs in room
        var npc = state.Npcs.Values.FirstOrDefault(n =>
            n.RoomId == state.CurrentRoomId &&
            (n.Id.Equals(itemName, StringComparison.OrdinalIgnoreCase) ||
             n.Name.Contains(itemName, StringComparison.OrdinalIgnoreCase)));

        if (npc is not null)
        {
            return new CommandResult(true, npc.Description);
        }

        return new CommandResult(false, $"You don't see any '{itemName}' here.");
    }
}
