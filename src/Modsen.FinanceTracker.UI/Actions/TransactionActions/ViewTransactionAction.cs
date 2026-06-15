using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.UI.Helpers;
using Modsen.FinanceTracker.UI.Interfaces;
using Modsen.FinanceTracker.UI.Models;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions.TransactionActions;

public sealed class ViewTransactionAction(
    IFinanceService financeService,
    IWalletService walletService,
    ITransactionListView listView) : ITransactionMenuAction
{
    public string Name => Constants.MainMenu.ViewTransaction;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Wallet? wallet = await UIHelper.PromptWalletAsync(walletService, cancellationToken);
        if (wallet is null)
        {
            return;
        }

        string searchTerm = PromptSearchTerm();
        (DateTime? from, DateTime? to) = PromptDateRange();
        var filter = new TransactionFilterDto
        {
            SearchTerm = searchTerm,
            From = from,
            To = to
        };

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        IReadOnlyList<Transaction> transactions = await UIHelper.FetchTransactionsAsync(
            wallet.Id, filter, financeService, cancellationToken);
        if (transactions.Count == 0)
        {
            return; 
        }

        IEnumerable<TransactionRowModel> rows = PrepareRowModels(transactions);
        listView.Render(rows);
    }

    private static string PromptSearchTerm()
    {
        return AnsiConsole.Confirm(Constants.Prompts.SearchConfirmation)
            ? AnsiConsole.Ask<string>(Constants.Prompts.SearchTerm)
            : string.Empty;
    }

    private static (DateTime? from, DateTime? to) PromptDateRange()
    {
        DateTime? from = AnsiConsole.Prompt(
            new TextPrompt<DateTime>(Constants.Prompts.StartDate)
                .DefaultValue(DateTime.Now.AddMonths(-1))
                .ValidationErrorMessage(Constants.Errors.InvalidFormat));

        DateTime? to = AnsiConsole.Prompt(
            new TextPrompt<DateTime>(Constants.Prompts.EndDate)
                .DefaultValue(DateTime.Now)
                .ValidationErrorMessage(Constants.Errors.InvalidFormat)
                .Validate(date =>
                    date >= from
                        ? ValidationResult.Success()
                        : ValidationResult.Error(Constants.Errors.EndInFuture)));

        return (from, to);
    }

    private static IEnumerable<TransactionRowModel> PrepareRowModels(
        IReadOnlyList<Transaction> transactions)
    {
        return transactions.Select(t => new TransactionRowModel(
            t.Id.ToString()[..Constants.UI.GuidShortLength],
            t.Date.ToShortDateString(),
            t.Category?.Name ?? "N/A",
            t.Description,
            FormatAmount(t)
        ));
    }


    private static string FormatAmount(Transaction t)
    {
        (string? color, string? sign) = t switch
        {
            IncomeTransaction => (Constants.Colors.Success, "+"),

            _ => (Constants.Colors.Error, "-")
        };

        return $"[{color}]{sign}{t.Amount:N2}[/]";
    }
}