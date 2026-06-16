namespace Modsen.FinanceTracker.DAL;

public sealed class UnitOfWork(
    JsonDbContext jsonDbContext,
    ILogger<UnitOfWork> logger) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("UnitOfWork is committing transaction...");

        await jsonDbContext.SaveChangesAsync(cancellationToken);

        logger.LogDebug("UnitOfWork transaction committed.");
    }
}
