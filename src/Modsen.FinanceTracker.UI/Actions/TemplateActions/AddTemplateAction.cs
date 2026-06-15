using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;
using Modsen.FinanceTracker.UI.Helpers;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

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
            wallet.Id, amount, name, description,
            period, nextDate, category, cancellationToken);
    }

    private async Task ProcessTemplateCreationAsync(
        Guid walletId,
        decimal amount,
        string name,
        string description,
        Period period,
        DateTime nextDate,
        Category category,
        CancellationToken cancellationToken)
    {
        Result<RecurringTransactionTemplate> templateResult = RecurringTransactionTemplate.Create(
            amount, name, description, period, nextDate, category);

        if (templateResult.IsFailure)
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Error}]{templateResult.Error}[/]");
            return;
        }

        Result result = await financeService.AddTemplateAsync(
            walletId, templateResult.Value, cancellationToken);

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
        var categories = (await categoryService
            .GetCategoriesByTypeAsync(type, cancellationToken)).ToList();

        if (categories.Count == 0)
        {
            AnsiConsole.MarkupLine(Constants.Errors.CategoriesNotFound);
            return null;
        }

        return AnsiConsole.Prompt(new SelectionPrompt<Category>()
            .Title(Constants.Prompts.Category)
            .UseConverter(c => c.Name)
            .AddChoices(categories));
    }
}
