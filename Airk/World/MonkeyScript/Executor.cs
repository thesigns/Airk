using Airk.Commands;
using Airk.State;

namespace Airk.World.MonkeyScript;

public static class Executor
{
    public static CommandResult Execute(GameState state, ScriptNode script, string direction)
    {
        var ctx = new ExecutionContext(state);
        ctx.Evaluate(script);

        if (ctx.MovedTo is null && ctx.Messages.Count == 0)
            return new CommandResult(false, $"You can't go {direction} from here.");

        var message = string.Join("\n", ctx.Messages);
        return new CommandResult(
            Success: ctx.MovedTo is not null,
            Message: message,
            ShowDescription: ctx.MovedTo is not null);
    }

    private sealed class ExecutionContext(GameState state)
    {
        public string? MovedTo { get; private set; }
        public List<string> Messages { get; } = [];

        public bool Evaluate(ScriptNode node) => node switch
        {
            ActionNode action => ExecuteAction(action),
            AndNode and => EvaluateAnd(and),
            OrNode or => EvaluateOr(or),
            ConditionalNode cond => EvaluateConditional(cond),
            SequenceNode seq => EvaluateSequence(seq),
            _ => throw new InvalidOperationException($"Unknown node type: {node.GetType().Name}")
        };

        private bool EvaluateAnd(AndNode node)
        {
            foreach (var child in node.Children)
            {
                if (!Evaluate(child))
                    return false;
            }
            return true;
        }

        private bool EvaluateOr(OrNode node)
        {
            foreach (var child in node.Children)
            {
                if (Evaluate(child))
                    return true;
            }
            return false;
        }

        private bool EvaluateConditional(ConditionalNode node)
        {
            return Evaluate(node.Condition)
                ? Evaluate(node.OnTrue)
                : Evaluate(node.OnFalse);
        }

        private bool EvaluateSequence(SequenceNode node)
        {
            bool last = false;
            foreach (var stmt in node.Statements)
                last = Evaluate(stmt);
            return last;
        }

        private bool ExecuteAction(ActionNode action) => action.Name switch
        {
            "go" => ActionGo(action.Argument),
            "msg" => ActionMsg(action.Argument),
            "pay" => ActionPay(action.Argument),
            _ => throw new InvalidOperationException($"Unknown MonkeyScript function: @{action.Name}")
        };

        private bool ActionGo(string globalRoomId)
        {
            state.CurrentRoomId = globalRoomId;
            state.Rooms[globalRoomId].Visited = true;
            MovedTo = globalRoomId;
            return true;
        }

        private bool ActionMsg(string text)
        {
            Messages.Add(text);
            return true;
        }

        private bool ActionPay(string amountStr)
        {
            if (!int.TryParse(amountStr, out var amount))
                throw new InvalidOperationException($"@pay argument is not a number: '{amountStr}'");

            if (state.Credits < amount)
                return false;

            state.Credits -= amount;
            return true;
        }
    }
}
