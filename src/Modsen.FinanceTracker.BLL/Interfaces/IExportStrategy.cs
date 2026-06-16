namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface IExportStrategy
{
    public Task ExportAsync(
        IEnumerable<Transaction> transactions, 
        string filePath, 
        CancellationToken cancellationToken);
}