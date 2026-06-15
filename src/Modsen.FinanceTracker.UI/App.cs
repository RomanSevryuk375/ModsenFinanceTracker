using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Infrastructure.Configuration;
using Modsen.FinanceTracker.Infrastructure.Security;
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
        if (!AuthenticateUser())
        {
            return; 
        }

        financeService.OnCategoryLimitExceeded += (sender, args) =>
        {
            AnsiConsole.MarkupLine(
                $"[yellow]WARNING: You exceeded the budget limit for {args.CategoryName} " +
                $"by {args.ExcessAmount:N2}![/]");
        };

        while (_isRunning && !cancellationToken.IsCancellationRequested)
        {
            IEnumerable<string> availableChoices = actions.Select(a => a.Name);
            string choice = mainMenu.ShowAndGetChoice(availableChoices);

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            await HandleChoiceAsync(choice, cancellationToken);
        }
    }

    private async Task HandleChoiceAsync(string choice, CancellationToken cancellationToken)
    {
        IMenuAction? action = actions.FirstOrDefault(a => a.Name == choice);

        if (action is not null)
        {
            await action.ExecuteAsync(cancellationToken);
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }

    private static bool AuthenticateUser()
    {
        AppConfiguration config = AppConfiguration.Instance;

        if (!config.IsPasswordEnabled)
        {
            return true;
        }

        string password = AnsiConsole.Prompt(
            new TextPrompt<string>(Constants.Prompts.EnterPassword)
                .Secret());

        string hashedInput = PasswordHasher.ComputeSha256Hash(password);

        if (hashedInput.Equals(config.HashedPassword, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        AnsiConsole.MarkupLine(Constants.Errors.InvalidPassword);
        return false;
    }
}