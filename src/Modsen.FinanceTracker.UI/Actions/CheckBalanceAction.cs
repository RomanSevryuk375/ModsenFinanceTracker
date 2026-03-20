using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Infrastructure.Configuration;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public class CheckBalanceAction : IMenuAction
{
    private readonly IFinanceService _financeService;

    public CheckBalanceAction(IFinanceService financeService)
    {
        _financeService = financeService;
    }

    public string Name => Constants.MainMenu.ActionBalance;

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var balance = await _financeService.GetBalanceAsync(ct);
        
        var currency = AppConfiguration.Instance.Currency;
        
        var color = balance >= 0 
            ? Constants.Colors.Success 
            : Constants.Colors.Error;

        var panel = new Panel(
            Align.Center(
                new Markup($"Your current balance is: [{color}]{balance:N2} {currency}[/]"),
                VerticalAlignment.Middle))
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(2, 1, 2, 1),
            Header = new PanelHeader("Summary")
        };

        AnsiConsole.Write(panel);
    }
}