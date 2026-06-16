using Modsen.FinanceTracker.Domain.Extensions;
using Modsen.FinanceTracker.Domain.ValueObjects; 

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

        Money amount = await PromptAmountAsync(wallet, category, cancellationToken);
        TransactionDescription description = PromptDescription();

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        Transaction? transaction = factory.CreateTransaction(type, amount, description, category);
        if (transaction is null)
        {
            return;
        }

        Result result = await financeService.AddTransactionAsync(wallet.Id, transaction, cancellationToken);

        UIHelper.ProcessResult(result, Constants.Success.AddTransaction);
    }

    private async Task<Money> PromptAmountAsync(
        Wallet wallet,
        Category category,
        CancellationToken cancellationToken)
    {
        Result<decimal> balanceResult = await financeService.GetBalanceAsync(wallet.Id, cancellationToken);
        Money currentBalance = balanceResult.IsSuccess
            ? Money.Create(balanceResult.Value, wallet.BaseCurrency).Value
            : Money.Create(0, wallet.BaseCurrency).Value;

        decimal amountDecimal = AnsiConsole.Prompt(new TextPrompt<decimal>(Constants.Prompts.Amount)
            .Validate(amount =>
            {
                if (amount <= 0)
                {
                    return ValidationResult.Error(Constants.Errors.NegativeAmount);
                }

                Result<Money> moneyResult = Money.Create(amount, wallet.BaseCurrency);
                if (moneyResult.IsFailure)
                {
                    return ValidationResult.Error($"[{Constants.Colors.Error}]{moneyResult.Error}[/]");
                }

                if (category.Type is TransactionType.Expense && currentBalance < moneyResult.Value)
                {
                    return ValidationResult.Error(Constants.Errors.BalanceBecomeNegative);
                }

                return ValidationResult.Success();
            }));

        return Money.Create(amountDecimal, wallet.BaseCurrency).Value;
    }

    private static TransactionDescription PromptDescription()
    {
        string descriptionString = AnsiConsole.Prompt(new TextPrompt<string>(Constants.Prompts.Description)
            .Validate(desc =>
            {
                Result<TransactionDescription> result = TransactionDescription.Create(desc);

                return result.IsSuccess
                    ? ValidationResult.Success()
                    : ValidationResult.Error($"[{Constants.Colors.Error}]{result.Error}[/]");
            }));

        return TransactionDescription.Create(descriptionString).Value;
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

        if (categoriesResult.IsFailure || !categoriesResult.Value.Any())
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
}
