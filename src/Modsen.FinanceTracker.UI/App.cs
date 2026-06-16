using Microsoft.Extensions.Logging;
using Modsen.FinanceTracker.Infrastructure.Configuration;
using Modsen.FinanceTracker.Infrastructure.Security;

namespace Modsen.FinanceTracker.UI;

public sealed class App(
    IMainMenu mainMenu,
    IEnumerable<IMenuAction> actions,
    IFinanceService financeService,
    ISchedulerService schedulerService,
    ILogger<App> logger) : IApp
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

        schedulerService.OnTransactionWrittenOff += (sender, args) =>
        {
            AnsiConsole.MarkupLine(
                $"[{Constants.Colors.Info}]AUTO-PAYMENT:[/] {args.Amount:N2} for '{args.Description}' " +
                $"deducted from '{args.WalletName}'. Next due: {args.NextExecutionDate:d}");
        };
        await schedulerService.CheckAndProcessRecurringTransactionsAsync(cancellationToken);

        Console.WriteLine("Press any key to continue to Main Menu...");
        Console.ReadKey(true);

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
            logger.LogInformation("User navigated to menu action: '{ActionName}'", action.Name);
            await action.ExecuteAsync(cancellationToken);
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }

    private bool AuthenticateUser()
    {
        AppConfiguration config = AppConfiguration.Instance;

        if (!config.IsPasswordEnabled)
        {
            logger.LogInformation("User successfully authenticated.");
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

        logger.LogWarning("Failed authentication attempt: Invalid password entered.");
        AnsiConsole.MarkupLine(Constants.Errors.InvalidPassword);
        return false;
    }
}
