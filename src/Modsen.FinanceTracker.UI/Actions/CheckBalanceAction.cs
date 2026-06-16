using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.UI.Actions;

public sealed class CheckBalanceAction(
    IFinanceService financeService,
    IWalletService walletService,
    string systemCurrecy) : IMenuAction
{
    private readonly string _systemCurrecy = systemCurrecy;

    public string Name => Constants.MainMenu.ActionBalance;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Wallet? wallet = await UIHelper.PromptWalletAsync(walletService, cancellationToken);
        if (wallet is null)
        {
            return;
        }

        Result<decimal> balanceResult = await financeService.GetBalanceAsync(wallet.Id, cancellationToken);

        if (balanceResult.IsFailure)
        {
            AnsiConsole.MarkupLine($"[{Constants.Colors.Error}]{balanceResult.Error}[/]");
            return;
        }

        string color = balanceResult.Value >= 0
            ? Constants.Colors.Success
            : Constants.Colors.Error;
        string message = string.Format(
            Constants.Balance.MessageTemplate, color, balanceResult.Value, _systemCurrecy);

        var panel = new Panel(Align.Center(new Markup(message), VerticalAlignment.Middle))
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(
                Constants.Layout.PanelPaddingHorizontal,
                Constants.Layout.PanelPaddingVertical,
                Constants.Layout.PanelPaddingHorizontal,
                Constants.Layout.PanelPaddingVertical),
            Header = new PanelHeader($"{wallet.Name} {Constants.Balance.Header}")
        };

        AnsiConsole.Write(panel);
    }
}
