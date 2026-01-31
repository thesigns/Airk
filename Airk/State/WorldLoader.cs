using Airk.World;

namespace Airk.State;

public static class WorldLoader
{
    public static Dictionary<string, Room> LoadSector(string sectorId, string basePath)
    {
        var mapFile = Path.Combine(basePath, $"{sectorId}.md");
        if (!File.Exists(mapFile))
            throw new FileNotFoundException($"Sector map file not found: {mapFile}");

        var mapMarkdown = File.ReadAllText(mapFile);
        var mapText = ExtractCodeBlock(mapMarkdown, sectorId);
        var rooms = MapParser.Parse(sectorId, mapText);

        // Load room definition files
        foreach (var room in rooms.Values)
        {
            var localId = room.Id[(sectorId.Length + 1)..]; // strip "sectorId_"
            var roomFile = Path.Combine(basePath, $"{sectorId}_{localId}.md");
            if (!File.Exists(roomFile)) continue;

            var roomText = File.ReadAllText(roomFile);
            RoomFileParser.Apply(room, roomText);
        }

        return rooms;
    }

    private static string ExtractCodeBlock(string markdown, string sectorId)
    {
        const string fence = "```";
        var start = markdown.IndexOf(fence, StringComparison.Ordinal);
        if (start < 0)
            throw new InvalidOperationException(
                $"Sector '{sectorId}': no code block found in map file.");

        start += fence.Length;
        // Skip to end of the opening fence line
        var lineEnd = markdown.IndexOf('\n', start);
        if (lineEnd >= 0)
            start = lineEnd + 1;

        var end = markdown.IndexOf(fence, start, StringComparison.Ordinal);
        if (end < 0)
            throw new InvalidOperationException(
                $"Sector '{sectorId}': unclosed code block in map file.");

        return markdown[start..end];
    }
}
