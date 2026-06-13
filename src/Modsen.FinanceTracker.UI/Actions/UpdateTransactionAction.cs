using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public sealed class UpdateTransactionAction(IFinanceService financeService) : IMenuAction
{
    public string Name => $"{Constants.MainMenu.ActionUpdate}";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var transactions = await FetchTransactionsAsync(cancellationToken);
        if (transactions.Count == 0)
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]No transactions found to update.[/]");
            return;
        }

        var target = PromptForSelection(transactions);
        var newAmount = PromptNewAmount(target.Amount);
        var newDescription = PromptNewDescription(target.Description);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        await FinalizeUpdateAsync(target, newAmount, newDescription, cancellationToken);
    }


    private async Task<List<Transaction>> FetchTransactionsAsync(CancellationToken cancellationToken)
    {
        var filter = new TransactionFilterDto();
        var result = await financeService.GetFilteredTransactionsAsync(filter, cancellationToken);
        return result.ToList();
    }

    private static Transaction PromptForSelection(List<Transaction> transactions)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<Transaction>()
                .Title("Choose transaction to edit:")
                .UseConverter(t => $"{t.Date:d} | {t.Description} ({t.Amount:N2})")
                .AddChoices(transactions));
    }

    private static decimal PromptNewAmount(decimal currentAmount)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<decimal>(string.Format($"New amount (current: {currentAmount}"))
                .DefaultValue(currentAmount)
                .Validate(a => a > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error($"[{Constants.Colors.Error}]Amount must be positive[/]")));
    }

    private static string PromptNewDescription(string currentDescription)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>(string.Format($"New description (current: {currentDescription}):"))
                .DefaultValue(currentDescription));
    }

    private async Task FinalizeUpdateAsync(
        Transaction target,
        decimal amount,
        string description,
        CancellationToken cancellationToken)
    {
        target.Amount = amount;
        target.Description = description;

        await financeService.UpdateTransactionAsync(target, cancellationToken);
        AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]Transaction updated successfully[/]");
    }
}