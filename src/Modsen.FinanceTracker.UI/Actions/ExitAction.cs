using Modsen.FinanceTracker.UI.Interfaces;

namespace Modsen.FinanceTracker.UI.Actions;

public class ExitAction : IMenuAction
{
    public string Name => Constants.MainMenu.ActionExit;
    public async Task ExecuteAsync(CancellationToken ct)
    {
        Environment.Exit(0);
    }
}