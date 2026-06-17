namespace Modsen.FinanceTracker.DAL.Repositories;

public sealed class JsonWalletRepository(JsonDbContext context)
    : BaseRepository<Wallet>(context.Wallets), IWalletRepository
{
}
