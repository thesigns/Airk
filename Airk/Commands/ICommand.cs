using Airk.State;

namespace Airk.Commands;

public sealed record CommandResult(bool Success, string Message, bool ShouldQuit = false, bool ShowDescription = false);

public interface ICommand
{
    string Name { get; }
    string[] Aliases { get; }
    string Description { get; }
    CommandResult Execute(GameState state, string[] args);
}
