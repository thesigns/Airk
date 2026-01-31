using Airk.State;

namespace Airk.Commands;

public sealed class LookCommand : ICommand
{
    private static readonly Dictionary<string, string> ItemDescriptions = new()
    {
        ["credstick"] = "A thin plastic stick with a dim LED. Contains 5 credits.",
        ["datapad"] = "A cracked datapad. The screen flickers with corrupted data about someone named 'Kira'.",
        ["transit-map"] = "A worn paper map of the Kreznik metro system. Lines crisscross in a confusing web.",
        ["package"] = "A small sealed package wrapped in black plastic. Chrome told you not to open it. Try 'open package' if you dare.",
        ["cortex-chip"] = "A small, iridescent chip sealed in anti-static packaging. Military markings are partially scratched off.",
        ["neural-interface"] = "A tangle of fine wires and a small processor unit. Used for connecting to neural implants.",
        ["access-card"] = "A battered NeoCortex maintenance access card. The magnetic strip is scratched but intact. Name field reads 'ECHO - MAINT. LEVEL 3'."
    };

    public string Name => "look";
    public string[] Aliases => ["l", "examine", "x"];
    public string Description => "Look around or examine an item";

    private static readonly HashSet<string> Directions = new(StringComparer.OrdinalIgnoreCase)
        { "north", "south", "east", "west", "n", "s", "e", "w" };

    private static string NormalizeDirection(string dir) => dir.ToLowerInvariant() switch
    {
        "n" => "north",
        "s" => "south",
        "e" => "east",
        "w" => "west",
        _ => dir.ToLowerInvariant()
    };

    public CommandResult Execute(GameState state, string[] args)
    {
        if (args.Length == 0)
        {
            return new CommandResult(true, "You look around.", ShowDescription: true);
        }

        var itemName = string.Join(" ", args).ToLowerInvariant();
        var room = state.Rooms[state.CurrentRoomId];

        // Check if looking in a direction
        if (args.Length == 1 && Directions.Contains(args[0]))
        {
            var direction = NormalizeDirection(args[0]);
            if (room.Exits.TryGetValue(direction, out var targetRoomId))
            {
                var targetRoom = state.Rooms[targetRoomId];
                return new CommandResult(true, $"To the {direction}: {targetRoom.ShortDescription}");
            }
            return new CommandResult(true, $"There's nothing noteworthy to the {direction}.");
        }

        // Override descriptions based on state
        if (itemName == "package" && state.Flags.Contains("opened_package"))
        {
            return new CommandResult(true,
                "The torn-open package. Inside is an NS-7 Neural Suppressor with NeoCortex Corp markings. The kind of hardware used to wipe people's memories.");
        }

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
        var npc = state.FindNpcInRoom(itemName);

        if (npc is not null)
        {
            return new CommandResult(true, npc.Description);
        }

        return new CommandResult(false, $"You don't see any '{itemName}' here.");
    }
}
