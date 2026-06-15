using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.UI.Helpers;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions.TransactionActions;

public sealed class UpdateTransactionAction(
    IFinanceService financeService,
    IWalletService walletService) : ITransactionMenuAction
{
    public string Name => Constants.MainMenu.UpdateTransaction;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Wallet? wallet = await UIHelper.PromptWalletAsync(walletService, cancellationToken);
        if (wallet is null)
        {
            return;
        }

        IReadOnlyList<Transaction> transactions = await UIHelper.FetchTransactionsAsync(
            wallet.Id, new TransactionFilterDto(), financeService, cancellationToken);
        if (transactions.Count == 0)
        {
            return; 
        }

        Transaction target = SelectTransactionToUpdate(transactions);
        decimal newAmount = SelectNewAmount(target);
        string newDescription = SelectNewDescription(target);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        Result updateResult = await financeService.UpdateTransactionAsync(
            wallet.Id, newAmount, newDescription, target.Id, cancellationToken);

        UIHelper.ProcessResult(updateResult, Constants.Success.UpdateTransaction);
    }

    private static Transaction SelectTransactionToUpdate(
        IReadOnlyList<Transaction> transactions)
    {
        return AnsiConsole.Prompt(
                new SelectionPrompt<Transaction>()
                    .Title(Constants.Prompts.EditTransaction)
                    .UseConverter(t => $"{t.Date:d} | {t.Description} ({t.Amount:N2})")
                    .AddChoices(transactions));
    }

    private static decimal SelectNewAmount(Transaction target)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<decimal>(Constants.Prompts.NewAmount(target.Amount))
                .DefaultValue(target.Amount)
                .Validate(a => a > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error(Constants.Errors.NegativeAmount)));
    }

    private static string SelectNewDescription(Transaction target)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>(Constants.Prompts.NewDescription(target.Description))
                .DefaultValue(target.Description));
    }
}