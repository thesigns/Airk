using Airk.State;

namespace Airk.Commands;

public sealed class GiveCommand : ICommand
{
    public string Name => "give";
    public string[] Aliases => ["hand", "offer"];
    public string Description => "Give an item to someone (give <item> to <person>)";

    public CommandResult Execute(GameState state, string[] args)
    {
        if (args.Length == 0)
        {
            return new CommandResult(false, "Give what to whom? (Usage: give <item> to <person>)");
        }

        var input = string.Join(" ", args).ToLowerInvariant();
        var toIndex = input.LastIndexOf(" to ");

        if (toIndex < 0)
        {
            return new CommandResult(false, "Give what to whom? (Usage: give <item> to <person>)");
        }

        var itemName = input[..toIndex].Trim();
        var npcName = input[(toIndex + 4)..].Trim();

        if (string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(npcName))
        {
            return new CommandResult(false, "Give what to whom? (Usage: give <item> to <person>)");
        }

        if (!state.Inventory.Contains(itemName))
        {
            return new CommandResult(false, $"You don't have a '{itemName}'.");
        }

        var npc = state.FindNpcInRoom(npcName);

        if (npc is null)
        {
            return new CommandResult(false, $"There's no one called '{npcName}' here.");
        }

        return (npc.Id, itemName) switch
        {
            ("kira", "neural-interface") => GiveNeuralInterfaceToKira(state),
            _ => new CommandResult(false, $"{npc.Name} doesn't seem to want the {itemName}.")
        };
    }

    private static CommandResult GiveNeuralInterfaceToKira(GameState state)
    {
        if (!state.Flags.Contains("memory_restored"))
        {
            return new CommandResult(false,
                "Kira glances at the neural interface. \"Not yet. We'll need that later, once I can see what we're working with.\"");
        }

        if (state.Flags.Contains("deep_scan_done"))
        {
            return new CommandResult(false,
                "Kira shakes her head. \"I've already gotten everything I can from the deep scan. Keep it as a spare.\"");
        }

        state.Inventory.Remove("neural-interface");
        state.Flags.Add("deep_scan_done");
        return new CommandResult(true,
            "Kira takes the neural interface and connects it to her workbench. " +
            "\"This will let me go deeper into your neural pathways. Hold still...\"\n\n" +
            "Fragments cascade through your mind: a white corridor, a door marked 'ICARUS-7', " +
            "your own hands typing at a terminal. You were a researcher. You worked on Project Icarus. " +
            "And then -- a face. Your supervisor. He ordered the wipe when you tried to expose " +
            "what Icarus was really doing: mass neural suppression targeting the entire district.\n\n" +
            "Kira disconnects the interface. \"Now you know who you are. " +
            "The question is -- what are you going to do about it?\"");
    }
}
