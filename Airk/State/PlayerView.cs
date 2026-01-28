namespace Airk.State;

public sealed record PlayerView(
    int Turn,
    string Room,
    string Description,
    IReadOnlyList<string> Items,
    IReadOnlyList<string> Exits,
    IReadOnlyList<string> People,
    IReadOnlyList<string> Inventory,
    int Health,
    int Credits,
    string? Message
);
