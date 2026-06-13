using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface IExportStrategy
{
    Task ExportAsync(
        IEnumerable<Transaction> transactions, 
        string filePath, 
        CancellationToken cancellationToken);
}