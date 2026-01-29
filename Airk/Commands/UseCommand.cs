using Airk.State;

namespace Airk.Commands;

public sealed class UseCommand : ICommand
{
    public string Name => "use";
    public string[] Aliases => ["activate", "read"];
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
            "package" => new CommandResult(false, "Chrome told you not to open it. Better deliver it as promised."),
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
            "You unfold the transit map. Lines branch out from the central hub:\n" +
            "  Red Line: Neon Boulevard - Sector 7 Platform - Outer Rim\n" +
            "  Blue Line: Corp District - Central Hub - Docklands\n" +
            "You are near the Neon Boulevard station.");
    }
}
