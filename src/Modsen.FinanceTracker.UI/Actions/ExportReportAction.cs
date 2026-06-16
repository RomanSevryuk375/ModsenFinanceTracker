using Modsen.FinanceTracker.BLL.Strategies;

namespace Modsen.FinanceTracker.UI.Actions;

public sealed class ExportReportAction(
    IReportService reportService,
    IWalletService walletService) : IMenuAction
{
    private const string DateFormat = "yyyy-MM-dd_HH-mm";

    public string Name => Constants.MainMenu.ActionExportReport;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Wallet? wallet = await UIHelper.PromptWalletAsync(walletService, cancellationToken);
        if (wallet is null)
        {
            return;
        }

        string format = PromptFormat();
        (IExportStrategy strategy, string extension) = GetStrategyAndExtension(format);
        string fullPath = GenerateReportPath(format, extension);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        await reportService.ExportAsync(wallet.Id, strategy, fullPath, cancellationToken);

        AnsiConsole.MarkupLine($"[{Constants.Colors.Success}]{Constants.Success.ExportReport} {fullPath}[/]");
    }

    private static string PromptFormat()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(Constants.Prompts.ExportFormat)
                .AddChoices("CSV", "TXT", "PDF", "DOCX"));
    }

    private static (IExportStrategy strategy, string extension) GetStrategyAndExtension(string format)
    {
        return format switch
        {
            "CSV" => (new CsvExportStrategy(), ".csv"),
            "TXT" => (new TxtExportStrategy(), ".txt"),
            "PDF" => (new PdfExportStrategy(), ".pdf"),
            "DOCX" => (new DocxExportStrategy(), ".docx"),

            _ => throw new ArgumentException("Invalid format")
        };
    }

    private static string GenerateReportPath(string format, string extension)
    {
        string timestamp = DateTime.Now.ToString(DateFormat);
        string fileName = $"FinanceReport_{timestamp}_{format}{extension}";

        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
    }
}