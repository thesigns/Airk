namespace Airk.World;

public sealed class Npc
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string RoomId { get; init; }
    public List<DialogueLine> Dialogue { get; init; } = new();
}

public sealed class DialogueLine
{
    public required string Id { get; init; }
    public required string Text { get; init; }
    public string? Label { get; init; }
    public string? RequiresFlag { get; init; }
    public string? RequiresItem { get; init; }
    public string? SetsFlag { get; init; }
    public int? GivesCredits { get; init; }
    public string? GivesItem { get; init; }
    public string? RemovesItem { get; init; }
    public bool Repeatable { get; init; } = true;
}
