using Airk.Engine;
using Airk.Persistence;
using Airk.State;
using Airk.UI;

if (args.Length == 0)
{
    // Interactive mode
    var state = SessionManager.Load();
    var ui = new InteractiveUI();
    var engine = new GameEngine(ui, state);
    engine.Run();
}
else if (args[0].Equals("new", StringComparison.OrdinalIgnoreCase))
{
    // Start a new game
    SessionManager.Delete();
    var state = WorldBuilder.CreateNewGame();
    SessionManager.Save(state);

    if (args.Length > 1)
    {
        // Batch mode with new game: new "command"
        var command = string.Join(" ", args.Skip(1));
        var ui = new BatchUI(command);
        var engine = new GameEngine(ui, state, batchMode: true);
        engine.Run();
    }
    else
    {
        // Just create new game and output initial state
        var view = state.Project();
        var ui = new BatchUI("look");
        ui.DisplayView(view);
    }
}
else
{
    // Batch mode: command passed as argument
    var state = SessionManager.Load();
    var command = string.Join(" ", args);
    var ui = new BatchUI(command);
    var engine = new GameEngine(ui, state, batchMode: true);
    engine.Run();
}
