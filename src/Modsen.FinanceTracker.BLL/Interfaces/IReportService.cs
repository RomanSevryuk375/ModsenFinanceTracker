namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface IReportService
{
    public Task ExportAsync(
        Guid walletId,
        IExportStrategy strategy,
        string filePath,
        CancellationToken cancellationToken);
}