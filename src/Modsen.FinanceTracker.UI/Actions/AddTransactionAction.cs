using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public class AddTransactionAction : IMenuAction
{
    public string Name => Constants.MainMenu.ActionAdd;
    public async Task ExecuteAsync(CancellationToken ct)
    {
        AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]Adding logic will be here (Task 3)[/]");
    }
}