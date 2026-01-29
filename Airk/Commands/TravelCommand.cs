using Airk.State;

namespace Airk.Commands;

public sealed class TravelCommand : ICommand
{
    public string Name => "travel";
    public string[] Aliases => ["ride"];
    public string Description => "Travel to a metro station (travel <code>)";

    public CommandResult Execute(GameState state, string[] args)
    {
        var currentStation = state.MetroStations.Values
            .FirstOrDefault(s => s.PlatformRoomId == state.CurrentRoomId);

        if (currentStation is null)
        {
            return new CommandResult(false, "There's no metro platform here. Find a station first.");
        }

        if (args.Length == 0)
        {
            var destinations = state.MetroStations.Values
                .Where(s => s.Line == currentStation.Line && s.Code != currentStation.Code)
                .ToList();

            if (destinations.Count == 0)
            {
                return new CommandResult(false, "No other stations are reachable from here.");
            }

            var lines = new List<string>
            {
                $"You're at {currentStation.Name} ({currentStation.Line} Line).",
                "Available destinations:"
            };

            foreach (var d in destinations)
            {
                lines.Add($"  {d.Code} - {d.Name}");
            }

            lines.Add("(Use: travel <code>)");
            return new CommandResult(false, string.Join("\n", lines));
        }

        var code = args[0].ToLowerInvariant();

        if (!state.MetroStations.TryGetValue(code, out var target))
        {
            return new CommandResult(false, $"Unknown station code: '{code}'. Use 'travel' to see destinations.");
        }

        if (target.Code == currentStation.Code)
        {
            return new CommandResult(false, "You're already at this station.");
        }

        if (target.Line != currentStation.Line)
        {
            return new CommandResult(false,
                $"{target.Name} is on the {target.Line} Line. You need to transfer at a hub station.");
        }

        state.CurrentRoomId = target.PlatformRoomId;
        state.Rooms[target.PlatformRoomId].Visited = true;

        return new CommandResult(true,
            $"You board the {currentStation.Line} Line train. The doors hiss shut. " +
            $"After a few minutes of rattling through dark tunnels, you arrive at {target.Name}.",
            ShowDescription: true);
    }
}
