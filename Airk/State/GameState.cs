using Airk.World;

namespace Airk.State;

public sealed class GameState
{
    public int TurnNumber { get; set; } = 1;
    public string CurrentRoomId { get; set; } = "alley";
    public List<string> Inventory { get; set; } = new();
    public int Health { get; set; } = 100;
    public int Credits { get; set; } = 0;
    public Dictionary<string, Room> Rooms { get; set; } = new();
    public Dictionary<string, Npc> Npcs { get; set; } = new();
    public HashSet<string> Flags { get; set; } = new();
    public HashSet<string> UsedDialogue { get; set; } = new();
    public Dictionary<string, MetroStation> MetroStations { get; set; } = new();
    public string? LastMessage { get; set; }

    public Npc? FindNpcInRoom(string input)
    {
        return Npcs.Values.FirstOrDefault(n =>
            n.RoomId == CurrentRoomId &&
            (n.Id.Equals(input, StringComparison.OrdinalIgnoreCase) ||
             n.Name.Contains(input, StringComparison.OrdinalIgnoreCase)));
    }

    public PlayerView Project()
    {
        var room = Rooms[CurrentRoomId];
        var npcsHere = Npcs.Values
            .Where(n => n.RoomId == CurrentRoomId)
            .Select(n => n.Name)
            .ToList();

        return new PlayerView(
            Turn: TurnNumber,
            Room: room.Name,
            Description: room.Description,
            Items: room.Items.ToList(),
            Exits: room.Exits.Keys.ToList(),
            People: npcsHere,
            Inventory: Inventory.ToList(),
            Health: Health,
            Credits: Credits,
            Message: LastMessage
        );
    }
}
