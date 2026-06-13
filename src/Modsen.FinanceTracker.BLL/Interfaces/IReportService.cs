namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface IReportService
{
    Task ExportAsync(
        IExportStrategy strategy,
        string filePath,
        CancellationToken cancellationToken);
}