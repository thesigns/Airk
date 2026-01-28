using Airk.State;

namespace Airk.Commands;

public sealed class QuitCommand : ICommand
{
    public string Name => "quit";
    public string[] Aliases => ["exit", "q"];
    public string Description => "Exit the game";

    public CommandResult Execute(GameState state, string[] args)
    {
        return new CommandResult(true, "Goodbye, runner.", ShouldQuit: true);
    }
}
