using Microsoft.Extensions.Logging;
using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Decorators;

public sealed class WalletServiceLoggingDecorator(
    IWalletService inner,
    ILogger<WalletServiceLoggingDecorator> logger) : IWalletService
{
    public async Task<Result> CreateWalletAsync(
        string name,
        string baseCurrency,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attempting to create wallet '{Name}' with currency {Currency}",
            name, baseCurrency);
        Result result = await inner.CreateWalletAsync(name, baseCurrency, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to create wallet: {Error}", result.Error);
        }
        else
        {
            logger.LogInformation("Wallet '{Name}' created successfully.", name);
        }

        return result;
    }

    public async Task<Result> DeleteWalletAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attempting to delete wallet {WalletId}", walletId);
        Result result = await inner.DeleteWalletAsync(walletId, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to delete wallet: {Error}", result.Error);
        }
        else
        {
            logger.LogInformation("Wallet {WalletId} deleted successfully.", walletId);
        }

        return result;
    }

    public async Task<Result<IReadOnlyList<Wallet>>> GetAllWallets(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching all wallets.");
        return await inner.GetAllWallets(cancellationToken);
    }
}
