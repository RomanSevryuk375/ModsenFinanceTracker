using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.UI.Actions.WalletActions;

public sealed class ViewWalletsAction(
    IWalletService walletService,
    IWalletListView view) : IWalletMenuAction
{
    public string Name => Constants.MainMenu.ViewWallets;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<Wallet>> result = await walletService.GetAllWallets(cancellationToken);
        if (result.IsFailure)
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Error}]Error: {result.Error}[/]");
            return;
        }
        IReadOnlyList<Wallet> wallets = result.Value;
        if (wallets.Count == 0)
        {
            AnsiConsole.MarkupLine(Constants.Info.NoWalletsToView);
            return;
        }

        view.Render(wallets);
    }
}