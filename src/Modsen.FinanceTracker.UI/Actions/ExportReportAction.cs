using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.BLL.Strategies;
using Modsen.FinanceTracker.UI.Interfaces;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI.Actions;

public class ExportReportAction : IMenuAction
{

    private readonly IReportService _reportService;

    public ExportReportAction(IReportService reportService)
    {
        _reportService = reportService;
    }

    public string Name => $"{Constants.MainMenu.ActionExportReport}"; 

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var format = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select export format:")
                .AddChoices("CSV", "TXT"));
        
        var (strategy, extension) = format switch
        {
            "CSV" => ((IExportStrategy)new CsvExportStrategy(), ".csv"),
            "TXT" => (new TxtExportStrategy(), ".txt"),
            _ => throw new ArgumentException("Invalid format")
        };

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
        var generatedFileName = $"FinanceReport_{timestamp}_{format}{extension}";
        
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, generatedFileName);
        
        await _reportService.ExportAsync(strategy, path, ct);

        AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]Report exported to {path}[/]");
    }
}