using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.BLL.Services;

public sealed class WalletService(
    IWalletRepository repository) : IWalletService
{
    public async Task<Result<IReadOnlyList<Wallet>>> GetAllWallets(
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Wallet> wallets = await repository.GetAllAsync(null, null, null, cancellationToken);

        return Result.Success<IReadOnlyList<Wallet>>([.. wallets]);
    }
    public async Task<Result> CreateWalletAsync(
        string name,
        string baseCurrency,
        CancellationToken cancellationToken = default)
    {
        Result<Wallet> createResult = Wallet.Create(name, baseCurrency);
        if (createResult.IsFailure)
        {
            return Result.Fail(createResult.Error);
        }

        await repository.AddAsync(createResult.Value, cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteWalletAsync(
        Guid walletId,
        CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(walletId, cancellationToken);

        return Result.Success();
    }
}
