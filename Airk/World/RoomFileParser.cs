using System.Text.RegularExpressions;

namespace Airk.World;

public static partial class RoomFileParser
{
    [GeneratedRegex(@"^\*\*(\w+):\*\*(.*)$")]
    private static partial Regex FieldPattern();

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
                    throw new InvalidOperationException(
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
}
