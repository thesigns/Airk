using Airk.Commands;
using Airk.State;

namespace Airk.Engine;

public sealed class CommandProcessor
{
    private readonly Dictionary<string, ICommand> _commands = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<ICommand> _allCommands = new();

    public CommandProcessor()
    {
        // Help command needs access to command list
        var helpCommand = new HelpCommand(() => _allCommands);

        RegisterCommand(new LookCommand());
        RegisterCommand(new GoCommand());
        RegisterCommand(new TakeCommand());
        RegisterCommand(new DropCommand());
        RegisterCommand(new InventoryCommand());
        RegisterCommand(new TalkCommand());
        RegisterCommand(new UseCommand());
        RegisterCommand(new OpenCommand());
        RegisterCommand(new GiveCommand());
        RegisterCommand(new PayCommand());
        RegisterCommand(helpCommand);
        RegisterCommand(new QuitCommand());
    }

    private void RegisterCommand(ICommand command)
    {
        _allCommands.Add(command);
        _commands[command.Name] = command;
        foreach (var alias in command.Aliases)
        {
            _commands[alias] = command;
        }
    }

    public CommandResult Execute(GameState state, string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new CommandResult(false, "What do you want to do?");
        }

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var verb = parts[0];
        var args = parts.Skip(1).ToArray();

        // Handle bare direction commands (e.g., "north" instead of "go north")
        if (IsDirection(verb) && args.Length == 0)
        {
            args = [verb];
            verb = "go";
        }

        if (!_commands.TryGetValue(verb, out var command))
        {
            return new CommandResult(false, $"Unknown command: '{verb}'. Type 'help' for a list of commands.");
        }

        return command.Execute(state, args);
    }

    private static bool IsDirection(string word)
    {
        return word.ToLowerInvariant() is "north" or "south" or "east" or "west" or "n" or "s" or "e" or "w";
    }
}
