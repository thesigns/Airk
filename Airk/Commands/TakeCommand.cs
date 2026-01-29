using Airk.State;

namespace Airk.Commands;

public sealed class TakeCommand : ICommand
{
    public string Name => "take";
    public string[] Aliases => ["get", "grab", "pick"];
    public string Description => "Pick up an item";

    public CommandResult Execute(GameState state, string[] args)
    {
        if (args.Length == 0)
        {
            return new CommandResult(false, "Take what?");
        }

        var itemName = string.Join(" ", args).ToLowerInvariant();
        var room = state.Rooms[state.CurrentRoomId];

        if (!room.Items.Contains(itemName))
        {
            return new CommandResult(false, $"There's no '{itemName}' here to take.");
        }

        room.Items.Remove(itemName);
        state.Inventory.Add(itemName);

        // Special handling for credstick - it dissolves after transferring credits
        if (itemName == "credstick")
        {
            state.Inventory.Remove(itemName);
            state.Credits += 15;
            return new CommandResult(true, "You pick up the credstick. It transfers 15 credits to your account before dissolving.");
        }

        return new CommandResult(true, $"You take the {itemName}.");
    }
}
