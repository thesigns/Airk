namespace Airk.World.MonkeyScript;

public static class Tokenizer
{
    public static List<Token> Tokenize(string context, string input)
    {
        var tokens = new List<Token>();
        int i = 0;

        while (i < input.Length)
        {
            var ch = input[i];

            if (char.IsWhiteSpace(ch))
            {
                i++;
                continue;
            }

            switch (ch)
            {
                case '?':
                    tokens.Add(new Token(TokenType.QuestionMark));
                    i++;
                    break;
                case ':':
                    tokens.Add(new Token(TokenType.Colon));
                    i++;
                    break;
                case '|':
                    tokens.Add(new Token(TokenType.Pipe));
                    i++;
                    break;
                case '>':
                    tokens.Add(new Token(TokenType.Arrow));
                    i++;
                    break;
                case '@':
                    i++;
                    tokens.Add(ReadAction(context, input, ref i));
                    break;
                default:
                    throw new WorldLoadException(
                        $"{context}: unexpected character '{ch}' in MonkeyScript at position {i}.");
            }
        }

        return tokens;
    }

    private static Token ReadAction(string context, string input, ref int i)
    {
        // Read function name
        int nameStart = i;
        while (i < input.Length && input[i] != '(')
        {
            if (!char.IsLetterOrDigit(input[i]) && input[i] != '_')
                throw new WorldLoadException(
                    $"{context}: invalid character '{input[i]}' in function name at position {i}.");
            i++;
        }

        if (i >= input.Length)
            throw new WorldLoadException(
                $"{context}: expected '(' after function name at position {i}.");

        var name = input[nameStart..i];
        if (name.Length == 0)
            throw new WorldLoadException(
                $"{context}: empty function name at position {nameStart}.");

        // Skip '('
        i++;

        // Read argument until ')'
        int argStart = i;
        while (i < input.Length && input[i] != ')')
            i++;

        if (i >= input.Length)
            throw new WorldLoadException(
                $"{context}: unclosed parenthesis for @{name} at position {argStart}.");

        var argument = input[argStart..i].Trim();

        // Skip ')'
        i++;

        return new Token(TokenType.Action, name.ToLowerInvariant(), argument);
    }
}
