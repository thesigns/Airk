using Airk.State;

namespace Airk.Commands;

public sealed class HelpCommand : ICommand
{
    private readonly Func<IEnumerable<ICommand>> _getCommands;

    public HelpCommand(Func<IEnumerable<ICommand>> getCommands)
    {
        _getCommands = getCommands;
    }

    public string Name => "help";
    public string[] Aliases => ["?", "commands"];
    public string Description => "Show available commands";

    public CommandResult Execute(GameState state, string[] args)
    {
        var lines = new List<string> { "Available commands:" };

        foreach (var cmd in _getCommands())
        {
            var aliases = cmd.Aliases.Length > 0 ? $" ({string.Join(", ", cmd.Aliases)})" : "";
            lines.Add($"  {cmd.Name}{aliases} - {cmd.Description}");
        }

        return new CommandResult(true, string.Join("\n", lines));
    }
}
