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
        var amount = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Enter amount:")
                .Validate(a => a > 0 
                    ? ValidationResult.Success() 
                    : ValidationResult.Error($"[{Constants.Colors.Error}]Amount must be positive[/]")));
        if (ct.IsCancellationRequested)
        {
            return;
        }

        var isIncome = AnsiConsole.Confirm($"Is this an [{Constants.Colors.Info}]Income[/]?");
        
        var type = isIncome 
            ? TransactionType.Income 
            : TransactionType.Expense;

        var categories = (await _categoryService.GetAllCategoriesAsync(ct: ct)).ToList();
        if (!categories.Any())
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Error}]No categories found. Seed data first.[/]");
            return;
        }
        
        var category = AnsiConsole.Prompt(
            new SelectionPrompt<Category>()
                .Title("Select category:")
                .UseConverter(c => c.Name)
                .AddChoices(categories));
        if (ct.IsCancellationRequested)
        {
            return;
        }

        var description = AnsiConsole.Ask<string>("Enter description:");
        if (ct.IsCancellationRequested)
        {
            return;
        }

        var transaction = _factory.CreateTransaction(type, amount, description, category.Id);
        await _financeService.AddTransactionAsync(transaction, ct);

        AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]Transaction added successfully[/]");
    }
}