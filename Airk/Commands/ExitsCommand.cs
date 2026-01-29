using Airk.State;

namespace Airk.Commands;

public sealed class ExitsCommand : ICommand
{
    public string Name => "exits";
    public string[] Aliases => [];
    public string Description => "List all exits with descriptions";

    public CommandResult Execute(GameState state, string[] args)
    {
        var room = state.Rooms[state.CurrentRoomId];

        if (room.Exits.Count == 0)
        {
            return new CommandResult(true, "There are no exits here.");
        }

        var lines = room.Exits
            .Select(e =>
            {
                var target = state.Rooms[e.Value];
                return $"  {e.Key}: {target.ShortDescription}";
            })
            .ToList();

        return new CommandResult(true, string.Join("\n", lines));
    }
}
