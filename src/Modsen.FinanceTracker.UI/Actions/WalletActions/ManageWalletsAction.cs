namespace Modsen.FinanceTracker.UI.Actions.WalletActions;

public sealed class ManageWalletsAction(
    IEnumerable<IWalletMenuAction> walletActions) : IMenuAction
{
    public string Name => Constants.MainMenu.ManageWallets;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var choices = walletActions.Select(a => a.Name).ToList();
        choices.Add(Constants.MainMenu.BackToMainMenu);

        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(Constants.Prompts.WalletManagementTitle)
                .AddChoices(choices));

        if (choice == Constants.MainMenu.BackToMainMenu)
        {
            return;
        }

        IWalletMenuAction actionToExecute = walletActions.First(a => a.Name == choice);
        await actionToExecute.ExecuteAsync(cancellationToken);
    }
}