using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public class DeleteTransactionAction : IMenuAction
{
    private readonly IFinanceService _financeService;

    public DeleteTransactionAction(IFinanceService financeService)
    {
        _financeService = financeService;
    }

    public string Name => $"{Constants.MainMenu.ActionDelete}";

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var transactions = await FetchTransactionsAsync(ct);
        if (!transactions.Any())
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]No transactions found to delete[/]");
            return;
        }
        
        var target = PromptForSelection(transactions);
        if (ct.IsCancellationRequested)
        {
            return;
        }

        if (!AnsiConsole.Confirm("Are you sure you want to delete this transaction?"))
        {
            return;
        }
        
        await FinalizeDeletionAsync(target.Id, ct);
    }

    private async Task<List<Transaction>> FetchTransactionsAsync(CancellationToken ct)
    {
        var filter = new TransactionFilterDto();
        var result = await _financeService.GetFilteredTransactionsAsync(filter, ct);
        return result.ToList();
    }

    private Transaction PromptForSelection(List<Transaction> transactions)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<Transaction>()
                .Title("Choose transaction to delete:")
                .UseConverter(t => $"{t.Date:d} | {t.Description} ({t.Amount:N2})")
                .AddChoices(transactions));
    }

    private async Task FinalizeDeletionAsync(Guid id, CancellationToken ct)
    {
        await _financeService.DeleteTransactionAsync(id, ct);
        AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]Transaction deleted successfully[/]");
    }
}