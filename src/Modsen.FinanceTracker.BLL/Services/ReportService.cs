using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.BLL.Services;

public sealed class ReportService(IRepository<Transaction> repository) : IReportService
{
    public async Task ExportAsync(
        IExportStrategy strategy, 
        string filePath, 
        CancellationToken cancellationToken)
    {
        var transactions = await repository.GetAllAsync(
            cancellationToken: cancellationToken);

        await strategy.ExportAsync(transactions, filePath, cancellationToken);
    }
}