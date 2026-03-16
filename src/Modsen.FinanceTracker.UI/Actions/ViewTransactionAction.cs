using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public class ViewTransactionAction : IMenuAction
{
    public string Name => Constants.MainMenu.ActionView;
    public async Task ExecuteAsync(CancellationToken ct)
    {
        AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]History view will be here (Task 3)[/]");
    }
}