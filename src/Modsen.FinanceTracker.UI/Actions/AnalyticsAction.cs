using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.UI.Interfaces;
using Modsen.FinanceTracker.UI.Models;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public sealed class AnalyticsAction(
    IFinanceService financeService,
    ICategoryService categoryService,
    IAnalyticsListView listView) : IMenuAction
{
    public string Name => Constants.MainMenu.ActionAnalytics;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var (from, to) = PromptDateRange();
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

        var expensesTransactions = (await financeService.GetFilteredTransactionsAsync(
            filter, cancellationToken)).OfType<ExpenseTransaction>().ToList();

        if (expensesTransactions.Count == 0)
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]No expense transactions found for this period.[/]");
            return;
        }

        var categories = (await categoryService.GetAllCategoriesAsync(cancellationToken)).ToList();

        var rows = PrepareRowModels(expensesTransactions, categories);
        listView.Render(rows);
    }

    private static (DateTime? from, DateTime? to) PromptDateRange()
    {
        DateTime? from = AnsiConsole.Prompt(
            new TextPrompt<DateTime>("Start date:")
                .DefaultValue(DateTime.Now.AddMonths(-1))
                .ValidationErrorMessage($"[{Constants.Colors.Error}]Invalid format[/]"));

        DateTime? to = AnsiConsole.Prompt(
            new TextPrompt<DateTime>("End date:")
                .DefaultValue(DateTime.Now)
                .ValidationErrorMessage($"[{Constants.Colors.Error}]Invalid format[/]")
                .Validate(date =>
                    date >= from
                        ? ValidationResult.Success()
                        : ValidationResult.Error("End date must be after start date")));

        return (from, to);
    }

    private static IEnumerable<AnalyticsRowModel> PrepareRowModels(
        List<ExpenseTransaction> expenses,
        List<Category> categories)
    {
        var totalAmount = expenses.Sum(x => x.Amount);

        return expenses
            .GroupBy(x => x.CategoryId)
            .Select(g =>
            {
                var categoryName = categories.FirstOrDefault(c => c.Id == g.Key)?.Name ?? "N/A";
                var groupAmount = g.Sum(x => x.Amount);

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