using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public class UpdateTransactionAction : IMenuAction
{
    private readonly IFinanceService _financeService;

    public UpdateTransactionAction(IFinanceService financeService)
    {
        _financeService = financeService;
    }

    public string Name => $"{Constants.MainMenu.ActionUpdate}"; 

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
                .Title("Choose transaction to edit:")
                .UseConverter(t => $"{t.Date:d} | {t.Description} ({t.Amount:N2})")
                .AddChoices(transactions));
        
        var newAmount = AnsiConsole.Prompt(
            new TextPrompt<decimal>($"New amount (current: {target.Amount}):")
                .DefaultValue(target.Amount));
        
        var newDesc = AnsiConsole.Prompt(
            new TextPrompt<string>($"New description (current: {target.Description}):")
                .DefaultValue(target.Description));
        
        if (ct.IsCancellationRequested)
        {
            return;
        }

        target.Amount = newAmount;
        target.Description = newDesc;
        
        await _financeService.UpdateTransactionAsync(target, ct);

        AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]Transaction updated[/]");
    }
}