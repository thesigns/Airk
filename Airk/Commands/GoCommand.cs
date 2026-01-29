using Airk.State;

namespace Airk.Commands;

public sealed class GoCommand : ICommand
{
    public string Name => "go";
    public string[] Aliases => ["move", "walk", "n", "s", "e", "w", "north", "south", "east", "west"];
    public string Description => "Move in a direction (north, south, east, west)";

    public CommandResult Execute(GameState state, string[] args)
    {
        string direction;

        if (args.Length == 0)
        {
            return new CommandResult(false, "Go where? Specify a direction (north, south, east, west).");
        }

        direction = args[0].ToLowerInvariant();

        // Handle shorthand directions
        direction = direction switch
        {
            "n" => "north",
            "s" => "south",
            "e" => "east",
            "w" => "west",
            _ => direction
        };

        var room = state.Rooms[state.CurrentRoomId];

        if (!room.Exits.TryGetValue(direction, out var targetRoomId))
        {
            return new CommandResult(false, $"You can't go {direction} from here.");
        }

        if (room.GatedExits.TryGetValue(direction, out var gate) &&
            !state.Flags.Contains(gate.RequiresFlag))
        {
            return new CommandResult(false, gate.FailureMessage);
        }

        // Check exit costs (e.g. metro entry fee)
        var exitCost = 0;
        if (room.ExitCosts.TryGetValue(direction, out var cost))
        {
            exitCost = cost;
        }

        if (exitCost > 0)
        {
            if (state.Credits < exitCost)
            {
                return new CommandResult(false,
                    $"The turnstile flashes red. Insufficient credits. Entry fee: {exitCost} credits. You have {state.Credits} credits.");
            }
            state.Credits -= exitCost;
        }

        state.CurrentRoomId = targetRoomId;
        var newRoom = state.Rooms[targetRoomId];
        newRoom.Visited = true;

        var message = exitCost > 0
            ? $"The turnstile deducts {exitCost} credits as you pass through.\n\nYou go {direction}."
            : $"You go {direction}.";

        return new CommandResult(true, message, ShowDescription: true);
    }
}
