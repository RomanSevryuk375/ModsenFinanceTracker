using Modsen.FinanceTracker.DAL.Context;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Repositories;

public sealed class JsonTransactionRepository(JsonDbContext context)
    : BaseRepository<Transaction>(context.Transactions), ITransactionRepository
{
    public override async Task AddAsync(
        Transaction entity, CancellationToken cancellationToken = default)
    {
        await base.AddAsync(entity, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public override async Task UpdateAsync(
        Transaction entity, CancellationToken cancellationToken = default)
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