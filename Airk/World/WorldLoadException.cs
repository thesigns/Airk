namespace Airk.World;

public sealed class WorldLoadException : Exception
{
    public WorldLoadException(string message) : base(message) { }
}
