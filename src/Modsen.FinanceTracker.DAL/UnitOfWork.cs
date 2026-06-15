using Modsen.FinanceTracker.DAL.Context;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL;

public sealed class UnitOfWork(JsonDbContext jsonDbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await jsonDbContext.SaveChangesAsync(cancellationToken);
    }
}
