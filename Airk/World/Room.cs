namespace Airk.World;

public sealed class Room
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string ShortDescription { get; init; }
    public required string Description { get; init; }
    public Dictionary<string, string> Exits { get; init; } = new();
    public Dictionary<string, ExitGate> GatedExits { get; init; } = new();
    public List<string> Items { get; init; } = new();
    public Dictionary<string, string> Readables { get; init; } = new();
    public bool Visited { get; set; }
}

public sealed class ExitGate
{
    public required string RequiresFlag { get; init; }
    public required string FailureMessage { get; init; }
}
