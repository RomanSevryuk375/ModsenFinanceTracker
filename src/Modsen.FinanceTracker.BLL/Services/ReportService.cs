using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Services;

public sealed class ReportService(IWalletRepository repository) : IReportService
{
    public async Task<Result> ExportAsync(
        Guid walletId,
        IExportStrategy strategy,
        string filePath,
        CancellationToken cancellationToken)
    {
        Wallet? wallet = await repository.GetByIdAsync(walletId, cancellationToken);
        if (wallet is null)
        {
            return Result.Fail($"Wallet {walletId} not found.");
        }

        await strategy.ExportAsync(wallet.Transactions, filePath, cancellationToken);

        return Result.Success();
    }
}
