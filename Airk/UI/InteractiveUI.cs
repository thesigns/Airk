using Airk.State;

namespace Airk.UI;

public sealed class InteractiveUI : IGameUI
{
    public string? ReadCommand()
    {
        Console.Write("\n> ");
        return Console.ReadLine()?.Trim();
    }

    public void DisplayView(PlayerView view)
    {
        Console.WriteLine();
        Console.WriteLine($"=== {view.Room} ===");
        Console.WriteLine();

        if (view.Message is not null)
        {
            Console.WriteLine(view.Message);
            Console.WriteLine();
        }

        Console.WriteLine(view.Description);

        if (view.People.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"People here: {string.Join(", ", view.People)}");
        }

        if (view.Items.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"You see: {string.Join(", ", view.Items)}");
        }

        Console.WriteLine();
        Console.WriteLine($"Exits: {string.Join(", ", view.Exits)}");
        Console.WriteLine($"[Turn {view.Turn} | Health: {view.Health} | Credits: {view.Credits}]");
    }

    public void DisplayError(string message)
    {
        Console.WriteLine($"Error: {message}");
    }
}
