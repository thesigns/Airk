using Airk.State;

namespace Airk.Commands;

public sealed class PayCommand : ICommand
{
    public string Name => "pay";
    public string[] Aliases => ["buy"];
    public string Description => "Pay for something";

    public CommandResult Execute(GameState state, string[] args)
    {
        if (state.CurrentRoomId != "metro")
        {
            return new CommandResult(false, "There's nothing to pay for here.");
        }

        if (state.Flags.Contains("metro_paid"))
        {
            return new CommandResult(false, "You've already paid the fare.");
        }

        if (state.Credits < 10)
        {
            return new CommandResult(false,
                $"You don't have enough credits. The fare is 10 credits and you have {state.Credits}.");
        }

        state.Credits -= 10;
        state.Flags.Add("metro_paid");
        return new CommandResult(true,
            "You feed 10 credits into the turnstile. It clicks open, granting access to the platform.");
    }
}
