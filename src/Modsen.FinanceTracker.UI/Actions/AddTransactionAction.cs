using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public sealed class AddTransactionAction(
    IFinanceService financeService,
    ITransactionFactory factory,
    ICategoryService categoryService) : IMenuAction
{
    public string Name => Constants.MainMenu.ActionAdd;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var type = PromptTransactionType();
            var category = await GetSelectedCategoryAsync(type, cancellationToken);
            if (category is null)
            {
                return;
            }

            var amount = await PromptAmount(category, cancellationToken);
            var description = PromptDescription();
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await SaveTransactionAsync(type, amount, description, category.Id, cancellationToken);
        }
        catch (ArgumentException ex)
        {
            AnsiConsole.MarkupLine($"[red]Validation Error: {ex.Message}[/]");
        }
    }


    private async Task<decimal> PromptAmount(Category category, CancellationToken cancellationToken)
    {
        var currentBalance = await financeService.GetBalanceAsync(cancellationToken);

        return AnsiConsole.Prompt(new TextPrompt<decimal>("Enter amount:")
            .Validate(amount =>
            {
                if (amount <= 0)
                {
                    return ValidationResult.Error(
                        $"[{Constants.Colors.Error}]Amount must be positive.[/]");
                }

                if (category.Type is TransactionType.Expense && (currentBalance - amount < 0))
                {
                    return ValidationResult.Error(
                        $"[{Constants.Colors.Error}]Your current balance is {currentBalance:N2}." +
                        $" Amount will become negative.[/]");
                }

                return ValidationResult.Success();
            }));
    }

    private static TransactionType PromptTransactionType()
    {
        var isIncome = AnsiConsole.Confirm($"Is this an [{Constants.Colors.Info}]Income[/]?");
        return isIncome
            ? TransactionType.Income
            : TransactionType.Expense;
    }

    private async Task<Category?> GetSelectedCategoryAsync(
        TransactionType type,
        CancellationToken cancellationToken)
    {
        var categories = (await categoryService.GetCategoriesByTypeAsync(
            type, cancellationToken)).ToList();
        if (categories.Count == 0)
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

    private static string PromptDescription()
    {
        return AnsiConsole.Ask<string>("Enter description:");
    }

    private async Task SaveTransactionAsync(
        TransactionType type,
        decimal amount,
        string desc,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var transaction = factory.CreateTransaction(type, amount, desc, categoryId);

        await financeService.AddTransactionAsync(transaction, cancellationToken);

        AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]Transaction added successfully[/]");
    }
}