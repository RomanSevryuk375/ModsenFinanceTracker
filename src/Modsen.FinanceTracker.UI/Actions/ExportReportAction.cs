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
        var format = PromptFormat();
        
        if (ct.IsCancellationRequested)
        {
            return;
        }

        var (strategy, extension) = GetStrategyAndExtension(format);
        
        var fullPath = GenerateReportPath(format, extension);
        
        await _reportService.ExportAsync(strategy, fullPath, ct);

        AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]Report exported to {fullPath}[/]");
    }

    private string PromptFormat()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select export format:")
                .AddChoices("CSV", "TXT"));
    }

    private (IExportStrategy strategy, string extension) GetStrategyAndExtension(string format)
    {
        return format switch
        {
            "CSV" => (new CsvExportStrategy(), ".csv"),
            "TXT" => (new TxtExportStrategy(), ".txt"),
            
            _ => throw new ArgumentException("Invalid format")
        };
    }

    private string GenerateReportPath(string format, string extension)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
        var fileName = $"FinanceReport_{timestamp}_{format}{extension}";
    
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
    }
}