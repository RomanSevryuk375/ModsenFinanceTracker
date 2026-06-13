using Modsen.FinanceTracker.UI.Interfaces;

namespace Modsen.FinanceTracker.UI;

public sealed class App(IMainMenu mainMenu, IEnumerable<IMenuAction> actions) : IApp
{
    private bool _isRunning = true;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        while (_isRunning && !cancellationToken.IsCancellationRequested)
        {
            var availableChoices = actions.Select(a => a.Name);
            var choice = mainMenu.ShowAndGetChoice(availableChoices);

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            HandleChoice(choice, cancellationToken);
        }
    }

    private void HandleChoice(string choice, CancellationToken cancellationToken)
    {
        var action = actions.FirstOrDefault(a => a.Name == choice);

        action?.ExecuteAsync(cancellationToken);

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }
}