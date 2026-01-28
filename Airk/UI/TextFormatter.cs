using System.Text;

namespace Airk.UI;

public static class TextFormatter
{
    private const int DefaultWidth = 80;

    public static string WordWrap(string? text, int maxWidth = DefaultWidth)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var result = new StringBuilder();
        var paragraphs = text.Split('\n');

        for (int p = 0; p < paragraphs.Length; p++)
        {
            if (p > 0) result.Append('\n');
            WrapParagraph(paragraphs[p], maxWidth, result);
        }

        return result.ToString();
    }

    private static void WrapParagraph(string paragraph, int maxWidth, StringBuilder result)
    {
        if (string.IsNullOrEmpty(paragraph)) return;

        var words = paragraph.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int lineLength = 0;

        foreach (var word in words)
        {
            if (word.Length > maxWidth)
            {
                if (lineLength > 0)
                {
                    result.Append('\n');
                    lineLength = 0;
                }
                for (int i = 0; i < word.Length; i += maxWidth)
                {
                    if (i > 0) result.Append('\n');
                    result.Append(word.AsSpan(i, Math.Min(maxWidth, word.Length - i)));
                }
                lineLength = word.Length % maxWidth;
            }
            else if (lineLength == 0)
            {
                result.Append(word);
                lineLength = word.Length;
            }
            else if (lineLength + 1 + word.Length <= maxWidth)
            {
                result.Append(' ').Append(word);
                lineLength += 1 + word.Length;
            }
            else
            {
                result.Append('\n').Append(word);
                lineLength = word.Length;
            }
        }
    }
}
