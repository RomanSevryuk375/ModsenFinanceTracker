using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.UI.Actions.TransactionActions;

public sealed class AddTransactionAction(
    IFinanceService financeService,
    ITransactionFactory factory,
    ICategoryService categoryService,
    IWalletService walletService) : ITransactionMenuAction
{
    public string Name => Constants.MainMenu.AddTransaction;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Wallet? wallet = await UIHelper.PromptWalletAsync(walletService, cancellationToken);
        if (wallet is null)
        {
            return;
        }

        TransactionType type = PromptTransactionType();
        Category? category = await GetSelectedCategoryAsync(type, cancellationToken);
        if (category is null)
        {
            return;
        }

        decimal amount = await PromptAmountAsync(wallet.Id, category, cancellationToken);
        string description = PromptDescription();
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        Result result = await financeService.AddTransactionAsync(
            wallet.Id, 
            factory.CreateTransaction(type, amount, description, category), 
            cancellationToken);

        UIHelper.ProcessResult(result, Constants.Success.AddTransaction);
    }

    private async Task<decimal> PromptAmountAsync(
        Guid walletId, 
        Category category, 
        CancellationToken cancellationToken)
    {
        Result<decimal> balanceResult = await financeService.GetBalanceAsync(walletId, cancellationToken);
        decimal currentBalance = balanceResult.IsSuccess
            ? balanceResult.Value 
            : 0;

        return AnsiConsole.Prompt(new TextPrompt<decimal>(Constants.Prompts.Amount)
            .Validate(amount =>
            {
                if (amount <= 0)
                {
                    return ValidationResult.Error(Constants.Errors.NegativeAmount);
                }

                if (category!.Type is TransactionType.Expense && (currentBalance - amount < 0))
                {
                    return ValidationResult.Error(Constants.Errors.BalanceBecomeNegative);
                }

                return ValidationResult.Success();
            }));
    }

    private static TransactionType PromptTransactionType()
    {
        return AnsiConsole.Confirm(Constants.Prompts.TransactionType)
            ? TransactionType.Income 
            : TransactionType.Expense;
    }

    private async Task<Category?> GetSelectedCategoryAsync(
        TransactionType type, 
        CancellationToken cancellationToken)
    {
        Result<IEnumerable<Category>> categoriesResult = await categoryService
            .GetCategoriesByTypeAsync(type, cancellationToken);
        if (categoriesResult.IsFailure || categoriesResult.Value.ToList().Count == 0)
        {
            AnsiConsole.MarkupLine(Constants.Errors.CategoriesNotFound);
            return null;
        }

        IEnumerable<Category> categories = categoriesResult.Value;

        return AnsiConsole.Prompt(new SelectionPrompt<Category>()
            .Title(Constants.Prompts.Category)
            .UseConverter(c => c.Name)
            .AddChoices(categories));
    }

    private static string PromptDescription() =>
        AnsiConsole.Ask<string>(Constants.Prompts.Description);
}
