using Airk.State;

namespace Airk.Commands;

public sealed class InventoryCommand : ICommand
{
    public string Name => "inventory";
    public string[] Aliases => ["i", "inv"];
    public string Description => "List items you're carrying";

    public CommandResult Execute(GameState state, string[] args)
    {
        if (state.Inventory.Count == 0)
        {
            return new CommandResult(true, "You're not carrying anything.");
        }

        var items = string.Join(", ", state.Inventory);
        return new CommandResult(true, $"You're carrying: {items}");
    }
}
