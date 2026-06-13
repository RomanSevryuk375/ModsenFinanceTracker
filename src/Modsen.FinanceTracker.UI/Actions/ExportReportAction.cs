using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.BLL.Strategies;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public sealed class ExportReportAction(IReportService reportService) : IMenuAction
{
    public string Name => $"{Constants.MainMenu.ActionExportReport}";

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var format = PromptFormat();

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        var (strategy, extension) = GetStrategyAndExtension(format);

        var fullPath = GenerateReportPath(format, extension);

        await reportService.ExportAsync(strategy, fullPath, cancellationToken);

        AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]Report exported to {fullPath}[/]");
    }

    private static string PromptFormat()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select export format:")
                .AddChoices("CSV", "TXT"));
    }

    private static (IExportStrategy strategy, string extension) GetStrategyAndExtension(string format)
    {
        return format switch
        {
            "CSV" => (new CsvExportStrategy(), ".csv"),
            "TXT" => (new TxtExportStrategy(), ".txt"),

            _ => throw new ArgumentException("Invalid format")
        };
    }

    private static string GenerateReportPath(string format, string extension)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
        var fileName = $"FinanceReport_{timestamp}_{format}{extension}";

        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
    }
}