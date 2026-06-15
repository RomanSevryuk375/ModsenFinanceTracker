using Modsen.FinanceTracker.UI.Interfaces;

namespace Modsen.FinanceTracker.UI.Actions;

public sealed class ExitAction : IMenuAction
{
    public string Name => Constants.MainMenu.ActionExit;
    public async Task ExecuteAsync(CancellationToken cancellationToken) => Environment.Exit(0);
}