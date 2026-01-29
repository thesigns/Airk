using Airk.Commands;
using Airk.Persistence;
using Airk.State;
using Airk.UI;

namespace Airk.Engine;

public sealed class GameEngine
{
    private readonly IGameUI _ui;
    private readonly CommandProcessor _commands;
    private GameState _state;
    private readonly bool _batchMode;

    public GameEngine(IGameUI ui, GameState state, bool batchMode = false)
    {
        _ui = ui;
        _state = state;
        _commands = new CommandProcessor();
        _batchMode = batchMode;
    }

    public void Run()
    {
        // In batch mode, we display the initial view only after the command executes
        if (!_batchMode)
        {
            var initialView = _state.Project();
            _ui.DisplayView(initialView);
        }

        while (true)
        {
            var input = _ui.ReadCommand();
            if (input is null)
            {
                break;
            }

            var result = _commands.Execute(_state, input);
            _state.LastMessage = result.Message;

            // Only increment turn on successful commands
            if (result.Success)
            {
                _state.TurnNumber++;
            }

            SessionManager.Save(_state);

            if (_batchMode)
            {
                // In batch mode, output the full view as JSON and exit
                var view = _state.Project();
                _ui.DisplayView(view);
                break;
            }

            var nextView = _state.Project() with { ShowDescription = result.ShowDescription };
            _ui.DisplayView(nextView);

            if (result.ShouldQuit)
            {
                break;
            }
        }
    }
}
