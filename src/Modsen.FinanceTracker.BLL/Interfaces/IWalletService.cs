using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface IWalletService
{
    public Task<Result> CreateWalletAsync(
        string name, 
        string baseCurrency, 
        CancellationToken cancellationToken = default);

    public Task<Result> DeleteWalletAsync(
        Guid walletId, 
        CancellationToken cancellationToken = default);

    public Task<Result<IReadOnlyList<Wallet>>> GetAllWallets(
        CancellationToken cancellationToken = default);
}