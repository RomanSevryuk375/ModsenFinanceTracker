using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface IReportService
{
    public Task<Result> ExportAsync(
        Guid walletId,
        IExportStrategy strategy,
        string filePath,
        CancellationToken cancellationToken);
}
