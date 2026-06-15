namespace Modsen.FinanceTracker.Domain.Interfaces;

public interface IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken = default);
}