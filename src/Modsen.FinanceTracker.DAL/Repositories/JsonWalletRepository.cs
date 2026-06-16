namespace Modsen.FinanceTracker.DAL.Repositories;

public sealed class JsonWalletRepository(JsonDbContext context)
    : BaseRepository<Wallet>(context.Wallets), IWalletRepository
{
    public override async Task AddAsync(
        Wallet entity, CancellationToken cancellationToken = default)
    {
        await base.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public override async Task UpdateAsync(
        Wallet entity, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public override async Task DeleteAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        await base.DeleteAsync(id, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
