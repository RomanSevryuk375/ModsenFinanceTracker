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
    var transactions = await FetchTransactionsAsync(ct);
    if (!transactions.Any())
    {
        AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]No transactions found to update.[/]");
        return;
    }
    
    var target = PromptForSelection(transactions);
    var newAmount = PromptNewAmount(target.Amount);
    var newDescription = PromptNewDescription(target.Description);
    
    if (ct.IsCancellationRequested)
    {
        return;
    }

    await FinalizeUpdateAsync(target, newAmount, newDescription, ct);
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
            .Title("Choose transaction to edit:")
            .UseConverter(t => $"{t.Date:d} | {t.Description} ({t.Amount:N2})")
            .AddChoices(transactions));
}

private decimal PromptNewAmount(decimal currentAmount)
{
    return AnsiConsole.Prompt(
        new TextPrompt<decimal>(string.Format($"New amount (current: {currentAmount}"))
            .DefaultValue(currentAmount)
            .Validate(a => a > 0 
                ? ValidationResult.Success() 
                : ValidationResult.Error($"[{Constants.Colors.Error}]Amount must be positive[/]")));
}

private string PromptNewDescription(string currentDescription)
{
    return AnsiConsole.Prompt(
        new TextPrompt<string>(string.Format($"New description (current: {currentDescription}):"))
            .DefaultValue(currentDescription));
}

private async Task FinalizeUpdateAsync(
    Transaction target,
    decimal amount,
    string description, 
    CancellationToken ct)
{
    target.Amount = amount;
    target.Description = description;

    await _financeService.UpdateTransactionAsync(target, ct);
    AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]Transaction updated successfully[/]");
}
}