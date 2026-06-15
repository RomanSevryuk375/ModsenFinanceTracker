using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.BLL.Services;

public sealed class ReportService(IWalletRepository repository) : IReportService
{
    public async Task ExportAsync(
        Guid walletId, 
        IExportStrategy strategy,
        string filePath, 
        CancellationToken cancellationToken)
    {
        Wallet wallet = await repository.GetByIdAsync(walletId, cancellationToken) ??
            throw new ArgumentException($"Wallet {walletId} not found");
        await strategy.ExportAsync(wallet.MutableTransactions, filePath, cancellationToken);
    }
}