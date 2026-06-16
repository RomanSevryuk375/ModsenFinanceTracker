using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.UI.Actions.WalletActions;

public sealed class DeleteWalletAction(
    IWalletService walletService) : IWalletMenuAction
{
    public string Name => Constants.MainMenu.DeleteWallet;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Wallet> wallets = await FetchWalletsAsync(cancellationToken);
        if (wallets.Count == 0)
        {
            return;
        }

        Wallet target = SelectWalletToDelete(wallets);
        if (!AnsiConsole.Confirm(Constants.Prompts.DeleteWalletConfirmation(target.Name)))
        {
            AnsiConsole.MarkupLine(Constants.Info.DeletionCancelled);
            return;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        Result deleteResult = await walletService.DeleteWalletAsync(target.Id, cancellationToken);

        UIHelper.ProcessResult(deleteResult, Constants.Success.DeleteWallet);
    }

    private async Task<IReadOnlyList<Wallet>> FetchWalletsAsync(CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<Wallet>> result = await walletService.GetAllWallets(cancellationToken);
        if (result.IsFailure || result.Value.Count == 0)
        {
            AnsiConsole.MarkupLine(Constants.Info.NoWalletsToDelete);
            return [];
        }

        return result.Value;
    }

    private static Wallet SelectWalletToDelete(IReadOnlyList<Wallet> choices)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<Wallet>()
                .Title(Constants.Prompts.DeleteWalletSelection)
                .UseConverter(w => Constants.Prompts.WalletDisplay(
                    w.Name, w.BaseCurrency, w.Balance.Amount))
                .AddChoices(choices));
    }
}
