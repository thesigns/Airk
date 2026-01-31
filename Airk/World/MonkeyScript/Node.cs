namespace Airk.World.MonkeyScript;

public abstract record ScriptNode;

public sealed record ActionNode(string Name, string Argument) : ScriptNode;

public sealed record AndNode(List<ScriptNode> Children) : ScriptNode;

public sealed record OrNode(List<ScriptNode> Children) : ScriptNode;

public sealed record ConditionalNode(
    ScriptNode Condition,
    ScriptNode OnTrue,
    ScriptNode OnFalse) : ScriptNode;

public sealed record SequenceNode(List<ScriptNode> Statements) : ScriptNode;
