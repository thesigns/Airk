using Airk.State;

namespace Airk.Commands;

public sealed class UseCommand : ICommand
{
    public string Name => "use";
    public string[] Aliases => ["activate"];
    public string Description => "Use an item from your inventory";

    public CommandResult Execute(GameState state, string[] args)
    {
        if (args.Length == 0)
        {
            return new CommandResult(false, "Use what?");
        }

        var itemName = string.Join(" ", args).ToLowerInvariant();

        if (!state.Inventory.Contains(itemName))
        {
            return new CommandResult(false, $"You don't have a '{itemName}'.");
        }

        return itemName switch
        {
            "datapad" => UseDatapad(state),
            "transit-map" => UseTransitMap(),
            "package" => UsePackage(state),
            "neural-interface" => new CommandResult(false,
                "The neural interface is a specialized tool. You'd need someone with expertise to make use of it."),
            _ => new CommandResult(false, $"You're not sure how to use the {itemName}.")
        };
    }

    private static CommandResult UseDatapad(GameState state)
    {
        if (!state.Flags.Contains("read_datapad"))
        {
            state.Flags.Add("read_datapad");
            return new CommandResult(true,
                "You power on the cracked datapad. Through the static, you make out fragments: " +
                "'...Kira... underground clinic... Sector 7 platform... memory reconstruction...' " +
                "The rest is corrupted. But the name 'Kira' sticks in your mind.");
        }

        return new CommandResult(true,
            "The datapad still shows the same corrupted data about Kira and an underground clinic near the Sector 7 platform.");
    }

    private static CommandResult UseTransitMap()
    {
        return new CommandResult(true,
            "You unfold the transit map. Lines and station codes:\n" +
            "  Red Line: Sector 7 [s7] --- Outer Rim [rim]\n" +
            "  Blue Line: Corp District - Central Hub - Docklands (SERVICE SUSPENDED)\n" +
            "Use 'travel <code>' at a metro platform to ride.");
    }

    private static CommandResult UsePackage(GameState state)
    {
        if (state.Flags.Contains("opened_package"))
        {
            return new CommandResult(false,
                "The package is already open. Inside is a neural suppressor module with NeoCortex Corp markings.");
        }

        return new CommandResult(false,
            "Chrome told you not to open it. But if you really want to... try 'open package'.");
    }
}
