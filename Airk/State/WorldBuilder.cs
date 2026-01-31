namespace Airk.State;

public static class WorldBuilder
{
    private const string SectorId = "test";

    public static GameState CreateNewGame()
    {
        var state = new GameState();

        var assetsPath = GetAssetsPath();
        var rooms = WorldLoader.LoadSector(SectorId, assetsPath);

        foreach (var (id, room) in rooms)
            state.Rooms[id] = room;

        state.CurrentRoomId = rooms.Keys.Order().First();
        state.LastMessage = "You wake up in a dark alley. Your head pounds. You don't remember how you got here.";

        return state;
    }

    public static void ReloadScripts(GameState state)
    {
        WorldLoader.ReloadScripts(state.Rooms, SectorId, GetAssetsPath());
    }

    private static string GetAssetsPath() =>
        Path.Combine(AppContext.BaseDirectory, "Assets", "World");
}
