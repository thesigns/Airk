using System.Text.Json;
using Airk.State;

namespace Airk.UI;

public sealed class BatchUI : IGameUI
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly string? _command;
    private bool _commandRead;

    public BatchUI(string command)
    {
        _command = command;
    }

    public string? ReadCommand()
    {
        if (_commandRead)
        {
            return null;
        }
        _commandRead = true;
        return _command;
    }

    public void DisplayView(PlayerView view)
    {
        var json = JsonSerializer.Serialize(view, Options);
        Console.WriteLine(json);
    }

    public void DisplayError(string message)
    {
        var error = new { error = message };
        var json = JsonSerializer.Serialize(error, Options);
        Console.WriteLine(json);
    }
}
