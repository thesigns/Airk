using System.Text.RegularExpressions;

namespace Airk.World;

public static partial class RoomFileParser
{
    [GeneratedRegex(@"^\*\*(\w+):\*\*(.*)$")]
    private static partial Regex FieldPattern();

    [GeneratedRegex(@"^-\s+\*\*(\w+):\*\*\s+([a-z]\d{2})\s+\((.+?)\)\s*(.*)$")]
    private static partial Regex ExitEntryPattern();

    private static readonly HashSet<string> ReservedWords = new(StringComparer.OrdinalIgnoreCase)
    {
        // Command names
        "look", "go", "take", "drop", "inventory", "talk", "use", "read",
        "open", "give", "travel", "exits", "help", "quit",
        // Aliases
        "l", "examine", "x", "move", "walk", "get", "grab", "pick",
        "put", "discard", "i", "inv", "speak", "chat", "ask",
        "activate", "hand", "offer", "unwrap", "ride", "?", "commands",
        "exit", "q",
        // Cardinal directions (come from MapParser grid)
        "north", "south", "east", "west", "n", "s", "e", "w"
    };

    public static void Apply(Room room, string fileText)
    {
        // Strip BOM
        if (fileText.Length > 0 && fileText[0] == '\uFEFF')
            fileText = fileText[1..];

        var name = ExtractName(room.Id, fileText);
        if (name is not null)
            room.Name = name;

        var fields = ParseFields(fileText);

        if (fields.TryGetValue("short", out var shortDesc))
            room.ShortDescription = shortDesc;

        if (fields.TryGetValue("description", out var desc))
            room.Description = desc;
    }

    private static string? ExtractName(string roomId, string text)
    {
        string? name = null;
        foreach (var rawLine in text.Split('\n'))
        {
            var line = rawLine.TrimEnd('\r');
            if (line.StartsWith("# "))
            {
                if (name is not null)
                    throw new WorldLoadException(
                        $"Room '{roomId}': multiple H1 headings found.");
                name = line[2..].Trim();
            }
        }
        return name;
    }

    private static Dictionary<string, string> ParseFields(string text)
    {
        var fields = new Dictionary<string, string>();
        var lines = text.Split('\n');
        var regex = FieldPattern();

        string? currentField = null;
        var contentLines = new List<string>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].TrimEnd('\r');
            var match = regex.Match(line);

            if (match.Success)
            {
                // Save previous field
                if (currentField is not null)
                    fields[currentField] = JoinContent(contentLines);

                currentField = match.Groups[1].Value.ToLowerInvariant();
                contentLines.Clear();

                var rest = match.Groups[2].Value.Trim();
                if (rest.Length > 0)
                    contentLines.Add(rest);

                continue;
            }

            if (currentField is not null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    // Empty line ends field
                    fields[currentField] = JoinContent(contentLines);
                    currentField = null;
                    contentLines.Clear();
                }
                else
                {
                    contentLines.Add(line);
                }
            }
        }

        // Save last field
        if (currentField is not null)
            fields[currentField] = JoinContent(contentLines);

        return fields;
    }

    private static string JoinContent(List<string> lines)
    {
        return string.Join("\n", lines).Trim();
    }

    public static List<(string Direction, string LocalId, string ExpectedName, string ScriptText)>
        ParseAdditionalExits(string roomId, string text)
    {
        // Strip BOM
        if (text.Length > 0 && text[0] == '\uFEFF')
            text = text[1..];

        var exits = new List<(string, string, string, string)>();
        var seenDirections = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var lines = text.Split('\n');
        var entryRegex = ExitEntryPattern();
        bool inSection = false;

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd('\r');

            if (!inSection)
            {
                if (line.StartsWith("## ") &&
                    line[3..].Trim().Equals("Additional Exits", StringComparison.OrdinalIgnoreCase))
                {
                    inSection = true;
                }
                continue;
            }

            // Next section or EOF
            if (line.StartsWith("## "))
                break;

            // Skip blank lines within section
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var match = entryRegex.Match(line);
            if (!match.Success)
            {
                throw new WorldLoadException(
                    $"Room '{roomId}': invalid line in Additional Exits section: '{line}'");
            }

            var direction = match.Groups[1].Value.ToLowerInvariant();
            var localId = match.Groups[2].Value;
            var expectedName = match.Groups[3].Value.Trim();
            var scriptText = match.Groups[4].Value.Trim();

            if (ReservedWords.Contains(direction))
            {
                throw new WorldLoadException(
                    $"Room '{roomId}': custom exit direction '{direction}' conflicts with a reserved command.");
            }

            if (!seenDirections.Add(direction))
            {
                throw new WorldLoadException(
                    $"Room '{roomId}': duplicate custom exit direction '{direction}'.");
            }

            exits.Add((direction, localId, expectedName, scriptText));
        }

        return exits;
    }
}
