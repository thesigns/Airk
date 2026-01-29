using Airk.State;

namespace Airk.Commands;

public sealed class OpenCommand : ICommand
{
    public string Name => "open";
    public string[] Aliases => ["unwrap"];
    public string Description => "Open something";

    public CommandResult Execute(GameState state, string[] args)
    {
        if (args.Length == 0)
        {
            return new CommandResult(false, "Open what?");
        }

        var itemName = string.Join(" ", args).ToLowerInvariant();

        if (!state.Inventory.Contains(itemName) &&
            !state.Rooms[state.CurrentRoomId].Items.Contains(itemName))
        {
            return new CommandResult(false, $"You don't see any '{itemName}' to open.");
        }

        return itemName switch
        {
            "package" => OpenPackage(state),
            "datapad" => new CommandResult(false, "The datapad is already accessible. Try 'use datapad'."),
            _ => new CommandResult(false, $"You can't open the {itemName}.")
        };
    }

    private static CommandResult OpenPackage(GameState state)
    {
        if (!state.Inventory.Contains("package"))
        {
            return new CommandResult(false, "You need to be holding the package to open it.");
        }

        if (state.Flags.Contains("opened_package"))
        {
            return new CommandResult(false, "The package is already open. Inside is a neural suppressor module with NeoCortex Corp markings.");
        }

        state.Flags.Add("opened_package");
        return new CommandResult(true,
            "You tear open the black plastic wrapping. Inside is a compact device with " +
            "a cluster of fine neural probes and a NeoCortex Corp logo etched into the casing. " +
            "A label reads 'NS-7 Neural Suppressor - Memory Targeting Module'. " +
            "This is the kind of hardware used to wipe people's memories. " +
            "Chrome told you not to open it. Now you know why.");
    }
}
