using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public class CheckBalanceAction : IMenuAction
{
    public string Name => Constants.MainMenu.ActionBalance;
    public async Task ExecuteAsync(CancellationToken ct)
    {
        AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]Current Balance: Mock[/]");
    }
}