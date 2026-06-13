using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.Domain.Interfaces;

public interface ITransactionRepository
{
    Task AddAsync(Transaction entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Transaction entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}