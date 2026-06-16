using Modsen.FinanceTracker.Domain.Extensions;
using Modsen.FinanceTracker.Domain.ValueObjects;

namespace Modsen.FinanceTracker.UI.Actions.TemplateActions;

public sealed class AddTemplateAction(
    IFinanceService financeService,
    ICategoryService categoryService,
    IWalletService walletService) : ITemplateMenuAction
{
    public string Name => Constants.MainMenu.AddTemplate;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Wallet? wallet = await UIHelper.PromptWalletAsync(walletService, cancellationToken);
        if (wallet is null)
        {
            return;
        }

        Category? category = await PromptCategorySequenceAsync(cancellationToken);
        if (category is null)
        {
            return;
        }

        string name = PromptName();
        decimal amount = PromptAmount();
        string description = PromptDescription();
        Period period = PromptPeriod();
        DateTime nextDate = PromptNextExecutionDate();

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        await ProcessTemplateCreationAsync(
            wallet, amount, name, description,
            period, nextDate, category, cancellationToken);
    }

    private async Task ProcessTemplateCreationAsync(
        Wallet wallet,
        decimal amount,
        string name,
        string description,
        Period period,
        DateTime nextDate,
        Category category,
        CancellationToken cancellationToken)
    {
        Result<Money> amountResult = Money.Create(amount, wallet.BaseCurrency);
        if (amountResult.IsFailure)
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Error}]{amountResult.Error}[/]");
            return;
        }

        Result<TransactionDescription> descriptionResult = TransactionDescription.Create(description);
        if (descriptionResult.IsFailure)
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Error}]{descriptionResult.Error}[/]");
            return;
        }


        Result<RecurringTransactionTemplate> templateResult = RecurringTransactionTemplate.Create(
            amountResult.Value, name, descriptionResult.Value, period, nextDate, category);

        if (templateResult.IsFailure)
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Error}]{templateResult.Error}[/]");
            return;
        }

        Result result = await financeService.AddTemplateAsync(
            wallet.Id, templateResult.Value, cancellationToken);

        UIHelper.ProcessResult(result, Constants.Success.AddTemplate);
    }

    private async Task<Category?> PromptCategorySequenceAsync(CancellationToken cancellationToken)
    {
        TransactionType type = PromptTransactionType();
        return await GetSelectedCategoryAsync(type, cancellationToken);
    }

    private static string PromptName() =>
        AnsiConsole.Ask<string>(Constants.Prompts.TemplateName);

    private static decimal PromptAmount()
    {
        return AnsiConsole.Prompt(new TextPrompt<decimal>(Constants.Prompts.Amount)
            .Validate(a => a > 0
                ? ValidationResult.Success()
                : ValidationResult.Error(Constants.Errors.NegativeAmount)));
    }

    private static string PromptDescription() =>
        AnsiConsole.Ask<string>(Constants.Prompts.Description);

    private static Period PromptPeriod()
    {
        return AnsiConsole.Prompt(new SelectionPrompt<Period>()
            .Title(Constants.Prompts.TemplatePeriod)
            .AddChoices(Enum.GetValues<Period>()));
    }

    private static DateTime PromptNextExecutionDate()
    {
        return AnsiConsole.Prompt(new TextPrompt<DateTime>(Constants.Prompts.NextExecutionDate)
            .DefaultValue(DateTime.Now.AddDays(1))
            .ValidationErrorMessage(Constants.Errors.InvalidFormat));
    }

    private static TransactionType PromptTransactionType() =>
        AnsiConsole.Confirm(Constants.Prompts.TransactionType)
            ? TransactionType.Income
            : TransactionType.Expense;

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
}
