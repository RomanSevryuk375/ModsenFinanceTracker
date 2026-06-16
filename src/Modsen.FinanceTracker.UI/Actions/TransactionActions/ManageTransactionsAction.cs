namespace Modsen.FinanceTracker.UI.Actions.TransactionActions;

public sealed class ManageTransactionsAction(
    IEnumerable<ITransactionMenuAction> transactionActions) : IMenuAction
{
    public string Name => Constants.MainMenu.ManageTransactions;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var choices = transactionActions.Select(x => x.Name).ToList();
        choices.Add(Constants.MainMenu.BackToMainMenu);

        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(Constants.Prompts.TransactionManagementTitle)
                .AddChoices(choices));

        if (choice == Constants.MainMenu.BackToMainMenu)
        {
            return;
        }

        ITransactionMenuAction actionToExecute =
            transactionActions.First(a => a.Name == choice);
        await actionToExecute.ExecuteAsync(cancellationToken);
    }
}
