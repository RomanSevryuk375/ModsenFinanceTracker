namespace Modsen.FinanceTracker.UI.Actions.TemplateActions;

public sealed class ViewTemplatesAction(
    IWalletService walletService,
    ITemplateListView listView) : ITemplateMenuAction
{
    public string Name => Constants.MainMenu.ViewTemplates;

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

        listView.Render(templates);
    }
}
