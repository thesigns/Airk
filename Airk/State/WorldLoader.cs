using Airk.World;
using Airk.World.MonkeyScript;

namespace Airk.State;

public static class WorldLoader
{
    public static Dictionary<string, Room> LoadSector(string sectorId, string basePath)
    {
        var mapFile = Path.Combine(basePath, $"{sectorId}.md");
        if (!File.Exists(mapFile))
            throw new WorldLoadException($"Sector map file not found: {mapFile}");

        var mapMarkdown = File.ReadAllText(mapFile);
        var mapText = ExtractCodeBlock(mapMarkdown, sectorId);
        var rooms = MapParser.Parse(sectorId, mapText);

        // Pass 1: Load room definition files (names, descriptions)
        var roomTexts = new Dictionary<string, string>();
        foreach (var room in rooms.Values)
        {
            var localId = room.Id[(sectorId.Length + 1)..]; // strip "sectorId_"
            var roomFile = Path.Combine(basePath, $"{sectorId}_{localId}.md");
            if (!File.Exists(roomFile)) continue;

            var roomText = File.ReadAllText(roomFile);
            RoomFileParser.Apply(room, roomText);
            roomTexts[room.Id] = roomText;
        }

        // Pass 2: Parse and validate additional exits (all room names are now final)
        foreach (var (roomId, text) in roomTexts)
        {
            var exits = RoomFileParser.ParseAdditionalExits(roomId, text);
            var room = rooms[roomId];

            foreach (var (direction, localId, expectedName, scriptText) in exits)
            {
                var globalId = $"{sectorId}_{localId}";

                if (!rooms.TryGetValue(globalId, out var target))
                    throw new WorldLoadException(
                        $"Room '{roomId}': additional exit '{direction}' points to '{localId}', which does not exist in sector '{sectorId}'.");

                if (target.Name != expectedName)
                    throw new WorldLoadException(
                        $"Room '{roomId}': additional exit '{direction}' expects name '{expectedName}' for '{localId}', but actual name is '{target.Name}'.");

                room.Exits[direction] = globalId;

                if (scriptText.Length > 0)
                {
                    var tokens = Tokenizer.Tokenize(roomId, scriptText);
                    ResolveGoTokens(tokens, sectorId, rooms, roomId);
                    var ast = Parser.Parse(roomId, tokens);
                    room.ExitScripts[direction] = ast;
                }
            }
        }

        return rooms;
    }

    public static void ReloadScripts(Dictionary<string, Room> rooms, string sectorId, string basePath)
    {
        foreach (var room in rooms.Values)
        {
            var localId = room.Id[(sectorId.Length + 1)..];
            var roomFile = Path.Combine(basePath, $"{sectorId}_{localId}.md");
            if (!File.Exists(roomFile)) continue;

            var text = File.ReadAllText(roomFile);
            var exits = RoomFileParser.ParseAdditionalExits(room.Id, text);

            foreach (var (direction, _, _, scriptText) in exits)
            {
                if (scriptText.Length == 0) continue;

                var tokens = Tokenizer.Tokenize(room.Id, scriptText);
                ResolveGoTokens(tokens, sectorId, rooms, room.Id);
                var ast = Parser.Parse(room.Id, tokens);
                room.ExitScripts[direction] = ast;
            }
        }
    }

    private static void ResolveGoTokens(
        List<Token> tokens, string sectorId, Dictionary<string, Room> rooms, string roomId)
    {
        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (token.Type != TokenType.Action || token.Name != "go") continue;

            var globalId = $"{sectorId}_{token.Argument}";
            if (!rooms.ContainsKey(globalId))
                throw new WorldLoadException(
                    $"Room '{roomId}': @go({token.Argument}) target does not exist in sector '{sectorId}'.");

            tokens[i] = token with { Argument = globalId };
        }
    }

    private static string ExtractCodeBlock(string markdown, string sectorId)
    {
        const string fence = "```";
        var start = markdown.IndexOf(fence, StringComparison.Ordinal);
        if (start < 0)
            throw new WorldLoadException(
                $"Sector '{sectorId}': no code block found in map file.");

        start += fence.Length;
        // Skip to end of the opening fence line
        var lineEnd = markdown.IndexOf('\n', start);
        if (lineEnd >= 0)
            start = lineEnd + 1;

        var end = markdown.IndexOf(fence, start, StringComparison.Ordinal);
        if (end < 0)
            throw new WorldLoadException(
                $"Sector '{sectorId}': unclosed code block in map file.");

        return markdown[start..end];
    }
}
