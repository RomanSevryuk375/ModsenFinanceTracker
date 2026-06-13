using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI;

public sealed class App(
    IMainMenu mainMenu, 
    IEnumerable<IMenuAction> actions, 
    IFinanceService financeService) : IApp
{
    private bool _isRunning = true;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        financeService.OnCategoryLimitExceeded += (sender, args) =>
        {
            AnsiConsole.MarkupLine(
                $"[yellow]WARNING: You exceeded the budget limit for {args.CategoryName} " +
                $"by {args.ExcessAmount:N2}![/]");
        };

        while (_isRunning && !cancellationToken.IsCancellationRequested)
        {
            var availableChoices = actions.Select(a => a.Name);
            var choice = mainMenu.ShowAndGetChoice(availableChoices);

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            await HandleChoice(choice, cancellationToken);
        }
    }

    private async Task HandleChoice(string choice, CancellationToken cancellationToken)
    {
        var action = actions.FirstOrDefault(a => a.Name == choice);

        action?.ExecuteAsync(cancellationToken);

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }
}