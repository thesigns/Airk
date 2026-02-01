using Airk.Commands;
using Airk.State;

namespace Airk.Sharpex;

public static class SharpexFunctions
{
    private static GameState _state = null!;
    private static List<string> _messages = [];
    private static string? _movedTo;

    public static CommandResult Execute(GameState state, string script, string direction)
    {
        _state = state;
        _messages = [];
        _movedTo = null;

        Sharpex.Eval(script);

        if (_movedTo is null && _messages.Count == 0)
            return new CommandResult(false, $"You can't go {direction} from here.");

        var message = string.Join("\n", _messages);
        return new CommandResult(
            Success: _movedTo is not null,
            Message: message,
            ShowDescription: _movedTo is not null);
    }

    [Sharpex("go")]
    public static bool Go(string globalRoomId)
    {
        _state.CurrentRoomId = globalRoomId;
        _state.Rooms[globalRoomId].Visited = true;
        _movedTo = globalRoomId;
        return true;
    }

    [Sharpex("msg")]
    public static bool Msg(string text)
    {
        _messages.Add(text);
        return true;
    }

    [Sharpex("pay")]
    public static bool Pay(int amount)
    {
        if (_state.Credits < amount)
            return false;

        _state.Credits -= amount;
        return true;
    }
}
