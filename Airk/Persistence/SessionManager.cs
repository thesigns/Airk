using System.Text.Json;
using Airk.State;

namespace Airk.Persistence;

public static class SessionManager
{
    private const string SessionFile = "session.json";

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static GameState Load()
    {
        if (!File.Exists(SessionFile))
        {
            return WorldBuilder.CreateNewGame();
        }

        var json = File.ReadAllText(SessionFile);
        return JsonSerializer.Deserialize<GameState>(json, Options)
               ?? WorldBuilder.CreateNewGame();
    }

    public static void Save(GameState state)
    {
        var json = JsonSerializer.Serialize(state, Options);
        File.WriteAllText(SessionFile, json);
    }

    public static void Delete()
    {
        if (File.Exists(SessionFile))
        {
            File.Delete(SessionFile);
        }
    }
}
