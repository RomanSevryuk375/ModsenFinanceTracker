using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.UI.Actions.TemplateActions;

public sealed class DeleteTemplateAction(
    IFinanceService financeService,
    IWalletService walletService) : ITemplateMenuAction
{
    public string Name => Constants.MainMenu.DeleteTemplate;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Wallet? wallet = await UIHelper.PromptWalletAsync(walletService, cancellationToken);
        if (wallet is null)
        {
            return;
        }

        IReadOnlyList<RecurringTransactionTemplate> templates = wallet.Templates;

        if (templates.Count == 0)
        {
            AnsiConsole.MarkupLine(Constants.Errors.TemplateNotFound);
            return;
        }

        RecurringTransactionTemplate target = AnsiConsole.Prompt(
            new SelectionPrompt<RecurringTransactionTemplate>()
                .Title(Constants.Prompts.DeleteTemplate)
                .UseConverter(t => $"{t.Name} | {t.Amount:N2} ({t.Period})")
                .AddChoices(templates));

        if (!AnsiConsole.Confirm(Constants.Prompts.DeleteTemplateConfirmation))
        {
            return;
        }

        Result deleteResult = await financeService.DeleteTemplateAsync(
            wallet.Id, target.Id, cancellationToken);
        UIHelper.ProcessResult(deleteResult, Constants.Success.DeleteTemplate);
    }
}
