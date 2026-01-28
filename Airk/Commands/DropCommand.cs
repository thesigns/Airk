using Airk.State;

namespace Airk.Commands;

public sealed class DropCommand : ICommand
{
    public string Name => "drop";
    public string[] Aliases => ["put", "discard"];
    public string Description => "Drop an item from your inventory";

    public CommandResult Execute(GameState state, string[] args)
    {
        if (args.Length == 0)
        {
            return new CommandResult(false, "Drop what?");
        }

        var itemName = string.Join(" ", args).ToLowerInvariant();

        if (!state.Inventory.Contains(itemName))
        {
            return new CommandResult(false, $"You don't have a '{itemName}'.");
        }

        state.Inventory.Remove(itemName);
        var room = state.Rooms[state.CurrentRoomId];
        room.Items.Add(itemName);

        return new CommandResult(true, $"You drop the {itemName}.");
    }
}
