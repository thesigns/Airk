namespace Airk.State;

public static class WorldBuilder
{
    public static GameState CreateNewGame()
    {
        var state = new GameState();

        var assetsPath = Path.Combine(AppContext.BaseDirectory, "Assets", "World");
        var rooms = WorldLoader.LoadSector("test", assetsPath);

        foreach (var (id, room) in rooms)
            state.Rooms[id] = room;

        state.CurrentRoomId = rooms.Keys.Order().First();
        state.LastMessage = "You wake up in a dark alley. Your head pounds. You don't remember how you got here.";

        return state;
    }
}
