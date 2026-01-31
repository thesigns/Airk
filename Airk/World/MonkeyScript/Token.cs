namespace Airk.World.MonkeyScript;

public enum TokenType
{
    Action,       // @name(arg)
    QuestionMark, // ?
    Colon,        // :
    Pipe,         // |
    Arrow         // >
}

public sealed record Token(TokenType Type, string? Name = null, string? Argument = null);
