using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.BLL.Services;

public class ReportService
{
    private readonly IRepository<Transaction> _repository;

    public ReportService(IRepository<Transaction> repository)
    {
        _repository = repository;
    }

    public async Task ExportAsync(IExportStrategy strategy, string filePath, CancellationToken ct)
    {
        var transactions = await _repository.GetAllAsync(ct: ct);
        await strategy.ExportAsync(transactions, filePath, ct);
    }
}