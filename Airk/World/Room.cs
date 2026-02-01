using System.Text.Json.Serialization;

namespace Airk.World;

public sealed class Room
{
    public required string Id { get; init; }
    public required string Name { get; set; }
    public required string ShortDescription { get; set; }
    public required string Description { get; set; }
    public Dictionary<string, string> Exits { get; init; } = new();
    public Dictionary<string, ExitGate> GatedExits { get; init; } = new();
    public Dictionary<string, int> ExitCosts { get; init; } = new();
    [JsonIgnore]
    public Dictionary<string, string> ExitScripts { get; init; } = new();
    public List<string> Items { get; set; } = new();
    public Dictionary<string, string> Readables { get; init; } = new();
    public bool Visited { get; set; }
}

public sealed class ExitGate
{
    public required string RequiresFlag { get; init; }
    public required string FailureMessage { get; init; }
}
