using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public class DeleteTransectionAction : IMenuAction
{
    public string Name => Constants.MainMenu.ActionDelete;
    public void Execute()
    {
        AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]Delete logic will be here (Task 3)[/]");
    }
}