using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public class ExportReportAction : IMenuAction
{

    public string Name => Constants.Reports.ActionExportReport; 

    public async Task ExecuteAsync(CancellationToken ct)
    {
        AnsiConsole.MarkupLine($"[{Constants.Colors.Info}]Export report will be here in future[/]");
    }
}