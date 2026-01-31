namespace Airk.World;

public static class MapParser
{
    public static Dictionary<string, Room> Parse(string sectorId, string mapText)
    {
        var lines = GetMapLines(mapText);
        if (lines.Count == 0)
            throw new WorldLoadException($"Sector '{sectorId}': map is empty.");

        var grid = ScanRooms(lines);
        ValidateNoDuplicates(sectorId, grid);

        var rooms = new Dictionary<string, Room>();
        var exits = new Dictionary<string, Dictionary<string, string>>();

        // Initialize exit dictionaries
        foreach (var (_, localId) in grid.Values)
            exits[$"{sectorId}_{localId}"] = new Dictionary<string, string>();

        // Scan horizontal connectors (even rows, col = gx*4 + 3)
        foreach (var ((gx, gy), (_, localId)) in grid)
        {
            var nextPos = (gx + 1, gy);
            if (!grid.TryGetValue(nextPos, out var neighbor)) continue;

            int row = gy * 2;
            int col = gx * 4 + 3;
            if (CharAt(lines, row, col) != '+') continue;

            var leftId = $"{sectorId}_{localId}";
            var rightId = $"{sectorId}_{neighbor.localId}";
            exits[leftId]["east"] = rightId;
            exits[rightId]["west"] = leftId;
        }

        // Scan vertical connectors (odd rows, col = gx*4 + 1)
        foreach (var ((gx, gy), (_, localId)) in grid)
        {
            var belowPos = (gx, gy + 1);
            if (!grid.TryGetValue(belowPos, out var neighbor)) continue;

            int row = gy * 2 + 1;
            int col = gx * 4 + 1;
            if (CharAt(lines, row, col) != '+') continue;

            var topId = $"{sectorId}_{localId}";
            var bottomId = $"{sectorId}_{neighbor.localId}";
            exits[topId]["south"] = bottomId;
            exits[bottomId]["north"] = topId;
        }

        // Build Room objects
        foreach (var (_, localId) in grid.Values)
        {
            var globalId = $"{sectorId}_{localId}";
            rooms[globalId] = new Room
            {
                Id = globalId,
                Name = localId,
                ShortDescription = "A room.",
                Description = $"You are in room {localId}.",
                Exits = exits[globalId]
            };
        }

        return rooms;
    }

    private static List<string> GetMapLines(string text)
    {
        // Strip BOM
        if (text.Length > 0 && text[0] == '\uFEFF')
            text = text[1..];

        var lines = new List<string>();
        foreach (var line in text.Split('\n'))
        {
            var stripped = line.TrimEnd('\r');
            if (string.IsNullOrWhiteSpace(stripped))
                break;
            lines.Add(stripped);
        }
        return lines;
    }

    private static Dictionary<(int gx, int gy), (int index, string localId)> ScanRooms(List<string> lines)
    {
        var grid = new Dictionary<(int gx, int gy), (int index, string localId)>();
        int roomIndex = 0;

        for (int gy = 0; gy * 2 < lines.Count; gy++)
        {
            int row = gy * 2;
            var line = lines[row];

            for (int gx = 0; gx * 4 + 2 < line.Length || gx * 4 < line.Length; gx++)
            {
                int col = gx * 4;
                if (col + 2 >= line.Length) break;

                char c0 = line[col];
                char c1 = line[col + 1];
                char c2 = line[col + 2];

                if (char.IsAsciiLetterLower(c0) && char.IsAsciiDigit(c1) && char.IsAsciiDigit(c2))
                {
                    var localId = new string(new[] { c0, c1, c2 });
                    grid[(gx, gy)] = (roomIndex++, localId);
                }
            }
        }

        return grid;
    }

    private static void ValidateNoDuplicates(string sectorId, Dictionary<(int, int), (int, string localId)> grid)
    {
        var seen = new HashSet<string>();
        foreach (var (_, localId) in grid.Values)
        {
            if (!seen.Add(localId))
                throw new WorldLoadException(
                    $"Sector '{sectorId}': duplicate room ID '{localId}'.");
        }
    }

    private static char CharAt(List<string> lines, int row, int col)
    {
        if (row < 0 || row >= lines.Count) return ' ';
        var line = lines[row];
        if (col < 0 || col >= line.Length) return ' ';
        return line[col];
    }
}
