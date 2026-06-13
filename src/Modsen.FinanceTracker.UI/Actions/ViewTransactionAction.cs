using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.UI.Interfaces;
using Modsen.FinanceTracker.UI.Models;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public sealed class ViewTransactionAction(
    IFinanceService financeService,
    ICategoryService categoryService,
    ITransactionListView listView) : IMenuAction
{
    public string Name => Constants.MainMenu.ActionView;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var searchTerm = PromptSearchTerm();
        var (from, to) = PromptDateRange();

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

        var transactions = (await financeService.GetFilteredTransactionsAsync(
            filter, cancellationToken)).ToList();
        if (transactions.Count == 0)
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]No transactions found.[/]");
            return;
        }

        var categories = (await categoryService.GetAllCategoriesAsync(cancellationToken)).ToList();

        var rows = PrepareRowModels(transactions, categories);
        listView.Render(rows);
    }

    private static string PromptSearchTerm()
    {
        return AnsiConsole.Confirm("Do you want to search by description?")
            ? AnsiConsole.Ask<string>("Enter search term:")
            : string.Empty;
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

    private static IEnumerable<TransactionRowModel> PrepareRowModels(
        List<Transaction> transactions, 
        List<Category> categories)
    {
        return transactions.Select(t => new TransactionRowModel(
            t.Id.ToString()[..8],
            t.Date.ToShortDateString(),
            categories.FirstOrDefault(c => c.Id == t.CategoryId)?.Name ?? "N/A",
            t.Description,
            FormatAmount(t)
        ));
    }


    private static string FormatAmount(Transaction t)
    {
        var color = t is IncomeTransaction
            ? $"{Constants.Colors.Success}"
            : $"{Constants.Colors.Error}";

        var sign = t is IncomeTransaction
            ? "+"
            : "-";

        return $"[{color}]{sign}{t.Amount:N2}[/]";
    }
}