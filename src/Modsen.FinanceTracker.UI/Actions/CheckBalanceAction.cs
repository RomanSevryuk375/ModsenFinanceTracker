using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Infrastructure.Configuration;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public sealed class CheckBalanceAction(IFinanceService financeService) : IMenuAction
{
    public string Name => Constants.MainMenu.ActionBalance;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var balance = await financeService.GetBalanceAsync(cancellationToken);
        var currency = AppConfiguration.Instance.Currency;

        var color = balance >= 0
            ? Constants.Colors.Success
            : Constants.Colors.Error;

        var panel = CreateBalancePanel(balance, currency, color);

        AnsiConsole.Write(panel);
    }

    private static Panel CreateBalancePanel(decimal balance, string currency, string color)
    {
        var message = string.Format(Constants.Balance.MessageTemplate, color, balance, currency);

        return new Panel(Align.Center(new Markup(message), VerticalAlignment.Middle))
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(
                Constants.Layout.PanelPaddingHorizontal,
                Constants.Layout.PanelPaddingVertical,
                Constants.Layout.PanelPaddingHorizontal,
                Constants.Layout.PanelPaddingVertical),
            Header = new PanelHeader(Constants.Balance.Header)
        };
    }
}