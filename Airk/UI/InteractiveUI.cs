using Airk.State;

namespace Airk.UI;

public sealed class InteractiveUI : IGameUI
{
    public string? ReadCommand()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("> ");
        var input = Console.ReadLine()?.Trim();
        Console.ResetColor();
        return input;
    }

    public void DisplayView(PlayerView view)
    {
        Console.WriteLine();
        Console.WriteLine($"=== {view.Room} ===");

        if (view.Message is not null)
        {
            Console.WriteLine();
            Console.WriteLine(TextFormatter.WordWrap(view.Message));
        }

        if (view.ShowDescription)
        {
            Console.WriteLine();
            Console.WriteLine(TextFormatter.WordWrap(view.Description));

            if (view.People.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine(TextFormatter.WordWrap($"People here: {string.Join(", ", view.People)}"));
            }

            if (view.Items.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine(TextFormatter.WordWrap($"You see: {string.Join(", ", view.Items)}"));
            }

            Console.WriteLine();
            Console.WriteLine(TextFormatter.WordWrap($"Exits: {string.Join(", ", view.Exits)}"));
        }

        Console.WriteLine();
        Console.WriteLine($"[Turn {view.Turn} | Health: {view.Health} | Credits: {view.Credits}]");
    }

    public void DisplayError(string message)
    {
        Console.WriteLine($"Error: {message}");
    }
}
