using System.Text.RegularExpressions;

namespace Airk.World;

public static partial class RoomFileParser
{
    [GeneratedRegex(@"^\*\*(\w+):\*\*(.*)$")]
    private static partial Regex FieldPattern();

    public static void Apply(Room room, string fileText)
    {
        var fields = ParseFields(fileText);

        if (fields.TryGetValue("name", out var name))
            room.Name = name;

        if (fields.TryGetValue("short", out var shortDesc))
            room.ShortDescription = shortDesc;

        if (fields.TryGetValue("description", out var desc))
            room.Description = desc;
    }

    private static Dictionary<string, string> ParseFields(string text)
    {
        // Strip BOM
        if (text.Length > 0 && text[0] == '\uFEFF')
            text = text[1..];

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
