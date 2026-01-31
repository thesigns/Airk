using Airk.World;

namespace Airk.State;

public static class WorldLoader
{
    public static Dictionary<string, Room> LoadSector(string sectorId, string basePath)
    {
        var mapFile = Path.Combine(basePath, $"{sectorId}.txt");
        if (!File.Exists(mapFile))
            throw new FileNotFoundException($"Sector map file not found: {mapFile}");

        var text = File.ReadAllText(mapFile);
        return MapParser.Parse(sectorId, text);
    }
}
