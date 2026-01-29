using Airk.State;

namespace Airk.Commands;

public sealed class ReadCommand : ICommand
{
    public string Name => "read";
    public string[] Aliases => [];
    public string Description => "Read a sign, notice, or document";

    public CommandResult Execute(GameState state, string[] args)
    {
        var room = state.Rooms[state.CurrentRoomId];

        if (args.Length == 0)
        {
            if (room.Readables.Count == 0)
            {
                return new CommandResult(false, "There's nothing to read here.");
            }

            if (room.Readables.Count == 1)
            {
                var single = room.Readables.First();
                return new CommandResult(true, single.Value);
            }

            var names = string.Join(", ", room.Readables.Keys);
            return new CommandResult(false, $"You can read: {names}");
        }

        var target = string.Join(" ", args).ToLowerInvariant();

        // Check room readables
        if (room.Readables.TryGetValue(target, out var text))
        {
            return new CommandResult(true, text);
        }

        // Inventory fallback for readable items
        if (state.Inventory.Contains(target))
        {
            return target switch
            {
                "datapad" or "transit-map" => new UseCommand().Execute(state, [target]),
                _ => new CommandResult(false, $"You can't read the {target}.")
            };
        }

        return new CommandResult(false, $"There's nothing called '{target}' to read here.");
    }
}
