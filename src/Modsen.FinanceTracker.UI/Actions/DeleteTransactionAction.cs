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
        var transactions = 
            (await _financeService.GetFilteredTransactionsAsync(new TransactionFilterDto(), ct)).ToList();

        if (!transactions.Any())
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]No transactions found to update.[/]");
            return;
        }
        
        var target = AnsiConsole.Prompt(
            new SelectionPrompt<Transaction>()
                .Title("Choose transaction to delete:")
                .UseConverter(t => $"{t.Date:d} | {t.Description} ({t.Amount:N2})")
                .AddChoices(transactions));

        await _financeService.DeleteTransactionAsync(target.Id, ct);

        AnsiConsole.MarkupLine($"[{Constants.Colors.Success}] Transaction deleted successfully[/]");
    }
}