using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.UI.Helpers;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions.TransactionActions;

public sealed class DeleteTransactionAction(
    IFinanceService financeService,
    IWalletService walletService) : ITransactionMenuAction
{
    public string Name => Constants.MainMenu.DeleteTransaction;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Wallet? wallet = await UIHelper.PromptWalletAsync(
            walletService, cancellationToken);
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

        Transaction target = SelectTransaction(transactions);
        if (!AnsiConsole.Confirm(Constants.Prompts.TransactionConfirmation))
        {
            return;
        }

        Result deleteResult = await financeService.DeleteTransactionAsync(
            wallet.Id, target.Id, cancellationToken);

        UIHelper.ProcessResult(deleteResult, Constants.Success.DeleteTransaction);
    }

    private static Transaction SelectTransaction(IReadOnlyList<Transaction> choices)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<Transaction>()
                .Title(Constants.Prompts.DeleteTransaction)
                .UseConverter(t => $"{t.Date:d} | {t.Description} ({t.Amount:N2})")
                .AddChoices(choices));
    }
}