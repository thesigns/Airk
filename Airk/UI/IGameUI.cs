using Airk.State;

namespace Airk.UI;

public interface IGameUI
{
    string? ReadCommand();
    void DisplayView(PlayerView view);
    void DisplayError(string message);
}
