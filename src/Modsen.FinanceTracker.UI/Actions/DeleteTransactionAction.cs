using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public sealed class DeleteTransactionAction(IFinanceService financeService) : IMenuAction
{
    public string Name => $"{Constants.MainMenu.ActionDelete}";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var transactions = await FetchTransactionsAsync(cancellationToken);
        if (transactions.Count == 0)
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]No transactions found to delete[/]");
            return;
        }

        var target = PromptForSelection(transactions);
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        if (!AnsiConsole.Confirm("Are you sure you want to delete this transaction?"))
        {
            return;
        }

        await FinalizeDeletionAsync(target.Id, cancellationToken);
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
                .Title("Choose transaction to delete:")
                .UseConverter(t => $"{t.Date:d} | {t.Description} ({t.Amount:N2})")
                .AddChoices(transactions));
    }

    private async Task FinalizeDeletionAsync(Guid id, CancellationToken cancellationToken)
    {
        await financeService.DeleteTransactionAsync(id, cancellationToken);
        AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]Transaction deleted successfully[/]");
    }
}