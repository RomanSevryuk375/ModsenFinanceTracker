using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public class AddTransactionAction : IMenuAction
{
    private readonly IFinanceService _financeService;
    private readonly ITransactionFactory _factory;
    private readonly ICategoryService _categoryService;

    public AddTransactionAction(
        IFinanceService financeService, 
        ITransactionFactory factory,
        ICategoryService categoryService)
    {
        _financeService = financeService;
        _factory = factory;
        _categoryService = categoryService;
    }

    public string Name => Constants.MainMenu.ActionAdd;

    public async Task ExecuteAsync(CancellationToken ct)
    {
        try
        {
            var amount = PromptAmount();
            var type = PromptTransactionType();
            var category = await GetSelectedCategoryAsync(type, ct);
            if (category is null)
            {
                return;
            }

            var description = PromptDescription();
            if (ct.IsCancellationRequested)
            {
                return;
            }

            await SaveTransactionAsync(type, amount, description, category.Id, ct);
        }
        catch (ArgumentException ex) 
        {
            AnsiConsole.MarkupLine($"[red]Validation Error: {ex.Message}[/]");
        }
    }
    

    private decimal PromptAmount()
    {
        return AnsiConsole.Prompt(new TextPrompt<decimal>("Enter amount:"));
    }

    private TransactionType PromptTransactionType()
    {
        var isIncome = AnsiConsole.Confirm($"Is this an [{Constants.Colors.Info}]Income[/]?");
        return isIncome 
            ? TransactionType.Income 
            : TransactionType.Expense;
    }

    private async Task<Category?> GetSelectedCategoryAsync(TransactionType type, CancellationToken ct)
    {
        var categories = (await _categoryService.GetCategoriesByTypeAsync(type, ct)).ToList();
        
        if (!categories.Any())
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Error}]No categories found. Seed data first.[/]");
            return null;
        }

        return AnsiConsole.Prompt(
            new SelectionPrompt<Category>()
                .Title("Select category:")
                .UseConverter(c => c.Name)
                .AddChoices(categories));
    }

    private string PromptDescription()
    {
        return AnsiConsole.Ask<string>("Enter description:");
    }

    private async Task SaveTransactionAsync(
        TransactionType type,
        decimal amount, 
        string desc,
        Guid categoryId,
        CancellationToken ct)
    {
        var transaction = _factory.CreateTransaction(type, amount, desc, categoryId);
        await _financeService.AddTransactionAsync(transaction, ct);
        AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]Transaction added successfully[/]");
    }
}