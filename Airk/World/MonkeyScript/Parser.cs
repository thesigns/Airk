namespace Airk.World.MonkeyScript;

public static class Parser
{
    public static ScriptNode Parse(string context, List<Token> tokens)
    {
        int pos = 0;
        var result = ParseScript(context, tokens, ref pos);

        if (pos < tokens.Count)
            throw new WorldLoadException(
                $"{context}: unexpected token at position {pos} in MonkeyScript.");

        return result;
    }

    // script := statement ('>' statement)*
    private static ScriptNode ParseScript(string context, List<Token> tokens, ref int pos)
    {
        var statements = new List<ScriptNode> { ParseStatement(context, tokens, ref pos) };

        while (pos < tokens.Count && tokens[pos].Type == TokenType.Arrow)
        {
            pos++; // skip >
            statements.Add(ParseStatement(context, tokens, ref pos));
        }

        return statements.Count == 1 ? statements[0] : new SequenceNode(statements);
    }

    // statement := expr '?' expr ':' expr | expr
    private static ScriptNode ParseStatement(string context, List<Token> tokens, ref int pos)
    {
        var expr = ParseExpr(context, tokens, ref pos);

        if (pos < tokens.Count && tokens[pos].Type == TokenType.QuestionMark)
        {
            pos++; // skip ?
            var onTrue = ParseExpr(context, tokens, ref pos);

            if (pos >= tokens.Count || tokens[pos].Type != TokenType.Colon)
                throw new WorldLoadException(
                    $"{context}: expected ':' after '?' true-branch in MonkeyScript.");

            pos++; // skip :
            var onFalse = ParseExpr(context, tokens, ref pos);

            return new ConditionalNode(expr, onTrue, onFalse);
        }

        return expr;
    }

    // expr := term ('|' term)*
    private static ScriptNode ParseExpr(string context, List<Token> tokens, ref int pos)
    {
        var terms = new List<ScriptNode> { ParseTerm(context, tokens, ref pos) };

        while (pos < tokens.Count && tokens[pos].Type == TokenType.Pipe)
        {
            pos++; // skip |
            terms.Add(ParseTerm(context, tokens, ref pos));
        }

        return terms.Count == 1 ? terms[0] : new OrNode(terms);
    }

    // term := action+
    private static ScriptNode ParseTerm(string context, List<Token> tokens, ref int pos)
    {
        if (pos >= tokens.Count || tokens[pos].Type != TokenType.Action)
            throw new WorldLoadException(
                $"{context}: expected @action in MonkeyScript at token {pos}.");

        var actions = new List<ScriptNode>();

        while (pos < tokens.Count && tokens[pos].Type == TokenType.Action)
        {
            var token = tokens[pos];
            actions.Add(new ActionNode(token.Name!, token.Argument!));
            pos++;
        }

        return actions.Count == 1 ? actions[0] : new AndNode(actions);
    }
}
