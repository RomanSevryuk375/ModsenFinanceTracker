using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.UI.Actions;

public sealed class AnalyticsAction(
    IFinanceService financeService,
    IWalletService walletService,
    IAnalyticsListView listView) : IMenuAction
{
    public string Name => Constants.MainMenu.ActionAnalytics;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Wallet? wallet = await UIHelper.PromptWalletAsync(walletService, cancellationToken);
        if (wallet is null)
        {
            return;
        }

        (DateTime? from, DateTime? to) = PromptDateRange();
        var filter = new TransactionFilterDto
        {
            SearchTerm = null,
            From = from,
            To = to
        };

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        Result<IReadOnlyList<Transaction>> transactionsResult = await financeService.GetFilteredTransactionsAsync(
            wallet.Id, filter, cancellationToken);

        if (transactionsResult.IsFailure)
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Error}]{transactionsResult.Error}[/]");
            return;
        }

        var expenses = transactionsResult.Value.OfType<ExpenseTransaction>().ToList();

        if (expenses.Count == 0)
        {
            AnsiConsole.MarkupLine(Constants.Errors.TransactionNotFound);
            return;
        }

        IEnumerable<AnalyticsRowModel> rows = PrepareRowModels(expenses);
        listView.Render(rows);
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

    private static IEnumerable<AnalyticsRowModel> PrepareRowModels(List<ExpenseTransaction> expenses)
    {
        decimal totalAmount = expenses.Sum(x => x.Amount.Amount);

        return expenses
            .GroupBy(x => x.Category.Id)
            .Select(g =>
            {
                string categoryName = g.First().Category.Name;
                decimal groupAmount = g.Sum(x => x.Amount.Amount);

                double percent = totalAmount > 0
                    ? ((double)groupAmount / (double)totalAmount) * 100
                    : 0;

                return new AnalyticsRowModel(
                    categoryName,
                    $"{percent:F1}%",
                    groupAmount.ToString("N2"),
                    (double)groupAmount
                );
            });
    }
}
