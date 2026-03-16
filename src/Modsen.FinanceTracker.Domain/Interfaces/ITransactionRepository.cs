using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.Domain.Interfaces;

public interface ITransactionRepository
{
    Task AddAsync(Transaction entity, CancellationToken ct = default);
    Task UpdateAsync(Transaction entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}