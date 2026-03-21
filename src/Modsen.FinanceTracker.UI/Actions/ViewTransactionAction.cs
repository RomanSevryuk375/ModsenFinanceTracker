using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.UI.Interfaces;
using Modsen.FinanceTracker.UI.Models;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public class ViewTransactionAction : IMenuAction
{
    private readonly IFinanceService _financeService;
    private readonly ICategoryService _categoryService;
    private readonly ITransactionListView _listView;

    public ViewTransactionAction(
        IFinanceService financeService, 
        ICategoryService categoryService, 
        ITransactionListView listView)
    {
        _financeService = financeService;
        _categoryService = categoryService;
        _listView = listView;
    }

    public string Name => Constants.MainMenu.ActionView;

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var searchTerm = PromptSearchTerm();
        var (from, to) = PromptDateRange();
        if (ct.IsCancellationRequested)
        {
            return;
        }
        
        var filter = new TransactionFilterDto
        {
            SearchTerm = searchTerm, 
            From = from, 
            To = to
        };
        var transactions = (await _financeService.GetFilteredTransactionsAsync(filter, ct)).ToList();

        if (!transactions.Any())
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]No transactions found.[/]");
            return;
        }

        var categories = (await _categoryService.GetAllCategoriesAsync(ct)).ToList();
        
        var rows = PrepareRowModels(transactions, categories);
        _listView.Render(rows);
    }
    
    private string PromptSearchTerm()
    {
        return AnsiConsole.Confirm("Do you want to search by description?")
            ? AnsiConsole.Ask<string>("Enter search term:")
            : string.Empty;
    }

    private (DateTime? from, DateTime? to) PromptDateRange()
    {
        DateTime? from = AnsiConsole.Prompt(
            new TextPrompt<DateTime>("Start date:")
                .DefaultValue(DateTime.Now.AddMonths(-1))
                .ValidationErrorMessage($"[{Constants.Colors.Error}]Invalid format[/]"));

        DateTime? to = AnsiConsole.Prompt(
            new TextPrompt<DateTime>("End date:")
                .DefaultValue(DateTime.Now)
                .ValidationErrorMessage($"[{Constants.Colors.Error}]Invalid format[/]")
                .Validate(date => date >= from 
                    ? ValidationResult.Success() 
                    : ValidationResult.Error("End date must be after start date")));

        return (from, to);
    }

    private IEnumerable<TransactionRowModel> PrepareRowModels(List<Transaction> transactions, List<Category> categories)
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